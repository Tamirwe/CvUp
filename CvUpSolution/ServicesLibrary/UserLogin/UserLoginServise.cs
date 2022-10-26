using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using EmailsLibrary;
using EmailsLibrary.Models;
using System.Security.Cryptography;

namespace ServicesLibrary.UserLogin
{
    public class UserLoginServise: IUserLoginServise
    {
        private ILoginQueries _loginQueries;
        private IEmailSendService _emailService;
        private IEmailQueries _emailQueries;
        public UserLoginServise(ILoginQueries loginQueries, IEmailSendService emailService, IEmailQueries emailQueries)
        {
            _loginQueries = loginQueries;
            _emailService = emailService;
            _emailQueries = emailQueries;
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

        public user? ForgotPassword(string email, int? companyId, out UserAuthStatus status)
        {
            List<user> users = _loginQueries.getUsers(email, companyId);

            if (users.Count == 0)
            {
                status = UserAuthStatus.not_registered;
                return null;
            }
            else if (users.Count == 1)
            {
                status = UserAuthStatus.Authenticated;
                string key = generateSecretKey();
                
                SendForgotPasswordEmail(users[0]);
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
            var hmac = new HMACSHA256();
            var key = Convert.ToBase64String(hmac.Key);
            return key;
        }

        private EmailModel SendForgotPasswordEmail(user user)
        {
            var email = new EmailModel
            {
                To = new List<EmailAddress> { new EmailAddress { Name = String.Format("{0} {1}", user.first_name, user.last_name), Address = user.email } },
                Subject = "Reset Password",
                Body = "follow this link"
            };

            _emailService.Send(email);
            return email;
        }

        public List<IdNameModel> UserCompanies(string email)
        {
            return _loginQueries.getUserCompanies(email);
        }


    }
}
