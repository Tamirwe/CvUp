using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using EmailsLibrary;
using EmailsLibrary.Models;
using System.Transactions;

namespace ServicesLibrary.Authentication
{
    public class AuthServise: IAuthServise
    {
        private IAuthQueries _authQueries;
        private IEmailService _emailService;
        private IEmailQueries _emailQueries;
        public AuthServise(IAuthQueries authQueries, IEmailService emailService, IEmailQueries emailQueries)
        {
            _authQueries = authQueries;
            _emailService = emailService;
            _emailQueries = emailQueries;
        }

        public user? Login(UserLoginModel data, out UserAuthStatus status)
        {
            if (data.key != null)
            {

            }

            var users = _authQueries.getUsersByEmailPassword(data.email,data.password);


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
                SendResetPasswordEmail(origin, key,users[0]);
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
                To = new List<EmailAddress> { new EmailAddress { Name = String.Format("{0} {1}", user.first_name, user.last_name), Address = user.email } },
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

        public void Register(string origin, CompanyAndUserRegisetModel data)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                company newCompany = AddNewCompany(data.companyName, data.companyDescr);
                user newUser = AddNewUser(newCompany.id, data.companyName, data.email, data.password, data.firstName, data.lastName, UsersRole.Admin, "Registered");
                string key = generateSecretKey();
                _authQueries.addUserPasswordReset(key, newUser);
                EmailModel sentEmail = SendRegistrationConfitmationEmail(origin, key,newUser);
                AddEmailSent(EmailType.REGISTRATION_CONFIRMATION, newUser, sentEmail);
                scope.Complete();
            }
        }

        private void AddEmailSent(EmailType emailType, user user, EmailModel sentEmail)
        {
            _emailQueries.AddNewEmailSent(user.id, emailType, user.email, sentEmail.From.Address, sentEmail.Subject, sentEmail.Body);
        }

        private user AddNewUser(int companyId, string companyName, string email, string password, string firstName, string lastName, UsersRole role, string log)
        {
            user user = _authQueries.AddNewUser(companyId, email, password, firstName, lastName, UserActivateStatus.REGISTRATION_NOT_COMPLETED, role, log);
            return user;
        }

        private EmailModel SendRegistrationConfitmationEmail(string origin, string key, user user)
        {
            var email = new EmailModel
            {
                To = new List<EmailAddress> { new EmailAddress { Name = String.Format("{0} {1}", user.first_name, user.last_name), Address = user.email } },
                Subject = "Complete Registration",
                Body = _emailService.ResetPasswordEmailBody(origin, key)
            };

            _emailService.Send(email);
            return email;
        }

        private company AddNewCompany(string companyName, string? companyDescr)
        {
            var company = _authQueries.AddNewCompany(companyName, companyDescr, CompanyActivateStatus.WAITE_FOR_FIRST_USER_TO_COMPLETE_REGISTRATION);
            return company;
        }
    }
}
