using EmailsLibrary;

namespace CvUpAPI.Services
{

    public class EmailService : IEmailService
    {
     

        public EmailService()
        {

        }

        public void SendEmail(string email)
        {
            Console.WriteLine(email);
        }

        public void ReadEmails()
        {
        }
    }
}
