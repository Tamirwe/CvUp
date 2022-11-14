using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface IAuthQueries
    {
        public List<user> getUsersByEmail(string email);
        public List<IdNameModel> getUserCompanies(string email);
        public user? getUser(int userId);
        public List<user> getUsers(string email, int? companyId);
        public company AddNewCompany(string companyName, string? companyDescr, CompanyActivateStatus status);
        user AddNewUser(int companyId, string email, string password, string firstName, string lastName, UserActivateStatus status, UsersRole role, string log);
        company updateCompany(company newCompany);
        void addUserPasswordReset(string key, user user);
        public registeration_key? getRegistrationKey(string key);
        void activateUser(user user);
        void SaveRefreshToken(string refreshToken, user authenticateUser);
        void RevokeUserToken(int userId);
        public void removeRegistrationKey(registeration_key rkey);
        public void UpdateUser(user user);
        


    }
}