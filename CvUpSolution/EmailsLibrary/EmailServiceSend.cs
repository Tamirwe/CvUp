﻿using EmailsLibrary.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailsLibrary
{
    public partial class EmailService : IEmailService
    {
       

        public async Task Send(EmailModel eml)
        {

            var message = new MimeMessage();

            if (eml.From != null && eml.From.Address != null)
            {
                message.From.Add(new MailboxAddress(eml.From.Name, eml.From.Address));
            }

            if (eml.To != null)
            {
                foreach (var item in eml.To)
                {
                    message.To.Add(new MailboxAddress(item.Name, item.Address));
                    //message.To.Add(new MailboxAddress("Tamir Weiss", "tamir.we@gmail.com"));
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

            var builder = new BodyBuilder { HtmlBody = eml.Body };
       
            if (eml.Attachments != null)
            {
                foreach (var item in eml.Attachments)
                {
                    builder.Attachments.Add($"{item.name.Replace(' ','_')}.pdf", item.Attachment);
                }
            }

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587);


                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(eml.MailSenderUserName, eml.MailSenderPassword);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            //});

            //    emailThread.IsBackground = true;
            //    emailThread.Start();
        }
    }
}
