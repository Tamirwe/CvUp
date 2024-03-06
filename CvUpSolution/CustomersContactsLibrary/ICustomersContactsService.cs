using Database.models;
using DataModelsLibrary.Models;

namespace CustomersContactsLibrary
{
    public interface ICustomersContactsService
    {
        Task<contact> AddContact(int companyId, ContactModel data);
        Task DeleteContact(int companyId, int id);
        Task<List<ContactModel>> GetContacts(int companyId);
        Task<contact> UpdateContact(int companyId, ContactModel data);
        public Task<customer> AddCustomer(CustomerModel data, int companyId);
        public Task<customer?> UpdateCustomer(CustomerModel data, int companyId);
        public Task<List<CustomerModel>> GetCustomersList(int companyId);
        public Task DeleteCustomer(int companyId, int id);
    }
}
