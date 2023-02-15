using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface IAuthQueries
    {
        public Task<List<user>> GetUsersByEmail(string email);
        public Task<List<IdNameModel>> GetUserCompanies(string email);
        public Task<user?> GetUser(int userId);
        public Task<List<user>> GetUsers(string email, int? companyId);
        public Task<company?> GetCompany(int companyId);
        public Task<company> AddNewCompany(string companyName, string? companyDescr, CompanyActiveStatus status);
        public Task<user> AddNewUser(int companyId, string email, string password, string firstName, string lastName, UserActiveStatus status, UserPermission permission, string log);
        public Task<company> UpdateCompany(company newCompany);
        Task AddUserPasswordReset(string key, user user);
        public Task<registeration_key?> GetRegistrationKey(string key);
        Task ActivateUser(user user);
        Task SaveRefreshToken(string refreshToken, user authenticateUser);
        Task RevokeUserToken(int userId);
        public Task RemoveRegistrationKey(registeration_key rkey);
        public Task UpdateUser(user user);
        public Task AddInterviewer(InterviewerModel data, int companyId);
        public Task UpdateInterviewer(InterviewerModel data, int companyId);
        public Task<List<InterviewerModel>> GetInterviewersList(int companyId);
        public Task DeleteInterviewer(int companyId, int id);
        public Task<company_cvs_email?> GetCompanyEmail(string newEmail);
        public Task AddCompanySatrterData(int companyId);
        public Task AddCompanyCvsEmail(int companyId, string generatedCompanyEmail);
    }
}