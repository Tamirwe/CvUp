using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using System.Security.Claims;

namespace AuthLibrary
{
    public interface IAuthServise
    {
        public user? Login(UserLoginModel dataa, out UserAuthStatus status);
        public List<IdNameModel> UserCompanies(string email);
        public user? ForgotPassword(string origin, string email, int? companyId, out UserAuthStatus status);
        void Register(string origin, CompanyAndUserRegisetModel data);
        public string GenerateRefreshToken();
        public TokenModel GenerateAccessToken(user authenticateUser, bool isRemember);
        public TokenModel? RefreshToken(string token, string refreshToken);
        void RevokeToken(int userId);
    }
}
