using Database.models;
using DataModelsLibrary.Enums;

namespace DataModelsLibrary.Queries
{
    public interface IEmailQueries
    {
        public Task<emails_sent> AddNewEmailSent(int userId, int companyId, EmailType emailType, string toAddress, string fromAddress, string subject, string body);
    }
}