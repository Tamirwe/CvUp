using Database.models;
using DataModelsLibrary.Enums;

namespace DataModelsLibrary.Queries
{
    public interface IEmailQueries
    {
        public Task<auth_out_email> AddAuthOutEmail(int userId, int companyId, EmailType emailType, string toAddress, string fromAddress, string subject, string body);
    }
}