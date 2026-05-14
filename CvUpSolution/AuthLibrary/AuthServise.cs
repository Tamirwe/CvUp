using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using dotenv.net;
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

        string _secretKey = "";
        string _refreshTokenHoursExpiration = "";
        string _issuer = "";
        string _audience = "";
        string _mailFromAddress = "";


        public AuthServise(IAuthQueries authQueries, IEmailService emailService, IEmailQueries emailQueries)
        {
            DotEnv.Load();
            var envVars = DotEnv.Read();
            _secretKey = envVars["JWT_SECRET_KEY"];
            _refreshTokenHoursExpiration = envVars["JWT_REFRESH_TOKE_EXPIRATION_HOURS"];
            _issuer = envVars["JWT_ISSUER"];
            _audience = envVars["JWT_AUDIENCE"];
            _mailFromAddress = envVars["IMPORT_GMAIL_ADDRESS"];

            _authQueries = authQueries;
            _emailService = emailService;
            _emailQueries = emailQueries;


        }

       

        public async Task<bool> CheckUserDuplicate(CompanyAndUserRegisetModel data)
        {
            List<user> usersList = await _authQueries.GetUsersByEmail(data.email);

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

        public async Task<bool> CheckCompanyUserDuplicate(UserModel data, int companyId)
        {
            List<user> usersList = await _authQueries.GetUsersByCompanyEmail(data.email, companyId);

            if (usersList.Count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<user?> PasswordReset(UserLoginModel data)
        {
            registeration_key? rKey = await _authQueries.GetRegistrationKey(data.key);

            if (rKey is not null)
            {
                var user = await _authQueries.GetUser(rKey.user_id);

                if (user is not null)
                {
                    user.passwaord = SecretHasher.Hash(data.password);
                    await _authQueries.UpdateUser(user);
                    return user;
                }
            }

            return null;
        }

        public async Task<UserStatusModel?> ForgotPassword(string origin, string email, int? companyId)
        {
            List<user> users = await _authQueries.GetUsers(email, companyId);

            if (users.Count == 0)
            {
                UserAuthStatus status = UserAuthStatus.not_registered;
                return new UserStatusModel { status = status, user = null };
            }
            else if (users.Count == 1)
            {
                UserAuthStatus status = UserAuthStatus.Authenticated;
                string key = generateSecretKey();
                await _authQueries.AddRegistrationKey(key, users[0]);
                await SendResetPasswordEmail(origin, key, users[0]);
                return new UserStatusModel { status = status, user = users[0] };

            }
            else
            {
                UserAuthStatus status = UserAuthStatus.more_then_one_company_per_email;
                return new UserStatusModel { status = status, user = null };
            }
        }

        private string generateSecretKey()
        {
            var guid = Guid.NewGuid().ToString();
            return guid;
        }

        private async Task<EmailModel> SendResetPasswordEmail(string origin, string key, user user)
        {
            DotEnv.Load();
            var envVars = DotEnv.Read();
            var _systemGmailUserName = envVars["SYSTEM_GMAIL_USER_NAME"];
            var _systemGmailAddress = envVars["SYSTEM_GMAIL_ADDRESS"];
            var _systemGmailPassword = envVars["SYSTEM_GMAIL_PASSWORD"];
            var _sSystemMailFromName = envVars["SYSTEM_GMAIL_FROM_NAME"];


            var email = new EmailModel
            {
                To = new List<EmailAddress> { new EmailAddress { Name = string.Format("{0} {1}", user.first_name, user.last_name), Address = user.email } },
                Subject = "Reset Password",
                Body = _emailService.ResetPasswordEmailBody(origin, key),
                From = new EmailAddress { Address = _systemGmailAddress, Name = _sSystemMailFromName },
                MailSenderUserName = _systemGmailUserName,
                MailSenderPassword = _systemGmailPassword,
            };

            await _emailService.Send(email);
            return email;
        }

        public Task<List<IdNameModel>> UserCompanies(string email)
        {
            throw new NotImplementedException();
        }
    }
}
