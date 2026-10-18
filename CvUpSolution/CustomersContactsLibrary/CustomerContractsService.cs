using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using EmailsLibrary;
using Microsoft.Extensions.Configuration;

namespace CustomersContactsLibrary
{
    public class CustomerContractsService : ICustomerContractsService
    {
        // The templates folder is not created on read. Until someone uploads a
        // template there is nothing to send, and the module says so rather than
        // leaving an empty directory behind on every page load.
        private const string TemplatesFolderName = "customerContractTemplates";
        private const string DocxExtension = ".docx";

        private readonly string _filesRootFolder;
        private readonly ICustomerContractsQueries _customerContractsQueries;
        private readonly IEmailService _emailService;

        public CustomerContractsService(
            IConfiguration configuration,
            ICustomerContractsQueries customerContractsQueries,
            IEmailService emailService)
        {
            _filesRootFolder = configuration["CVS_ROOT_FOLDER"]!;
            _customerContractsQueries = customerContractsQueries;
            _emailService = emailService;
        }

        public async Task<List<ContractTemplateModel>> GetContractTemplates(int companyId)
        {
            List<ContractTemplateModel> templates = await _customerContractsQueries.GetContractTemplates();
            string folder = TemplatesFolder(companyId);
            bool folderExists = Directory.Exists(folder);

            foreach (var template in templates)
            {
                template.fileExists = folderExists
                                      && !string.IsNullOrEmpty(template.name)
                                      && File.Exists(Path.Combine(folder, template.name));
            }

            return templates;
        }

        // The row's name IS the file name, so a template is found on disk by
        // the name it shows in the list -- there is nothing to keep in sync.
        public async Task<ContractTemplateModel> AddContractTemplate(
            int companyId, string name, string? descr, string fileName, Stream file)
        {
            if (Path.GetExtension(fileName).ToLower() != DocxExtension)
            {
                throw new ArgumentException("Only .docx templates can be uploaded.");
            }

            string folder = TemplatesFolder(companyId);
            Directory.CreateDirectory(folder);

            // A typed name wins over the uploaded file's own, but it is still a
            // file name: it has to end in .docx and not collide with an existing
            // template.
            string wanted = string.IsNullOrWhiteSpace(name)
                ? Path.GetFileName(fileName)
                : EnsureDocx(name);

            string storedFileName = UniqueFileName(folder, wanted);

            using (var target = new FileStream(Path.Combine(folder, storedFileName), FileMode.Create))
            {
                await file.CopyToAsync(target);
            }

            contract_template rec = await _customerContractsQueries.AddContractTemplate(
                new ContractTemplateModel
                {
                    name = storedFileName,
                    descr = descr,
                });

            return new ContractTemplateModel
            {
                id = rec.id,
                name = rec.name,
                descr = rec.descr,
                dateCreated = rec.date_created,
                dateUpdated = rec.date_updated,
                fileExists = true,
            };
        }

        // Renaming a template renames the document with it. Replacing the
        // content itself is an upload, not an update.
        public async Task<ContractTemplateModel?> UpdateContractTemplate(int companyId, ContractTemplateModel data)
        {
            ContractTemplateModel? current = await _customerContractsQueries.GetContractTemplate(data.id);

            if (current == null)
            {
                return null;
            }

            string folder = TemplatesFolder(companyId);
            string newName = string.IsNullOrWhiteSpace(data.name)
                ? current.name
                : EnsureDocx(data.name);

            if (newName != current.name)
            {
                string currentPath = Path.Combine(folder, current.name);

                if (File.Exists(currentPath))
                {
                    newName = UniqueFileName(folder, newName);
                    File.Move(currentPath, Path.Combine(folder, newName));
                }
            }

            data.name = newName;
            contract_template? rec = await _customerContractsQueries.UpdateContractTemplate(data);

            if (rec == null)
            {
                return null;
            }

            return new ContractTemplateModel
            {
                id = rec.id,
                name = rec.name,
                descr = rec.descr,
                dateCreated = rec.date_created,
                dateUpdated = rec.date_updated,
                fileExists = File.Exists(TemplateFilePath(companyId, rec.name)),
            };
        }

