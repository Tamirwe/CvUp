using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using System.Security.Claims;

namespace AuthLibrary
{
    public interface IAuthServise
    {
        public user? Login(UserLoginModel dataa);
        public List<IdNameModel> UserCompanies(string email);
        public user? ForgotPassword(string origin, string email, int? companyId, out UserAuthStatus status);
        void Register(string origin, CompanyAndUserRegisetModel data);
        public string GenerateRefreshToken();
        public TokenModel GenerateAccessToken(user authenticateUser, bool isRemember);
        public TokenModel? RefreshToken(string token, string refreshToken);
        public void RevokeToken(int userId);
        public user? CompleteRegistration(UserLoginModel data);
        public user? PasswordReset(UserLoginModel data);
        public bool CheckDuplicateUserPassword(CompanyAndUserRegisetModel data);
        public user AddInterviewer(InterviewerModel data, int companyId);
        public user UpdateInterviewer(InterviewerModel data, int companyId);
        public List<InterviewerModel> GetInterviewers(int companyId);
        public void DeleteInterviewer(int companyId, int id);
    }
}
