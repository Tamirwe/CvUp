using Database.models;

namespace DataModelsLibrary.Queries
{
    public interface ILoginQueries
    {
        public List<user> getUsersByEmailPassword(string email, string password);
    }
}