using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using Microsoft.Extensions.Configuration;

namespace ContactsLibrary
{
    public class ContactsService : IContactsService
    {
        private IConfiguration _configuration;
        private IContactsQueries _contactsQueries;

        public ContactsService(IConfiguration config, IContactsQueries contactsQueries)
        {
            _configuration = config;
            _contactsQueries = contactsQueries;
        }

        public async Task<contact> AddContact( int companyId, ContactModel data)
        {
            return await _contactsQueries.AddContact( companyId, data);
        }

        public async Task<contact> UpdateContact(int companyId, ContactModel data)
        {
            return await _contactsQueries.UpdateContact(companyId, data);
        }


        public async Task DeleteContact(int companyId, int id)
        {
            await _contactsQueries.Deletecontact(companyId,id);
        }

        public async Task<List<ContactModel>> GetContacts(int companyId)
        {
            return await _contactsQueries.GetContacts(companyId);
        }
    }
}
