using EmailsLibrary.Models;

namespace EmailsLibrary
{
    public interface IEmailService
    {
        public void Send(EmailModel eml);
        public string RegistrationEmailBody();
    }
}