using Database.models;
using DataModelsLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Queries
{
    public class EmailQueries: IEmailQueries
    {
        private cvup00001Context dbContext;

        public EmailQueries()
        {
            dbContext = new cvup00001Context();
        }

        public emails_sent AddNewEmailSent(int userId, EmailType emailType, string toAddress, string fromAddress, string subject, string body)
        {
            var email = new emails_sent
            {
                user_id = userId,
                sent_date = DateTime.Now,
                email_type = (int)emailType,
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
