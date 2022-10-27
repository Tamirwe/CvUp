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

        public string RegistrationEmailBody()
        {
            string body = string.Empty;
            string htmlFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"EmailTemplates\RegistrationTemplate.html");

            //string sFilePath = Path.GetFullPath(htmlFile);

            using (StreamReader reader = new StreamReader(htmlFile))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{loginLink}", "http://localhost:3000/login");

            return body;
        }

    }
}
