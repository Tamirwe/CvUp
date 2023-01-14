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
        public company AddNewCompany(string companyName, string? companyDescr, CompanyActiveStatus status);
        user AddNewUser(int companyId, string email, string password, string firstName, string lastName, UserActiveStatus status, UserPermission permission, string log);
        company updateCompany(company newCompany);
        void addUserPasswordReset(string key, user user);
        public registeration_key? getRegistrationKey(string key);
        void activateUser(user user);
        void SaveRefreshToken(string refreshToken, user authenticateUser);
        void RevokeUserToken(int userId);
        public void removeRegistrationKey(registeration_key rkey);
        public void UpdateUser(user user);
        public void AddInterviewer(InterviewerModel data, int companyId);
        public void UpdateInterviewer(InterviewerModel data, int companyId);
        public List<InterviewerModel> GetInterviewersList(int companyId);
        public void DeleteInterviewer(int companyId, int id);
    }
}