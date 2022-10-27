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

        public string RegistrationEmailBody(string origin, string key)
        {
            string body = string.Empty;
            string htmlFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"EmailTemplates\RegistrationCompleteTemplate.html");

            //string sFilePath = Path.GetFullPath(htmlFile);

            using (StreamReader reader = new StreamReader(htmlFile))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{loginLink}", origin + "/login?sk=" + key);

            return body;
        }

        public string ResetPasswordEmailBody(string origin,string key)
        {
            string body = string.Empty;
            string htmlFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"EmailTemplates\PasswordResetTemplate.html");

            //string sFilePath = Path.GetFullPath(htmlFile);

            using (StreamReader reader = new StreamReader(htmlFile))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{loginLink}", origin + "/login?sk=" + key);

            return body;
        }
    }
}
