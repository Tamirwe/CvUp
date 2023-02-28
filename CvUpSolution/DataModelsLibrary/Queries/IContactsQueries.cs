using Database.models;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface IContactsQueries
    {
        Task<folder> AddContact(int companyId, FolderModel data);
        Task Deletecontact(int companyId, int id);
        Task<List<FolderModel>> GetContacts(int companyId);
    }
}
