using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using EmailsLibrary;
using EmailsLibrary.Models;
using Microsoft.Extensions.Configuration;
using System.Transactions;

namespace AuthLibrary
{
    public partial class AuthServise : IAuthServise
    {
        private IAuthQueries _authQueries;
        private IEmailService _emailService;
        private IEmailQueries _emailQueries;
        public IConfiguration _configuration;

        public AuthServise(IAuthQueries authQueries, IEmailService emailService, IEmailQueries emailQueries, IConfiguration config)
        {
            _authQueries = authQueries;
            _emailService = emailService;
            _emailQueries = emailQueries;
            _configuration = config;
        }

        public bool CheckDuplicateUserPassword(CompanyAndUserRegisetModel data)
        {
            List<user> usersList = _authQueries.GetUsersByEmail(data.email);

            if (usersList.Count > 0)
            {
                foreach (var usr in usersList)
                {
                    bool isVerify = SecretHasher.Verify(data.password, usr.passwaord ?? "");

                    if (isVerify)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public user? PasswordReset(UserLoginModel data)
        {
            registeration_key? rKey = _authQueries.GetRegistrationKey(data.key);

            if (rKey is not null)
            {
                var user = _authQueries.GetUser(rKey.user_id);

                if (user is not null)
                {
                    user.passwaord = SecretHasher.Hash(data.password);
                    _authQueries.UpdateUser(user);
                    return user;
                }
            }

            return null;
        }

        public user? ForgotPassword(string origin, string email, int? companyId, out UserAuthStatus status)
        {
            List<user> users = _authQueries.GetUsers(email, companyId);

            if (users.Count == 0)
            {
                status = UserAuthStatus.not_registered;
                return null;
            }
            else if (users.Count == 1)
            {
                status = UserAuthStatus.Authenticated;
                string key = generateSecretKey();
                _authQueries.AddUserPasswordReset(key, users[0]);
                SendResetPasswordEmail(origin, key, users[0]);
                return users[0];
            }
            else
            {
                status = UserAuthStatus.more_then_one_company_per_email;
                return null;
            }
        }

        private string generateSecretKey()
        {
            var guid = Guid.NewGuid().ToString();
            return guid;
        }

        private EmailModel SendResetPasswordEmail(string origin, string key, user user)
        {
            var email = new EmailModel
            {
                To = new List<EmailAddress> { new EmailAddress { Name = string.Format("{0} {1}", user.first_name, user.last_name), Address = user.email } },
                Subject = "Reset Password",
                Body = _emailService.ResetPasswordEmailBody(origin, key)
            };

            _emailService.Send(email);
            return email;
        }

        public List<IdNameModel> UserCompanies(string email)
        {
            return _authQueries.GetUserCompanies(email);
        }
       
    }
}
