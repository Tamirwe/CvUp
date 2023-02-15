﻿using Database.models;
using DataModelsLibrary.Enums;

namespace DataModelsLibrary.Queries
{
    public class EmailQueries : IEmailQueries
    {
        public EmailQueries()
        {
        }

        public async Task<emails_sent> AddNewEmailSent(int userId, int companyId, EmailType emailType, string toAddress, string fromAddress, string subject, string body)
        {
            using (var dbContext = new cvup00001Context())
            {
                var email = new emails_sent
                {
                    user_id = userId,
                    company_id = companyId,
                    sent_date = DateTime.Now,
                    email_type = emailType.ToString(),
                    to_address = toAddress,
                    from_address = fromAddress,
                    subject = subject,
                    body = body
                };

                dbContext.emails_sents.Add(email);
                await dbContext.SaveChangesAsync();
                return email;
            }
        }
    }
}
