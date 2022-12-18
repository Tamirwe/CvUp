using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using EmailsLibrary.Models;
using System.Transactions;

namespace AuthLibrary
{
    public partial class AuthServise
    {
        public void Register(string origin, CompanyAndUserRegisetModel data)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                company newCompany = AddNewCompany(data.companyName, data.companyDescr);
                string hashPassword = SecretHasher.Hash(data.password);
                user newUser = AddNewUser(newCompany.id, data.companyName, data.email, hashPassword, data.firstName, data.lastName, UserPermission.Admin, "Registered");
                newCompany.key_email = "ui" + newUser.id + "ci" + newCompany.id;
                newCompany.cvs_email = "cvup.files+" + newCompany.key_email + "@gmail.com";
                _authQueries.updateCompany(newCompany);
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

        private user AddNewUser(int companyId, string companyName, string email, string password, string firstName, string lastName, UserPermission permission, string log)
        {
            user user = _authQueries.AddNewUser(companyId, email, password, firstName, lastName, UserActivateStatus.REGISTRATION_NOT_COMPLETED, permission, log);
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