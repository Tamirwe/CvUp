using Database.models;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface IContactsQueries
    {
        Task<contact> AddContact(int companyId, ContactModel data);
        Task Deletecontact(int companyId, int id);
        Task<List<ContactModel>> GetContacts(int companyId);
        Task<contact> UpdateContact(int companyId, ContactModel data);
    }
}
