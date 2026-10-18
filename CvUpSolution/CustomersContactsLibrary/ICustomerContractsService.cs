using DataModelsLibrary.Models;

namespace CustomersContactsLibrary
{
    public interface ICustomerContractsService
    {
        Task<List<ContractTemplateModel>> GetContractTemplates(int companyId);
        Task<ContractTemplateModel> AddContractTemplate(int companyId, string name, string? descr, string fileName, Stream file);
        Task<ContractTemplateModel?> UpdateContractTemplate(int companyId, ContractTemplateModel data);
        Task DeleteContractTemplate(int companyId, int id);

        Task<CustomerContractModel?> GetCustomerLatestContract(int companyId, int customerId);
        Task<List<CustomerContractModel>> GetLatestContracts(int companyId, int take);
        Task<string> SendContract(int companyId, SendContractModel data, UserModel? user);
    }
}