        public async Task DeleteContractTemplate(int companyId, int id)
        {
            ContractTemplateModel? template = await _customerContractsQueries.GetContractTemplate(id);

            await _customerContractsQueries.DeleteContractTemplate(id);

            if (template != null && !string.IsNullOrEmpty(template.name))
            {
                string path = TemplateFilePath(companyId, template.name);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        public async Task<CustomerContractModel?> GetCustomerLatestContract(int companyId, int customerId)
        {
            return await _customerContractsQueries.GetLatestCustomerContract(companyId, customerId);
        }

        public async Task<List<CustomerContractModel>> GetLatestContracts(int companyId, int take)
        {
            return await _customerContractsQueries.GetLatestContracts(companyId, take);
        }

        public async Task<string> SendContract(int companyId, SendContractModel data, UserModel? user)
        {
            if (user == null)
            {
                throw new InvalidOperationException("No user to send the contract from.");
            }

            ContractMergeDataModel? mergeData =
                await _customerContractsQueries.GetMergeData(companyId, data.customerId, data.contactId);

            if (mergeData == null)
            {
                throw new InvalidOperationException("Customer or contact not found.");
            }

            if (string.IsNullOrWhiteSpace(mergeData.contactEmail))
            {
                throw new InvalidOperationException("The selected contact has no email address.");
            }

            ContractTemplateModel? template =
                await _customerContractsQueries.GetContractTemplate(data.contractTemplateId);

            if (template == null || string.IsNullOrEmpty(template.name))
            {
                throw new InvalidOperationException("Contract template not found.");
            }

            string templatePath = TemplateFilePath(companyId, template.name);

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException("The template file is missing from the company folder.", template.name);
            }

            string attachmentName = AttachmentFileName(mergeData.customerName);
            MemoryStream contract = MergeContract(templatePath, mergeData);

            var email = new EmailModel
            {
                To = new List<EmailAddress>
                {
                    new EmailAddress { Name = mergeData.contactName, Address = mergeData.contactEmail },
                },
                Subject = data.subject,
                Body = $"{data.body} {user.signature} ",
                Attachments = new List<AttachmentModel>
                {
                    new AttachmentModel
                    {
                        Attachment = contract,
                        name = attachmentName,
                        fileName = $"{attachmentName}{DocxExtension}",
                        mediaType = "application",
                        mediaSubtype = "vnd.openxmlformats-officedocument.wordprocessingml.document",
                    },
                },
                From = new EmailAddress { Address = user.email, Name = $"{user.firstName} {user.lastName}" },
                MailSenderUserName = user.mailUsername,
                MailSenderPassword = user.mailPassword,
            };

            await _emailService.Send(email);
            await _customerContractsQueries.AddCustomerContract(data, DateTime.Now);

            return $"{attachmentName}{DocxExtension}";
        }

        // Placeholders are plain text in the document, so a straight replace is
        // enough -- no merge fields to maintain in Word.
        private MemoryStream MergeContract(string templatePath, ContractMergeDataModel mergeData)
        {
            var document = new Spire.Doc.Document(templatePath);

            document.Replace("{{CustomerName}}", mergeData.customerName ?? string.Empty, false, true);
            document.Replace("{{CustomerAddress}}", mergeData.customerAddress ?? string.Empty, false, true);
            document.Replace("{{CustomerDescr}}", mergeData.customerDescr ?? string.Empty, false, true);
            document.Replace("{{Date}}", DateTime.Now.ToString("dd/MM/yyyy"), false, true);

            var stream = new MemoryStream();
            document.SaveToFile(stream, Spire.Doc.FileFormat.Docx);
            stream.Seek(0, SeekOrigin.Begin);
            document.Close();

            return stream;
        }

        private string TemplatesFolder(int companyId)
        {
            return $"{_filesRootFolder}\\_{companyId}\\{TemplatesFolderName}";
        }

        private string TemplateFilePath(int companyId, string fileName)
        {
            return Path.Combine(TemplatesFolder(companyId), fileName);
        }

        // "Contract - Acme Ltd". Characters Windows will not accept in a file
        // name are dropped rather than rejected -- a customer name is free text.
        private string AttachmentFileName(string customerName)
        {
            string cleanName = string.Concat(
                (customerName ?? string.Empty).Where(c => !Path.GetInvalidFileNameChars().Contains(c))).Trim();

            return string.IsNullOrEmpty(cleanName) ? "Contract" : $"Contract - {cleanName}";
        }

        // The name doubles as the file name, so it has to survive as one: no
        // characters Windows rejects, and always the .docx extension.
        private string EnsureDocx(string name)
        {
            string cleanName = string.Concat(
                name.Where(c => !Path.GetInvalidFileNameChars().Contains(c))).Trim();

            if (string.IsNullOrEmpty(cleanName))
            {
                cleanName = "template";
            }

            return cleanName.ToLower().EndsWith(DocxExtension)
                ? cleanName
                : $"{cleanName}{DocxExtension}";
        }

        private string UniqueFileName(string folder, string fileName)
        {
            string name = Path.GetFileNameWithoutExtension(fileName);
            string candidate = fileName;
            int suffix = 1;

            while (File.Exists(Path.Combine(folder, candidate)))
            {
                candidate = $"{name} ({suffix}){DocxExtension}";
                suffix++;
            }

            return candidate;
        }
    }
}
