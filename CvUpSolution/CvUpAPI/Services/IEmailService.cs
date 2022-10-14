namespace CvUpAPI.Services
{
    public interface IEmailService
    {
        void SendEmail(string email);
        void ReadEmails();
    }
}