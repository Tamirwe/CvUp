using EmailsLibrary.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailsLibrary
{
    public class EmailSendService : IEmailSendService
    {
        private IConfigurationSection _gmailSettings;

        public EmailSendService(IConfiguration config)
        {
            _gmailSettings = config.GetRequiredSection("GmailSettings");

        }

        public void Send(SendEmailModel eml)
        {
            var message = new MimeMessage();

            if (eml.From.Address == null)
            {
                message.From.Add(new MailboxAddress(_gmailSettings.GetSection("fromName").Value, _gmailSettings.GetSection("fromAddress").Value));
            }
            else
            {
                message.From.Add(new MailboxAddress(eml.From.Name, eml.From.Address));
            }

            foreach (var item in eml.To)
            {
                message.To.Add(new MailboxAddress(item.Name, item.Address));
            }
            foreach (var item in eml.Cc)
            {
                message.Cc.Add(new MailboxAddress(item.Name, item.Address));
            }
            foreach (var item in eml.Bcc)
            {
                message.Bcc.Add(new MailboxAddress(item.Name, item.Address));
            }


            message.Subject = eml.Subject;

            message.Body = new TextPart("plain")
            {
                Text = eml.Body
            };

            Thread emailThread = new Thread(delegate ()
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587);


                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(_gmailSettings.GetSection("userName").Value, _gmailSettings.GetSection("password").Value);

                    client.Send(message);
                    client.Disconnect(true);
                }
            });

            emailThread.IsBackground = true;
            emailThread.Start();
        }
    }
}
