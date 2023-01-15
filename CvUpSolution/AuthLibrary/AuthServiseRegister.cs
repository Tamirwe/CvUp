using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using EmailsLibrary.Models;
using System.Transactions;

namespace AuthLibrary
{
    public partial class AuthServise
    {
        public void Register(string? origin, CompanyAndUserRegisetModel data)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                company newCompany = AddNewCompany(data.companyName, data.companyDescr);
                string hashPassword = SecretHasher.Hash(data.password);
                user newUser = AddNewUser(newCompany.id, data.companyName, data.email, hashPassword, data.firstName, data.lastName, UserPermission.Admin, "Registered");

                string[] emailKeys = GenerateCompanyEmail(newCompany.id);
                newCompany.key_email = emailKeys[0];
                newCompany.cvs_email = emailKeys[1];
                _authQueries.UpdateCompany(newCompany);
                string key = generateSecretKey();
                _authQueries.AddUserPasswordReset(key, newUser);
                EmailModel sentEmail = SendRegistrationConfitmationEmail(origin, key, newUser);
                AddEmailSent(EmailType.Confirm_Registration, newUser, sentEmail);
                scope.Complete();
            }
        }

        private string[] GenerateCompanyEmail(int companyId)
        {
            string uniqueRandomKey = "";

            for (int i = 0; i < 5; i++)
            {
                string randKey = GetUniqueRandomEmailKey(companyId);
                var company = _authQueries.GetCompanyByKeyEmail(uniqueRandomKey);

                if (company is null)
                {
                    uniqueRandomKey = randKey;
                    break;
                }
            }

            if (uniqueRandomKey == "")
            {
                throw new InvalidOperationException("server error"); ;
            }

            string mailFromAddress = _configuration["GlobalSettings:MailFromAddress"];
            string[] mailFromParts = mailFromAddress.Split("@");
            return new string[] { uniqueRandomKey, $"{mailFromParts[0]}+{uniqueRandomKey}{companyId}@{mailFromParts[1]}" };
        }


        private string GetUniqueRandomEmailKey(int companyId)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[7];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var finalString = new String(stringChars);
            return finalString;
        }

        private void AddEmailSent(EmailType emailType, user user, EmailModel sentEmail)
        {
            _emailQueries.AddNewEmailSent(user.id, emailType, user.email, sentEmail.From.Address, sentEmail.Subject, sentEmail.Body);
        }

        private user AddNewUser(int companyId, string companyName, string email, string password, string firstName, string lastName, UserPermission permission, string log)
        {
            user user = _authQueries.AddNewUser(companyId, email, password, firstName, lastName, UserActiveStatus.Waite_Complete_Registration, permission, log);
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

        private company AddNewCompany(string companyName, string? companyDescr)
        {
            var company = _authQueries.AddNewCompany(companyName, companyDescr, CompanyActiveStatus.Waite_Complete_Registration);
            return company;
        }


    }
}