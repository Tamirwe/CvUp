using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;

namespace ServicesLibrary.UserLogin
{
    public class UserLoginServise: IUserLoginServise
    {
        private ILoginQueries _loginQueries;
        public UserLoginServise(ILoginQueries loginQueries)
        {
            _loginQueries = loginQueries;
        }

        public user? Login(UserLoginModel data, out UserLoginStatus status)
        {
            var users = _loginQueries.getUsersByEmailPassword(data.email,data.password);

            if (users.Count == 0)
            {
                status = UserLoginStatus.not_registered;
                return null;
            }
            else if (users.Count == 1)
            {
                status = UserLoginStatus.Authenticated;
                return users[0];
            }
            else
            {
                status = UserLoginStatus.more_then_one_company_per_email;
                return null;
            }
        }

       
    }
}
