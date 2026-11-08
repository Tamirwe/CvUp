using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface IContractTemplatesQueries
    {
        Task<List<ContractTemplateModel>> GetContractTemplates();
        Task<ContractTemplateModel> AddContractTemplate(string name, string? descr);
        Task DeleteContractTemplate(int id);
    }
}
