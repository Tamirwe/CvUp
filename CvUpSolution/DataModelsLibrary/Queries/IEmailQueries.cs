using Database.models;
using DataModelsLibrary.Enums;

namespace DataModelsLibrary.Queries
{
    public interface IEmailQueries
    {
        public emails_sent AddNewEmailSent(int userId, EmailType emailType, string toAddress, string fromAddress, string subject, string body);

    }
}