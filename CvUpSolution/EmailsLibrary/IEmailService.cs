using EmailsLibrary.Models;

namespace EmailsLibrary
{
    public interface IEmailService
    {
        public Task Send(EmailModel eml);
        public string RegistrationEmailBody(string? origin, string key);
        string ResetPasswordEmailBody(string origin, string key);
    }
}