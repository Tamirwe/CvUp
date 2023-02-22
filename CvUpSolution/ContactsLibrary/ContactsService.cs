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

        public async Task<folder> AddContact( int companyId, FolderModel data)
        {
            return await _contactsQueries.AddContact(data);
        }

        public async Task DeleteContact(int companyId, int id)
        {
            await _contactsQueries.Deletecontact(companyId,id);
        }

        public async Task<List<FolderModel>> GetContacts(int companyId, int id)
        {
            return await _contactsQueries.GetContacts(companyId, id);
        }
    }
}
