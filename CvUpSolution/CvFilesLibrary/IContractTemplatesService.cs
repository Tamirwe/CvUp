using DataModelsLibrary.Models;

namespace CvFilesLibrary
{
    public interface IContractTemplatesService
    {
        bool IsAllowedFile(string fileName);
        Task<List<ContractTemplateModel>> GetContractTemplates();
        Task<ContractTemplateModel> AddContractTemplate(string name, string? descr, string fileName, Stream fileContent);
        Task<Stream?> GetContractTemplatePdf(int id);
        Task DeleteContractTemplate(int id);
    }
}
