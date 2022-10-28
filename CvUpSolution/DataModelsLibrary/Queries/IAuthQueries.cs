using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface IAuthQueries
    {
        public List<user> getUsersByEmailPassword(string email, string password);
        public List<IdNameModel> getUserCompanies(string email);
        public List<user> getUsers(string email, int? companyId);
        public company AddNewCompany(string companyName, string? companyDescr, CompanyActivateStatus status);
        user AddNewUser(int companyId, string email, string password, string firstName, string lastName, UserActivateStatus status, UsersRole role, string log);
        company updateCompany(company newCompany);
        void addUserPasswordReset(string key, user user);
        login_verification getloginVerification(string key);
        void activateUser(user user);
    }
}