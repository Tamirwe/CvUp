using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using Microsoft.Extensions.Configuration;

namespace CustomersContactsLibrary
{
    public class CustomersContactsService:ICustomersContactsService
    {
        private IConfiguration _configuration;
        private IContactsQueries _contactsQueries;

        public CustomersContactsService(IConfiguration config, IContactsQueries contactsQueries)
        {
            _configuration = config;
            _contactsQueries = contactsQueries;
        }

        public async Task<contact> AddContact(int companyId, ContactModel data)
        {
            return await _contactsQueries.AddContact(companyId, data);
        }

        public async Task<contact> UpdateContact(int companyId, ContactModel data)
        {
            return await _contactsQueries.UpdateContact(companyId, data);
        }


        public async Task DeleteContact(int companyId, int id)
        {
            await _contactsQueries.Deletecontact(companyId, id);
        }

        public async Task<List<ContactModel>> GetContacts(int companyId)
        {
            return await _contactsQueries.GetContacts(companyId);
        }

        public async Task<customer> AddCustomer(IdNameModel data, int companyId)
        {
            customer newRec = await _contactsQueries.AddCustomer(data, companyId);
            return newRec;
        }

        public async Task<customer?> UpdateCustomer(IdNameModel data, int companyId)
        {
            customer? updRec = await _contactsQueries.UpdateCustomer(data, companyId);
            return updRec;
        }

        public async Task<List<IdNameModel>> GetCustomersList(int companyId)
        {
            List<IdNameModel> depList = await _contactsQueries.GetCustomersList(companyId);
            return depList;
        }

        public async Task DeleteCustomer(int companyId, int id)
        {
            await _contactsQueries.DeleteCustomer(companyId, id);
        }
    }
}
