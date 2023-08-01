using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;

namespace AuthLibrary
{
    public interface IAuthServise
    {
        Task<user?> Login(UserLoginModel data);
        Task<List<IdNameModel>> UserCompanies(string email);
        Task<UserStatusModel> ForgotPassword(string origin, string email, int? companyId);
        Task AddCompanyAndFirstUser(string? origin, CompanyAndUserRegisetModel data);
        string GenerateRefreshToken();
        Task<TokenModel> GenerateAccessToken(user authenticateUser, bool isRemember);
        Task ResendRegistrationEmail(string? origin, UserModel data, int companyId);
        Task<TokenModel?> RefreshToken(string token, string refreshToken);
        Task RevokeToken(int userId);
        Task<user?> CompleteRegistration(UserLoginModel data);
        Task<user?> PasswordReset(UserLoginModel data);
        Task<bool> CheckUserDuplicate(CompanyAndUserRegisetModel data);
        Task<bool> CheckCompanyUserDuplicate(UserModel data, int companyId);
        Task AddInterviewer(InterviewerModel data, int companyId);
        Task UpdateInterviewer(InterviewerModel data, int companyId);
        Task<List<InterviewerModel>> GetInterviewersList(int companyId);
        Task<List<UserModel>> GetUsers(int companyId);
        Task AddCompanyUser(string? origin, UserModel data, int companyId);
        Task UpdateCompanyUser(UserModel data, int companyId);
        Task DeleteUser(int companyId, int id);
        Task ActivateCompanyUser(string? origin, int companyId, UserModel data);
        Task DactivateCompanyUser( int companyId, UserModel data);
        Task<UserModel?> GetUser(int companyId, int userId);
    }
}