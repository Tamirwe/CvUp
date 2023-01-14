using Database.models;
using DataModelsLibrary.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Queries
{
    public class EmailQueries : IEmailQueries
    {
        public EmailQueries()
        {
        }

        public emails_sent AddNewEmailSent(int userId, EmailType emailType, string toAddress, string fromAddress, string subject, string body)
        {
            using (var dbContext = new cvup00001Context())
            {
                var email = new emails_sent
                {
                    user_id = userId,
                    sent_date = DateTime.Now,
                    email_type = emailType.ToString(),
                    to_address = toAddress,
                    from_address = fromAddress,
                    subject = subject,
                    body = body
                };

                dbContext.emails_sents.Add(email);
                dbContext.SaveChanges();
                return email;
            }
        }
    }
}
