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
using DataModelsLibrary.Models;
using System.IO;
using Spire.Doc;
using Spire.Doc.Documents;
using System.Text.RegularExpressions;

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
            List<ImportCvModel> cvsList = new List<ImportCvModel>();

            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, true);
                client.Authenticate(importMailAddress, importMailPassword);
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);
                var uids = client.Inbox.Search(SearchQuery.NotSeen);

                foreach (var uid in uids)
                {
                    var message = inbox.GetMessage(uid);
                    SaveEmailAttachmentToFile(message, cvsList);
                    Console.WriteLine("Subject: {0}", message.Subject);
                    inbox.SetFlags(uid, MessageFlags.Seen, true);
                }

                client.Disconnect(true);
            }

            ExtractAttachmentsProps(cvsList);
            SaveAttachmentsToDb(cvsList);
        }

        private void SaveEmailAttachmentToFile(MimeMessage message, List<ImportCvModel> cvsList)
        {
            string companyId = GetCompanyIdFromAddress(message.To);
            int uqId = _cvsPositionsServise.GetUniqueCvId();

            if (companyId == "")
            {
                return;
            }

            foreach (MimeEntity attachment in message.Attachments)
            {
                int counter = 0;
                string originalFileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
                string fileExtension = Path.GetExtension(originalFileName).ToLower();
                string fileNamePath = GetSaveAttachmentLocation(attachment, companyId, ++counter, fileExtension);

                if (fileExtension == ".doc" || fileExtension == ".docx" || fileExtension == ".pdf")
                {
                    using (var stream = File.Create(fileNamePath))
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

                        cvsList.Add(new ImportCvModel { companyId = companyId, fileExtension = fileExtension, fileNamePath = fileNamePath });
                    }
                }
            }
        }

        private void ExtractAttachmentsProps(List<ImportCvModel> cvsList)
        {
            foreach (var item in cvsList)
            {
                GetCvTxt(item);
                GetCandidateEmail(item);
                GetCandidatePhone(item);
            }
        }

        private void SaveAttachmentsToDb(List<ImportCvModel> cvsList)
        {
            foreach (var item in cvsList)
            {
                if (item.email.Length > 0)
                {
                    _cvsPositionsServise.GetAddCandidateId(item);
                }
            }
        }

        private void GetCandidatePhone(ImportCvModel item)
        {
            const string MatchPhonePattern =       @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}";
            Regex rx = new Regex(MatchPhonePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = rx.Matches(item.cvTxt);

            if (matches.Count > 0)
            {
                item.email = matches[0].Value;
            }
        }

        private void GetCandidateEmail(ImportCvModel item)
        {
            Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
            RegexOptions.IgnoreCase);
            MatchCollection emailMatches = emailRegex.Matches(item.cvTxt);

            if (emailMatches.Count>0)
            {
                item.email = emailMatches[0].Value;
            }
        }

        private void GetCvTxt(ImportCvModel item)
        {
            Spire.Doc.Document document = new Spire.Doc.Document(item.fileNamePath);
            item.cvTxt = document.GetText();
        }

        private string GetSaveAttachmentLocation(MimeEntity attachment,string companyId,int counter, string fileExtension)
        {
            string companyFolder = "c" + companyId;
            string yearFolder = "y" + DateTime.Now.Year.ToString("0000");
            string monthFolder = "m" + DateTime.Now.Month.ToString("00");

            Directory.CreateDirectory(cvFilesPath + companyFolder);
            Directory.CreateDirectory(cvFilesPath + companyFolder + "\\" + yearFolder);
            Directory.CreateDirectory(cvFilesPath + companyFolder + "\\" + yearFolder + "\\" + monthFolder);

            string cvDay = "d" + DateTime.Now.Day.ToString("00");
            string cvTime = "t" + DateTime.Now.ToString("HHmm");

            string fileName = companyFolder + yearFolder + monthFolder + cvDay + cvTime + "q" + counter.ToString() + fileExtension;
            var fileNamePath = cvFilesPath + companyFolder + "\\" + yearFolder + "\\" + monthFolder + "\\" + fileName;
            return fileNamePath;
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
