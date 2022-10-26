using EmailsLibrary.Models;

namespace EmailsLibrary
{
    public interface IEmailSendService
    {
        public void Send(EmailModel eml);
    }
}