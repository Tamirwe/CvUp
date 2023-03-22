using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;

namespace AuthLibrary
{
    public interface IAuthServise
    {
        public Task<user?> Login(UserLoginModel data);
        public Task<List<IdNameModel>> UserCompanies(string email);
        public Task<UserStatusModel> ForgotPassword(string origin, string email, int? companyId);
        Task Register(string? origin, CompanyAndUserRegisetModel data);
        public string GenerateRefreshToken();
        public Task<TokenModel> GenerateAccessToken(user authenticateUser, bool isRemember);
        public Task<TokenModel?> RefreshToken(string token, string refreshToken);
        public Task RevokeToken(int userId);
        public Task<user?> CompleteRegistration(UserLoginModel data);
        public Task<user?> PasswordReset(UserLoginModel data);
        public Task<bool> CheckDuplicateUserPassword(CompanyAndUserRegisetModel data);
        public Task AddInterviewer(InterviewerModel data, int companyId);
        public Task UpdateInterviewer(InterviewerModel data, int companyId);
        public Task<List<InterviewerModel>> GetInterviewersList(int companyId);
        Task<List<UserModel>> GetUsers(int companyId);
        Task AddUserByUser(UserModel data, int companyId);
        Task UpdateUserByUser(UserModel data, int companyId);
        Task DeleteUser(int companyId, int id);
    }
}