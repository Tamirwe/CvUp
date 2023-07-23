using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailsLibrary.Models
{
    public class EmailAddress
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

    public class EmailModel
    {
        public List<EmailAddress>? To { get; set; } = new List<EmailAddress>();
        public List<EmailAddress>? Cc { get; set; } = new List<EmailAddress>();
        public List<EmailAddress>? Bcc { get; set; } = new List<EmailAddress>();
        public EmailAddress? From { get; set; } = new EmailAddress();
        public string? Subject { get; set; } = string.Empty;
        public string? Body { get; set; } = string.Empty;
    }

    public class GMmailSettings
    {
        public string userName { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string fromName { get; set; } = string.Empty;
        public string fromAddress { get; set; } = string.Empty;
    }

   
}