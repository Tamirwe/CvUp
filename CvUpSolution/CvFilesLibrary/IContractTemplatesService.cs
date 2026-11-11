using DataModelsLibrary.Models;

namespace CvFilesLibrary
{
    public interface IContractTemplatesService
    {
        bool IsAllowedFile(string fileName);
        Task<List<ContractTemplateModel>> GetContractTemplates();
        Task<ContractTemplateModel> AddContractTemplate(string name, string? descr, string fileName, Stream fileContent);
        Task<Stream?> GetContractTemplatePdf(int id);
        ContractTemplateFileModel? GetContractTemplateOriginalFile(int id);
        Task DeleteContractTemplate(int id);
        Task<bool> SendContract(SendContractModel contract, UserModel user);
        Task<List<ContractSendModel>> GetContractSends();
    }
}
