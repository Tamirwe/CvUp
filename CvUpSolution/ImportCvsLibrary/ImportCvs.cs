using MailKit.Net.Imap;
using MailKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Search;
using MimeKit;
using Microsoft.Extensions.Configuration;
using CvsPositionsLibrary;

namespace ImportCvsLibrary
{
    public class ImportCvs : IImportCvs
    {
        IConfiguration _configuration;
        ICvsPositionsServise _cvsPositionsServise;
        string importMailAddress;
        string importMailPassword;
        string cvFilesPath;

        public ImportCvs(IConfiguration config, ICvsPositionsServise cvsPositionsServise)
        {
            _configuration = config;
            _cvsPositionsServise = cvsPositionsServise;
            cvFilesPath = _configuration["AppSettings:CvFilesPath"];
            importMailAddress = _configuration["AppSettings:ImportMailAddress"];
            importMailPassword = _configuration["AppSettings:ImportMailPassword"];
        }

        public void ImportFromGmail()
        {
            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, true);

                client.Authenticate(importMailAddress, importMailPassword);

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);

                var uids = client.Inbox.Search(SearchQuery.NotSeen);

                Console.WriteLine("Total messages: {0}", inbox.Count);
                Console.WriteLine("Recent messages: {0}", inbox.Recent);

                foreach (var uid in uids)
                {
                    var message = inbox.GetMessage(uid);

                    string companyId = GetCompanyIdFromAddress(message.To);

                    if (companyId == "")
                    {
                        return;
                    }

                    foreach (MimeEntity attachment in message.Attachments)
                    {
                        var fileName = cvFilesPath + attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;

                        string fileExtension = System.IO.Path.GetExtension(fileName).ToLower();

                        //string day = DateTime.Now.Day.ToString("00");
                        //string month = DateTime.Now.Month.ToString("00");
                        //string year = DateTime.Now.Year.ToString("0000");

                        if (fileExtension == ".doc" || fileExtension == ".docx" || fileExtension == ".pdf")
                        {
                            using (var stream = File.Create(fileName))
                            {
                                if (attachment is MessagePart)
                                {
                                    var rfc822 = (MessagePart)attachment;

                                    rfc822.Message.WriteTo(stream);
                                }
                                else
                                {
                                    var part = (MimePart)attachment;

                                    part.Content.DecodeTo(stream);
                                }
                            }
                        }
                    }

                    Console.WriteLine("Subject: {0}", message.Subject);
                    inbox.SetFlags(uid, MessageFlags.Seen, true);
                }

                client.Disconnect(true);
            }
        }

        private string GetCompanyIdFromAddress(InternetAddressList addressList)
        {
            string companyId = "";

            try
            {
                addressList.ToList().ForEach(x =>
                {
                    if (companyId == "")
                    {
                        var toAddress = x.ToString().Split('@')[0];

                        if (toAddress.IndexOf(importMailAddress) == 0)
                        {
                            companyId = toAddress.Substring(toAddress.IndexOf("ci") + 2);
                        }
                    }
                });
            }
            catch (Exception)
            {
                return "";
            }

            return companyId;
        }
    }
}
