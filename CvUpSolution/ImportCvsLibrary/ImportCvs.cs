using MailKit.Net.Imap;
using MailKit;
using System.Text;
using MailKit.Search;
using MimeKit;
using Microsoft.Extensions.Configuration;
using CvsPositionsLibrary;
using DataModelsLibrary.Models;
using System.Text.RegularExpressions;
using static DataModelsLibrary.GlobalConstant;
using Spire.Pdf;
using Spire.Pdf.Exporting.Text;
using Spire.Pdf.Texts;
using System.Drawing.Imaging;

namespace ImportCvsLibrary
{
    public class ImportCvs : IImportCvs
    {

        ICvsPositionsServise _cvsPositionsServise;
        string _cvsRootFolder;
        string _mailUserName;
        string _mailPassword;

        public ImportCvs(IConfiguration config, ICvsPositionsServise cvsPositionsServise)
        {
            _cvsPositionsServise = cvsPositionsServise;

            _cvsRootFolder = config["GlobalSettings:CvsFilesRootFolder"];
            Directory.CreateDirectory(_cvsRootFolder);
            _mailUserName = config["GlobalSettings:gmailUserName"];
            _mailPassword = config["GlobalSettings:gmailPassword"];
        }

        public void ImportFromGmail()
        {

            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, true);
                client.Authenticate(_mailUserName, _mailPassword);
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);
                var uids = client.Inbox.Search(SearchQuery.NotSeen);

                foreach (var uid in uids)
                {
                    //try
                    //{
                    var message = inbox.GetMessage(uid);
                    Console.WriteLine("Subject: {0}", message.Subject);
                    inbox.SetFlags(uid, MessageFlags.Seen, true);
                    List<ImportCvModel> cvsList = new List<ImportCvModel>();
                    SaveEmailAttachmentToFile(message, uid, cvsList);
                    ExtractAttachmentsProps(cvsList);
                    AddAttachmentsToDb(cvsList);
                    AddCvsToIndex(cvsList);

                    //}
                    //catch (Exception)
                    //{
                    //    //log
                    //}
                }

                client.Disconnect(true);
            }
        }

        private void SaveEmailAttachmentToFile(MimeMessage message,UniqueId uid, List<ImportCvModel> cvsList)
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
                string fileExtension = System.IO.Path.GetExtension(originalFileName).ToLower();
                string fileNamePath = GetSaveAttachmentLocation(attachment, companyId, uqId,++counter, fileExtension, out string cvId);

                if (fileExtension == DOC_EXTENSION || fileExtension == DOCX_EXTENSION || fileExtension == PDF_EXTENSION)
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

                        cvsList.Add(new ImportCvModel { companyId = companyId, 
                            cvId = cvId, 
                            fileExtension = fileExtension, 
                            fileNamePath = fileNamePath, 
                            emailId= uid.ToString(),
                            subject = message.Subject,
                            from = message.From.ToString(),
                        });
                    }
                }
            }
        }

        private void ExtractAttachmentsProps(List<ImportCvModel> cvsList)
        {
            foreach (var item in cvsList)
            {
                if (item.fileExtension == PDF_EXTENSION)
                {
                    item.cvTxt = GetCvTxtPdf(item.fileNamePath);
                }
                else
                {
                    item.cvTxt =  GetCvTxtWord(item.fileNamePath);
                }

                item.cvAsciiSum = GetCvAsciiSum(item.cvTxt);
                GetCandidateEmail(item);
                GetCandidatePhone(item);
            }
        }

        private void AddAttachmentsToDb(List<ImportCvModel> cvsList)
        {
            foreach (var item in cvsList)
            {
                if (item.email.Length > 0)
                {
                    item.candidateId = _cvsPositionsServise.GetAddCandidateId( Convert.ToInt32(item.companyId), item.email,item.phone);
                    _cvsPositionsServise.AddNewCvToDb(item);
                }
            }
        }

        private void AddCvsToIndex(List<ImportCvModel> cvsList)
        {
            foreach (var item in cvsList)
            {
                if (item.email.Length > 0)
                {
                    _cvsPositionsServise.AddNewCvToIndex(item);
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
                item.phone = matches[0].Value;
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

        private string GetCvTxtPdf(string fileNamePath)
        {
            StringBuilder cvTxtSB = new StringBuilder();
            SimpleTextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

            Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
            doc.LoadFromFile(fileNamePath);
            //doc.SaveAsImage()

            StringBuilder buffer = new StringBuilder();
            PdfTextExtractOptions extractOptions = new PdfTextExtractOptions();
            extractOptions.IsExtractAllText = true;

            foreach (PdfPageBase page in doc.Pages)
            {
                PdfTextExtractor textExtractor = new PdfTextExtractor(page);
                cvTxtSB.Append(textExtractor.ExtractText(extractOptions));
            }

            doc.Close();
            string cvTxt = cvTxtSB.ToString();
            return RemoveCvExtraSpaces(cvTxt);
        }

        private string GetCvTxtWord(string fileNamePath)
        {
            Spire.Doc.Document document = new Spire.Doc.Document(fileNamePath);
            //document.SaveToFile("Convert.PDF", Spire.Doc.FileFormat.PDF);
            string cvTxt = document.GetText();
           System.Drawing.Image[] cvImages =  document.SaveToImages(Spire.Doc.Documents.ImageType.Bitmap);

            //foreach (var item in cvImages)
            //{
            //    item.Save("sdf");
            //}

            return RemoveCvExtraSpaces(cvTxt);
        }

        private string RemoveCvExtraSpaces(string cvTxt)
        {
            string txt  = Regex.Replace(cvTxt, @"\s+", " ");
            txt = txt.Length > 7999 ? txt.Substring(0, 7999) : txt;
            return txt;
        }

        private int GetCvAsciiSum(string cvTxt)
        {
            int docAsciiSum = 0;

            foreach (char c in cvTxt)
            {
                try
                {
                    docAsciiSum += Convert.ToInt32(c);
                }
                catch (Exception) { }
            }

            return docAsciiSum;
        }

        private string GetSaveAttachmentLocation(MimeEntity attachment,string companyId,int uqId, int counter, string fileExtension, out string cvId)
        {
            string companyFolder = companyId+"_";
            
            string yearFolder =  DateTime.Now.Year.ToString("0000")+"_";
            string monthFolder = DateTime.Now.Month.ToString("00") + "_";

            Directory.CreateDirectory($"{_cvsRootFolder}\\{companyFolder}");
            Directory.CreateDirectory($"{_cvsRootFolder}\\{companyFolder}\\{yearFolder}");
            Directory.CreateDirectory($"{_cvsRootFolder}\\{companyFolder}\\{yearFolder}\\{monthFolder}");

            string cvDay =  DateTime.Now.Day.ToString("00") + "_";
            string cvTime =  DateTime.Now.ToString("HHmm") + "_";

            cvId = companyFolder + yearFolder + monthFolder + cvDay + cvTime + uqId.ToString() + counter.ToString();
            string fileName = cvId + fileExtension;
            var fileNamePath = $"{_cvsRootFolder}\\{companyFolder}\\{yearFolder}\\{monthFolder}\\{fileName}";
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

                        if (toAddress.IndexOf(_mailUserName) > -1)
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
