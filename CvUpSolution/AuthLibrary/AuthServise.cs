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

        public user? ForgotPassword(string origin, string email, int? companyId, out UserAuthStatus status)
        {
            List<user> users = _authQueries.getUsers(email, companyId);

            if (users.Count == 0)
            {
                status = UserAuthStatus.not_registered;
                return null;
            }
            else if (users.Count == 1)
            {
                status = UserAuthStatus.Authenticated;
                string key = generateSecretKey();
                _authQueries.addUserPasswordReset(key, users[0]);
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
            return _authQueries.getUserCompanies(email);
        }
       
    }
}
