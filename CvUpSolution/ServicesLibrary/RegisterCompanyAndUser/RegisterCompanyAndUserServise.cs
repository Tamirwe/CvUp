
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using Database.models;
using DataModelsLibrary.Enums;
using EmailsLibrary;
using EmailsLibrary.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System.Transactions;

namespace ServicesLibrary.RegisterCompanyAndUser
{
    public class RegisterCompanyAndUserServise : IRegisterCompanyAndUserServise
    {
        private IRegistrationQueries _registrationQueries;
        private IEmailSendService _emailService;
        private IEmailQueries _emailQueries;

        public RegisterCompanyAndUserServise(IRegistrationQueries registrationQueries, IEmailSendService emailService, IEmailQueries emailQueries)
        {
            _registrationQueries = registrationQueries;
            _emailService = emailService;
            _emailQueries = emailQueries;
        }

        public void Register(CompanyAndUserRegisetModel data)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                company newCompany = AddNewCompany(data.companyName, data.companyDescr);
                user newUser = AddNewUser(newCompany.id, data.companyName, data.email, data.password, data.firstName, data.lastName, UsersRole.Admin, "Registered");
                EmailModel sentEmail = SendRegistrationConfitmationEmail(newUser);
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
            user user = _registrationQueries.AddNewUser(companyId, email, password, firstName, lastName, UserActivateStatus.REGISTRATION_NOT_COMPLETED, role, log);
            return user;
        }

        private EmailModel SendRegistrationConfitmationEmail(user user)
        {
            var email = new EmailModel
            {
                To = new List<EmailAddress> { new EmailAddress { Name = String.Format("{0} {1}", user.first_name, user.last_name), Address = user.email } },
                Subject = "Complete Registration",
                Body = "follow this link"
            };

            _emailService.Send(email);
            return email;
        }

        private company AddNewCompany(string companyName, string? companyDescr)
        {
            var company = _registrationQueries.AddNewCompany(companyName, companyDescr, CompanyActivateStatus.WAITE_FOR_FIRST_USER_TO_COMPLETE_REGISTRATION);
            return company;
        }

    }
}
