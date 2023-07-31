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
    public partial class EmailService : IEmailService
    {
        string _mailFromName;
        string _mailFromAddress;
        string _mailUserName;
        string _mailPassword;

        public EmailService(IConfiguration config)
        {
            _mailFromName = config["GlobalSettings:MailFromName"];
            _mailFromAddress = config["GlobalSettings:GmailPassword"];
            _mailUserName = config["GlobalSettings:GmailUserName"];
            _mailPassword = config["GlobalSettings:GmailPassword"];
        }

        public async Task Send(EmailModel eml)
        {

            var message = new MimeMessage();

            if (eml.From != null)
            {
                if (eml.From.Address == null)
                {
                    message.From.Add(new MailboxAddress(_mailFromName, _mailFromAddress));
                }
                else
                {
                    message.From.Add(new MailboxAddress(eml.From.Name, eml.From.Address));
                }
            }

            if (eml.To != null)
            {
                foreach (var item in eml.To)
                {
                    //message.To.Add(new MailboxAddress(item.Name, item.Address));
                    message.To.Add(new MailboxAddress("Tamir Weiss", "tamir.we@gmail.com"));
                }
            }

            if (eml.Cc != null)
            {
                foreach (var item in eml.Cc)
                {
                    //message.Cc.Add(new MailboxAddress(item.Name, item.Address));
                }
            }

            if (eml.Bcc != null)
            {
                foreach (var item in eml.Bcc)
                {
                    //message.Bcc.Add(new MailboxAddress(item.Name, item.Address));
                }
            }

            message.Subject = eml.Subject;

            //message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            //{
            //    Text = eml.Body
            //};

            var builder = new BodyBuilder { HtmlBody = eml.Body };

            if (eml.Attachments != null)
            {
                foreach (var item in eml.Attachments)
                {
                    builder.Attachments.Add($"{item.name.Replace(' ','_')}.pdf", item.Attachment);
                }
            }

            message.Body = builder.ToMessageBody();

            //builder.Attachments.Add
            //Thread emailThread = new Thread(delegate ()
            //{
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587);


                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(_mailUserName, _mailPassword);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            //});

            //    emailThread.IsBackground = true;
            //    emailThread.Start();
        }
    }
}
