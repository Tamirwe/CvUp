using Database.models;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface ICustomerContractsQueries
    {
        Task<List<ContractTemplateModel>> GetContractTemplates();
        Task<ContractTemplateModel?> GetContractTemplate(int id);
        Task<contract_template> AddContractTemplate(ContractTemplateModel data);
        Task<contract_template?> UpdateContractTemplate(ContractTemplateModel data);
        Task DeleteContractTemplate(int id);

        Task<customer_contract> AddCustomerContract(SendContractModel data, DateTime dateSent);
        Task<CustomerContractModel?> GetLatestCustomerContract(int companyId, int customerId);
        Task<List<CustomerContractModel>> GetLatestContracts(int companyId, int take);
        Task<ContractMergeDataModel?> GetMergeData(int companyId, int customerId, int contactId);
    }
}
