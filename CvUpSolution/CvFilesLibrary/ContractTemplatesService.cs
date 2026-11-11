using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using EmailsLibrary;
using Microsoft.Extensions.Configuration;
using Spire.Pdf;
using Spire.Pdf.Texts;

namespace CvFilesLibrary
{
    public class ContractTemplatesService : IContractTemplatesService
    {
        private static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx" };

        // Free Spire appends an evaluation notice as page 4 of a converted document
        private const int SpireEvalPageIndex = 3;

        private const string CustomerNamePlaceholder = "{{CustomerName}}";
        private const string CompanyAddressPlaceholder = "{{CompanyAddress}}";
        private const string CompanyIdPlaceholder = "{{CompanyId}}";
        private const string DatePlaceholder = "{{Date}}";
        private const string DateFormat = "dd/MM/yyyy";

        // Written where an optional value was left empty, so it can be filled in by hand
        private const string EmptyValueLine = "___________";

        private IContractTemplatesQueries _contractTemplatesQueries;
        private IEmailService _emailService;
        private string _templatesFolder;

        public ContractTemplatesService(IConfiguration configuration, IContractTemplatesQueries contractTemplatesQueries, IEmailService emailService)
        {
            _emailService = emailService;
            _templatesFolder = Path.Combine(configuration["APP_LOCAL_ROOT_FOLDER"]!, "contractTemplates");
            _contractTemplatesQueries = contractTemplatesQueries;
        }

        public bool IsAllowedFile(string fileName)
        {
            return AllowedExtensions.Contains(Path.GetExtension(fileName).ToLowerInvariant());
        }

        public async Task<List<ContractTemplateModel>> GetContractTemplates()
        {
            return await _contractTemplatesQueries.GetContractTemplates();
        }

        /// <summary>
        /// The file is saved as {id}{extension}. If saving it fails the table row is
        /// removed, so the list never shows a template that has no document.
        /// </summary>
        public async Task<ContractTemplateModel> AddContractTemplate(string name, string? descr, string fileName, Stream fileContent)
        {
            var template = await _contractTemplatesQueries.AddContractTemplate(name, descr);
            var filePath = Path.Combine(_templatesFolder, $"{template.id}{Path.GetExtension(fileName).ToLowerInvariant()}");

            try
            {
                Directory.CreateDirectory(_templatesFolder);

                using var file = File.Create(filePath);
                await fileContent.CopyToAsync(file);
            }
            catch
            {
                File.Delete(filePath);
                await _contractTemplatesQueries.DeleteContractTemplate(template.id);
                throw;
            }

            return template;
        }

        public async Task<Stream?> GetContractTemplatePdf(int id)
        {
            var filePath = FindTemplateFile(id);

            if (filePath == null)
            {
                return null;
            }

            if (Path.GetExtension(filePath) == ".pdf")
            {
                return File.OpenRead(filePath);
            }

            return await Task.Run(() => ConvertWordToPdf(filePath));
        }

        /// <summary>
        /// The uploaded file as it was stored, in its original format.
        /// Returns null when the template has no file.
        /// </summary>
        public ContractTemplateFileModel? GetContractTemplateOriginalFile(int id)
        {
            var filePath = FindTemplateFile(id);

            if (filePath == null)
            {
                return null;
            }

            var extension = Path.GetExtension(filePath);

            return new ContractTemplateFileModel
            {
                content = File.OpenRead(filePath),
                contentType = extension switch
                {
                    ".pdf" => "application/pdf",
                    ".doc" => "application/msword",
                    _ => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                },
            };
        }

