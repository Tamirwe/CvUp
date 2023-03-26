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
        public async Task AddCompanyAndFirstUser(string? origin, CompanyAndUserRegisetModel data)
        {
            await _authQueries.DeleteOldRegistrationsKeys();

            //using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //{
                company newCompany = await AddCompany(data.companyName, data.companyDescr);
                string hashPassword = SecretHasher.Hash(data.password);
                user newUser = await _authQueries.AddUser(newCompany.id, data.email, hashPassword, data.firstName, data.lastName, UserActiveStatus.Waite_Complete_Registration, UserPermission.Admin, "Registered");
                await GenerateCompanyEmail(newCompany.id);
                AddCompanySatrterData(newCompany.id);
                string key = generateSecretKey();
                await _authQueries.AddRegistrationKey(key, newUser);
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

        //private async Task<user> RegisterUser(int companyId, string email, string password, string firstName, string lastName, UserPermission permission, string log)
        //{
        //    user user = await _authQueries.RegisterUser(companyId, email, password, firstName, lastName, UserActiveStatus.Waite_Complete_Registration, permission, log);
        //    return user;
        //}

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

        public async Task<List<UserModel>> GetUsers(int companyId)
        {
            List<UserModel> depList = await _authQueries.GetUsers(companyId);
            return depList;
        }

        public async Task ResendRegistrationEmail(string? origin, UserModel data, int companyId)
        {
            user? usr = await _authQueries.GetUser(companyId ,data.email);

            if (usr != null)
            {
                string key = generateSecretKey();
                await _authQueries.AddRegistrationKey(key, usr);
                EmailModel sentEmail = SendRegistrationConfitmationEmail(origin, key, usr);
                await AddEmailSent(EmailType.Confirm_Registration, companyId, usr, sentEmail);
            }
        }

        public async Task DeactivateUser(int companyId, UserModel data)
        {
            user? usr = await _authQueries.GetUser(companyId, data.email);

            if (usr != null)
            {
                await _authQueries.DeactivateUser(usr);
            }
        }

        public async Task AddCompanyUser(string? origin, UserModel data, int companyId)
        {
            user newUser = await _authQueries.AddUser(companyId, data.email, null, data.firstName, data.lastName, UserActiveStatus.Waite_Complete_Registration,data.permissionType, $"addedBy:{data.addedById}-{data.addedByName}");
            string key = generateSecretKey();
            await _authQueries.AddRegistrationKey(key, newUser);
            EmailModel sentEmail = SendRegistrationConfitmationEmail(origin, key, newUser);
            await AddEmailSent(EmailType.Confirm_Registration, companyId, newUser, sentEmail);
        }

        public async Task UpdateCompanyUser(UserModel data, int companyId)
        {
            await _authQueries.UpdateCompanyUser(data, companyId);
        }

        public async Task DeleteUser(int companyId, int id)
        {
            await _authQueries.DeleteUser(companyId, id);
        }
    }
}