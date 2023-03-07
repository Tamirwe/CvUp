using Database.models;
using DataModelsLibrary.Models;

namespace ContactsLibrary
{
    public interface IContactsService
    {
        Task<contact> AddContact( int companyId, ContactModel data);
        Task DeleteContact(int companyId, int id);
        Task<List<ContactModel>> GetContacts(int companyId);
        Task<contact> UpdateContact(int companyId, ContactModel data);
    }
}
