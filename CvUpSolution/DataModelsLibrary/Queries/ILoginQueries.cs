using Database.models;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface ILoginQueries
    {
        public List<user> getUsersByEmailPassword(string email, string password);
        public List<IdNameModel> getUserCompanies(string email);
        public List<user> getUsers(string email, int? companyId);
    }
}