using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;

namespace AuthLibrary
{
    public interface IAuth
    {
        public user? Login(UserLoginModel dataa, out UserAuthStatus status);
        public List<IdNameModel> UserCompanies(string email);
        public user? ForgotPassword(string origin, string email, int? companyId, out UserAuthStatus status);

        void Register(string origin, CompanyAndUserRegisetModel data);
    }
}