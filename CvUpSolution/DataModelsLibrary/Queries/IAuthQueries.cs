using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface IAuthQueries
    {
        public List<user> GetUsersByEmail(string email);
        public List<IdNameModel> GetUserCompanies(string email);
        public user? GetUser(int userId);
        public List<user> GetUsers(string email, int? companyId);
        public company? GetCompany(int companyId);
        public company AddNewCompany(string companyName, string? companyDescr, CompanyActiveStatus status);
        user AddNewUser(int companyId, string email, string password, string firstName, string lastName, UserActiveStatus status, UserPermission permission, string log);
        company UpdateCompany(company newCompany);
        void AddUserPasswordReset(string key, user user);
        public registeration_key? GetRegistrationKey(string key);
        void ActivateUser(user user);
        void SaveRefreshToken(string refreshToken, user authenticateUser);
        void RevokeUserToken(int userId);
        public void RemoveRegistrationKey(registeration_key rkey);
        public void UpdateUser(user user);
        public void AddInterviewer(InterviewerModel data, int companyId);
        public void UpdateInterviewer(InterviewerModel data, int companyId);
        public List<InterviewerModel> GetInterviewersList(int companyId);
        public void DeleteInterviewer(int companyId, int id);
        public company? GetCompanyByKeyEmail(string uniqueRandomKey);
        public void AddCompanySatrterData(int companyId);
    }
}