using System;
using System.Collections.Generic;
using System.Text;

namespace DataModelsLibrary.Models
{
    public class SendEmailModel
    {
        public int companyId { get; set; }
        public int userId { get; set; }
        public int? candidateId { get; set; }
        public int cvId { get; set; }
        public int? positionId { get; set; }
        public List<EmailAddress>? toAddresses { get; set; }
        public string? subject { get; set; } = "";
        public string? body { get; set; } = "";
        public List<EmailCvAttachmentModel>? attachCvs { get; set; }

        //public int? candId { get; set; }
        //public int? cvId { get; set; }
        //public int? positionId { get; set; } = 0;
        //public string? positionName { get; set; } = string.Empty;
        //public string? customerName { get; set; } = string.Empty;
        //public int? customerId { get; set; } = 0;

    }

    public class EmailCvAttachmentModel
    {
        public string cvKey { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
    }

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
        public List<AttachmentModel>? Attachments { get; set; }
        public string? MailSenderUserName { get; set; } = string.Empty;
        public string? MailSenderPassword { get; set; } = string.Empty;
    }

    public class AttachmentModel
    {
        public string name { get; set; } = string.Empty;
        public MemoryStream Attachment { get; set; } = new MemoryStream();
    }

    public class GMmailSettings
    {
        public string userName { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string fromName { get; set; } = string.Empty;
        public string fromAddress { get; set; } = string.Empty;
    }


}
