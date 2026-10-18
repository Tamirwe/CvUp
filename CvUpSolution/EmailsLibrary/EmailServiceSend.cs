using DataModelsLibrary.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Text;
using System.Text.RegularExpressions;

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
                }
            }

            if (eml.Cc != null)
            {
                foreach (var item in eml.Cc)
                {
                    message.Cc.Add(new MailboxAddress(item.Name, item.Address));
                }
            }

            if (eml.Bcc != null)
            {
                foreach (var item in eml.Bcc)
                {
                    message.Bcc.Add(new MailboxAddress(item.Name, item.Address));
                }
            }

            message.Subject = eml.Subject;

            // ---- multipart/alternative: plain, html, calendar ----
            // Order matters: least preferred first, calendar last.
            var alternative = new MultipartAlternative();

            var plain = new TextPart("plain");
            plain.SetText(Encoding.UTF8, HtmlToPlainText(eml.Body));
            alternative.Add(plain);

            var html = new TextPart("html");
            html.SetText(Encoding.UTF8, eml.Body ?? string.Empty);
            alternative.Add(html);

            if (!string.IsNullOrEmpty(eml.IcsContent))
            {
                // TextPart, NOT MimePart. No FileName, no ContentDisposition —
                // either one turns this back into an attachment.
                var invite = new TextPart("calendar");

                invite.ContentType.Parameters.Add("method", string.IsNullOrEmpty(eml.IcsMethod) ? "REQUEST" : eml.IcsMethod);
                invite.SetText(Encoding.UTF8, eml.IcsContent);
                invite.ContentTransferEncoding = ContentEncoding.Base64;

                alternative.Add(invite);
            }

            // ---- real attachments, if any, go in an outer multipart/mixed ----
            if (eml.Attachments != null && eml.Attachments.Any())
            {
                var mixed = new Multipart("mixed") { alternative };

                foreach (var item in eml.Attachments)
                {
                    if (item.Attachment.CanSeek)
                    {
                        item.Attachment.Position = 0;
                    }

                    // Cvs go out as pdf and say nothing about their type; a
                    // contract names its own, so anything that carries a
                    // fileName keeps it, spaces and all.
                    var part = new MimePart(
                        item.mediaType ?? "application",
                        item.mediaSubtype ?? "pdf")
                    {
                        Content = new MimeContent(item.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        FileName = item.fileName ?? $"{item.name.Replace(' ', '_')}.pdf",
                    };

                    mixed.Add(part);
                }

                message.Body = mixed;
            }
            else
            {
                message.Body = alternative;
            }

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                client.AuthenticationMechanisms.Remove("XOAUTH2");

                await client.AuthenticateAsync(eml.MailSenderUserName, eml.MailSenderPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        private static string HtmlToPlainText(string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            var text = Regex.Replace(html, @"<br\s*/?>|</p>|</div>|</tr>", "\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "<.*?>", string.Empty, RegexOptions.Singleline);
            text = System.Net.WebUtility.HtmlDecode(text);
            text = Regex.Replace(text, @"[ \t]+", " ");
            text = Regex.Replace(text, @"\n{3,}", "\n\n");

            return text.Trim();
        }
    }
}