using Database.models;
using DataModelsLibrary.Models;

namespace ContactsLibrary
{
    public interface IContactsService
    {
        Task<folder> AddContact( int companyId, FolderModel data);
        Task DeleteContact(int companyId, int id);
        Task<List<FolderModel>> GetContacts(int companyId);
    }
}
