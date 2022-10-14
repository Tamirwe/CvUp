using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;

namespace ServicesLibrary.UserLogin
{
    public interface IUserLoginServise
    {
        public user? Login(UserLoginModel dataa, out UserLoginStatus status);
    }
}