using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using Microsoft.Extensions.Configuration;
using Spire.Pdf;

namespace CvFilesLibrary
{
    public class ContractTemplatesService : IContractTemplatesService
    {
        private static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx" };

        // Free Spire appends an evaluation notice as page 4 of a converted document
        private const int SpireEvalPageIndex = 3;

        private IContractTemplatesQueries _contractTemplatesQueries;
        private string _templatesFolder;

        public ContractTemplatesService(IConfiguration configuration, IContractTemplatesQueries contractTemplatesQueries)
        {
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

        public async Task DeleteContractTemplate(int id)
        {
            await _contractTemplatesQueries.DeleteContractTemplate(id);

            var filePath = FindTemplateFile(id);

            if (filePath != null)
            {
                File.Delete(filePath);
            }
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
