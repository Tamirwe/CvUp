using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using EmailsLibrary.Models;
using System.Transactions;

namespace AuthLibrary
{
    public partial class Auth
    {
        public void Register(string origin, CompanyAndUserRegisetModel data)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                company newCompany = AddNewCompany(data.companyName, data.companyDescr);
                user newUser = AddNewUser(newCompany.id, data.companyName, data.email, data.password, data.firstName, data.lastName, UsersRole.Admin, "Registered");
                string key = generateSecretKey();
                _authQueries.addUserPasswordReset(key, newUser);
                EmailModel sentEmail = SendRegistrationConfitmationEmail(origin, key, newUser);
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
                To = new List<EmailAddress> { new EmailAddress { Name = string.Format("{0} {1}", user.first_name, user.last_name), Address = user.email } },
                Subject = "Complete Registration",
                Body = _emailService.RegistrationEmailBody(origin, key)
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
