using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using EmailsLibrary.Models;
using System.ComponentModel.Design;
using System.Transactions;

namespace AuthLibrary
{
    public partial class AuthServise
    {
        public async Task Register(string? origin, CompanyAndUserRegisetModel data)
        {
            await _authQueries.DeleteOldRegistrationsKeys();

            //using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //{
                company newCompany = await AddCompany(data.companyName, data.companyDescr);
                string hashPassword = SecretHasher.Hash(data.password);
                user newUser = await RegisterUser(newCompany.id, data.companyName, data.email, hashPassword, data.firstName, data.lastName, UserPermission.Admin, "Registered");

                await GenerateCompanyEmail(newCompany.id);
                AddCompanySatrterData(newCompany.id);
                string key = generateSecretKey();
                await _authQueries.AddUserPasswordReset(key, newUser);
                EmailModel sentEmail = SendRegistrationConfitmationEmail(origin, key, newUser);
                await AddEmailSent(EmailType.Confirm_Registration, newCompany.id, newUser, sentEmail);
                //scope.Complete();
            //}

        }

        private void AddCompanySatrterData(int companyId)
        {
            _authQueries.AddCompanySatrterData(companyId);
        }

        private async Task GenerateCompanyEmail(int companyId)
        {
            string mailFromAddress = _configuration["GlobalSettings:MailFromAddress"];
            string[] mailFromParts = mailFromAddress.Split("@");
            company_cvs_email? companyEmail;
            string generatedCompanyEmail;

            do
            {
                string randKey = GetUniqueRandomEmailKey();
                generatedCompanyEmail = $"{mailFromParts[0]}+{randKey}@{mailFromParts[1]}";
                companyEmail = await _authQueries.GetCompanyEmail(generatedCompanyEmail);
            }
            while (companyEmail != null);

            await _authQueries.AddCompanyCvsEmail(companyId, generatedCompanyEmail);

        }


        private string GetUniqueRandomEmailKey()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[11];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var finalString = new String(stringChars);
            return finalString;
        }

        private async Task<emails_sent> AddEmailSent(EmailType emailType, int companyId, user user, EmailModel sentEmail)
        {
            return await _emailQueries.AddEmailSent(user.id, companyId, emailType, user.email, sentEmail.From.Address, sentEmail.Subject, sentEmail.Body);
        }

        private async Task<user> RegisterUser(int companyId, string companyName, string email, string password, string firstName, string lastName, UserPermission permission, string log)
        {
            user user = await _authQueries.RegisterUser(companyId, email, password, firstName, lastName, UserActiveStatus.Waite_Complete_Registration, permission, log);
            return user;
        }

        private EmailModel SendRegistrationConfitmationEmail(string? origin, string key, user user)
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

        private async Task<company> AddCompany(string companyName, string? companyDescr)
        {
            var company = await _authQueries.AddCompany(companyName, companyDescr, CompanyActiveStatus.Waite_Complete_Registration);
            return company;
        }
    }
}