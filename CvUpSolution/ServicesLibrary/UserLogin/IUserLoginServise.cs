using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;

namespace ServicesLibrary.UserLogin
{
    public interface IUserLoginServise
    {
        public user? Login(UserLoginModel dataa, out UserAuthStatus status);
        public List<IdNameModel> UserCompanies(string email);
        public user? ForgotPassword(string email,int? companyId, out UserAuthStatus status);

    }
}