        public async Task DeleteContractTemplate(int id)
        {
            await _contractTemplatesQueries.DeleteContractTemplate(id);

            var filePath = FindTemplateFile(id);

            if (filePath != null)
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Attaches the uploaded file in its original format, not the converted preview,
        /// because converting a Word document to pdf cuts it off after 3 pages.
        /// The placeholders are filled in a copy; the stored template is not changed.
        /// Returns false when the template or its file no longer exists.
        /// </summary>
        public async Task<bool> SendContract(SendContractModel contract, UserModel user)
        {
            var template = await _contractTemplatesQueries.GetContractTemplate(contract.contractTemplateId);
            var filePath = FindTemplateFile(contract.contractTemplateId);

            if (template == null || filePath == null)
            {
                return false;
            }

            var attachment = new AttachmentModel
            {
                name = template.name,
                extension = Path.GetExtension(filePath),
                Attachment = await Task.Run(() => FillContractPlaceholders(filePath, contract)),
            };

            await _emailService.Send(new EmailModel
            {
                To = new List<EmailAddress>
                {
                    new EmailAddress { Name = contract.contactName ?? "", Address = contract.emailAddress },
                },
                Subject = contract.emailSubject,
                Body = $"{contract.emailBody} {user.signature} ",
                Attachments = new List<AttachmentModel> { attachment },
                From = new EmailAddress { Address = user.email, Name = $"{user.firstName} {user.lastName}" },
                MailSenderUserName = user.mailUsername,
                MailSenderPassword = user.mailPassword,
            });

            await _contractTemplatesQueries.AddContractSend(contract, template);

            return true;
        }

        public async Task<List<ContractSendModel>> GetContractSends()
        {
            return await _contractTemplatesQueries.GetContractSends();
        }

        private static string ValueOrEmptyLine(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? EmptyValueLine : value.Trim();
        }

        private MemoryStream FillContractPlaceholders(string filePath, SendContractModel contract)
        {
            var placeholders = new Dictionary<string, string>
            {
                [CustomerNamePlaceholder] = contract.companyName.Trim(),
                [CompanyAddressPlaceholder] = ValueOrEmptyLine(contract.companyAddress),
                [CompanyIdPlaceholder] = ValueOrEmptyLine(contract.companyIdNumber),
                // The send date, in the server's local time
                [DatePlaceholder] = DateTime.Now.ToString(DateFormat, System.Globalization.CultureInfo.InvariantCulture),
            };

            var extension = Path.GetExtension(filePath);
            var filled = new MemoryStream();

            if (extension == ".pdf")
            {
                var pdf = new PdfDocument();
                pdf.LoadFromFile(filePath);

                foreach (PdfPageBase page in pdf.Pages)
                {
                    var replacer = new PdfTextReplacer(page);

                    foreach (var (placeholder, value) in placeholders)
                    {
                        replacer.ReplaceAllText(placeholder, value);
                    }
                }

                pdf.SaveToStream(filled, FileFormat.PDF);
                pdf.Close();
            }
            else
            {
                var document = new Spire.Doc.Document(filePath);

                foreach (var (placeholder, value) in placeholders)
                {
                    // Plain text match, case sensitive, not limited to whole words
                    document.Replace(placeholder, value, true, false);
                }

                document.SaveToStream(filled, extension == ".doc" ? Spire.Doc.FileFormat.Doc : Spire.Doc.FileFormat.Docx);
                document.Close();
            }

            filled.Seek(0, SeekOrigin.Begin);

            return filled;
        }

        private string? FindTemplateFile(int id)
        {
            return Directory.Exists(_templatesFolder)
                ? Directory.GetFiles(_templatesFolder, $"{id}.*").FirstOrDefault()
                : null;
        }

        private Stream ConvertWordToPdf(string filePath)
        {
            var document = new Spire.Doc.Document(filePath);
            var converted = new MemoryStream();

            document.SaveToFile(converted, Spire.Doc.FileFormat.PDF);
            converted.Seek(0, SeekOrigin.Begin);

            var pdf = new PdfDocument(converted);

            if (pdf.Pages.Count > SpireEvalPageIndex)
            {
                pdf.Pages.RemoveAt(SpireEvalPageIndex);
            }

            var pdfStream = pdf.SaveToStream(FileFormat.PDF).Cast<MemoryStream>().First();
            pdfStream.Seek(0, SeekOrigin.Begin);

            return pdfStream;
        }
    }
}
