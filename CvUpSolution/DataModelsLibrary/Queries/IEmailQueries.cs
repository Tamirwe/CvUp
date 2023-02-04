using Database.models;
using DataModelsLibrary.Enums;
using System.ComponentModel.Design;

namespace DataModelsLibrary.Queries
{
    public interface IEmailQueries
    {
        public emails_sent AddNewEmailSent(int userId, int companyId, EmailType emailType, string toAddress, string fromAddress, string subject, string body);

    }
}