using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;

namespace AuthLibrary
{
    public partial class AuthServise
    {
        public user? Login(UserLoginModel data, out UserAuthStatus status)
        {
            var users = _authQueries.getUsersByEmailPassword(data.email, data.password);


            if (users.Count == 1)
            {
                if (data.key != "")
                {
                    var loginVerification = _authQueries.getloginVerification(data.key);

                    if (loginVerification == null)
                    {
                        status = UserAuthStatus.not_registered;
                        return null;
                    }
                    else if (loginVerification.email != users[0].email || loginVerification.user_id != users[0].id)
                    {
                        status = UserAuthStatus.not_registered;
                        return null;
                    }

                    if (users[0].activate_status_id != (int)UserActivateStatus.ACTIVE)
                    {
                        _authQueries.activateUser(users[0]);
                    }

                    status = UserAuthStatus.Authenticated;
                    return users[0];
                }
                else
                {
                    if (users[0].activate_status_id != (int)UserActivateStatus.ACTIVE)
                    {
                        status = UserAuthStatus.not_registered;
                        return null;
                    }

                    status = UserAuthStatus.Authenticated;
                    return users[0];
                }
            }
            else if (users.Count == 0)
            {
                status = UserAuthStatus.not_registered;
                return null;
            }
            else
            {
                status = UserAuthStatus.more_then_one_company_per_email;
                return null;
            }
        }
    }
}
