﻿using EmailsLibrary.Models;
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
            _mailFromAddress = config["GlobalSettings:MailFromAddress"];
            _mailUserName = config["GlobalSettings:gmailUserName"];
            _mailPassword = config["GlobalSettings:gmailPassword"];
        }

        public async Task Send(EmailModel eml)
        {
            var message = new MimeMessage();

            if (eml.From.Address == null)
            {
                message.From.Add(new MailboxAddress(_mailFromName, _mailFromAddress));
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

            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = eml.Body
            };

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
