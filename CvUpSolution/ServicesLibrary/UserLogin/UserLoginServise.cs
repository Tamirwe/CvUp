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

        public user? Login(UserLoginModel data, out UserAuthStatus status)
        {
            var users = _loginQueries.getUsersByEmailPassword(data.email,data.password);

            if (users.Count == 0)
            {
                status = UserAuthStatus.not_registered;
                return null;
            }
            else if (users.Count == 1)
            {
                status = UserAuthStatus.Authenticated;
                return users[0];
            }
            else
            {
                status = UserAuthStatus.more_then_one_company_per_email;
                return null;
            }
        }

        public user? ForgotPassword(string email, out UserAuthStatus status)
        {
            var users = _loginQueries.getUsersByEmail(email);

            if (users.Count == 0)
            {
                status = UserAuthStatus.not_registered;
                return null;
            }
            else if (users.Count == 1)
            {
                status = UserAuthStatus.Authenticated;
                return users[0];
            }
            else
            {
                status = UserAuthStatus.more_then_one_company_per_email;
                return null;
            }
        }

        public List<IdNameModel> UserCompanies(string email)
        {
            return _loginQueries.getUserCompanies(email);
        }


    }
}
