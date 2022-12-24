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
using J2N.Text;
using DataModelsLibrary.Enums;
using Database.models;

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
                    ExtractCvsProps(cvsList);
                    AddCvsToDb(cvsList);
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

        private void SaveEmailAttachmentToFile(MimeMessage message, UniqueId uid, List<ImportCvModel> cvsList)
        {
            int companyId = GetCompanyIdFromAddress(message.To);
            int uqId = _cvsPositionsServise.GetUniqueCvId();

            if (companyId == 0)
            {
                return;
            }

            foreach (MimeEntity attachment in message.Attachments)
            {
                int counter = 0;
                string originalFileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
                string fileExtension = System.IO.Path.GetExtension(originalFileName).ToLower();
                string fileNamePath = GetSaveAttachmentLocation(attachment, companyId, uqId, ++counter, fileExtension, out string cvId);

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

                        string subject = Regex.Replace(message.Subject, "fwd:", "", RegexOptions.IgnoreCase);

                        cvsList.Add(new ImportCvModel
                        {
                            companyId = companyId,
                            cvId = cvId,
                            fileExtension = fileExtension,
                            fileNamePath = fileNamePath,
                            emailId = uid.ToString(),
                            subject = subject.Trim(),
                            from = message.From.ToString(),
                        });
                    }
                }
            }
        }

        private void ExtractCvsProps(List<ImportCvModel> cvsList)
        {
            foreach (var item in cvsList)
            {
                if (item.fileExtension == PDF_EXTENSION)
                {
                    item.cvTxt = GetCvTxtPdf(item.fileNamePath);
                }
                else
                {
                    item.cvTxt = GetCvTxtWord(item.fileNamePath);
                }

                item.cvAsciiSum = GetCvAsciiSum(item.cvTxt);
                GetCandidateEmail(item);
                GetCandidatePhone(item);
                ParseEmailSubject(item);
            }
        }

        private void ParseEmailSubject(ImportCvModel cv)
        {
            List<ParserRulesModel> parsersRules = _cvsPositionsServise.GetParsersRules(cv.companyId);

            if (parsersRules.Count > 0)
            {
                List<int> parsersIds = parsersRules.DistinctBy(x => x.parser_id).Select(x=>x.parser_id).ToList();
                string seperator = "~~";

                foreach (int id in parsersIds)
                {
                    List<ParserRulesModel> parserRules = parsersRules.Where(x => x.parser_id == id).OrderBy(x => x.order).ToList();
                    string subject = cv.subject;
                    bool isCorrectParser = false;

                    foreach (var rule in parserRules)
                    {
                        if (subject.IndexOf(rule.delimiter) > -1)
                        {
                            subject = subject.Replace(rule.delimiter, seperator);
                            isCorrectParser = true;
                        }
                        else
                        {
                            if (rule.must_metch)
                            {
                                isCorrectParser = false;
                                break;
                            }
                        }
                    }

                    if (isCorrectParser)
                    {
                        string[] subjectArr = subject.Split(seperator);
                        subjectArr = subjectArr.Skip(1).ToArray();//remove first entry, we take the value after the seperator.

                        for (int i = 0; i < subjectArr.Length; i++)
                        {
                            switch (parserRules[i].value_type)
                            {
                                case nameof(ParserValueType.Name):
                                    cv.candidateName = subjectArr[i];
                                    break;
                                case nameof(ParserValueType.Position):
                                    cv.positionRelated = subjectArr[i];
                                    break;
                                case nameof(ParserValueType.CompanyType):

                                    break;
                                case nameof(ParserValueType.Address):

                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void AddCvsToDb(List<ImportCvModel> cvsList)
        {
            foreach (var cv in cvsList)
            {
                if (cv.email.Length > 0)
                {
                    candidate? cand = _cvsPositionsServise.GetCandidateId(cv.email);
                    cv.candidateId = _cvsPositionsServise.AddUpdateCandidate(cv, cand);
                    _cvsPositionsServise.AddNewCvToDb(cv);
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
            const string MatchPhonePattern = @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}";
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

            if (emailMatches.Count > 0)
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

            //MemoryStream dstStream = new MemoryStream();
            //document.SaveToFile(dstStream, Spire.Doc.FileFormat.PDF);

            string cvTxt = document.GetText();
            System.Drawing.Image[] cvImages = document.SaveToImages(Spire.Doc.Documents.ImageType.Bitmap);

            //foreach (var item in cvImages)
            //{
            //    item.Save("sdf");
            //}

            return RemoveCvExtraSpaces(cvTxt);
        }

        private string RemoveCvExtraSpaces(string cvTxt)
        {
            string txt = Regex.Replace(cvTxt, @"\s+", " ");
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

        private string GetSaveAttachmentLocation(MimeEntity attachment, int companyId, int uqId, int counter, string fileExtension, out string cvId)
        {
            string companyFolder = companyId + "_";

            string yearFolder = DateTime.Now.Year.ToString("0000") + "_";
            string monthFolder = DateTime.Now.Month.ToString("00") + "_";

            Directory.CreateDirectory($"{_cvsRootFolder}\\{companyFolder}");
            Directory.CreateDirectory($"{_cvsRootFolder}\\{companyFolder}\\{yearFolder}");
            Directory.CreateDirectory($"{_cvsRootFolder}\\{companyFolder}\\{yearFolder}\\{monthFolder}");

            string cvDay = DateTime.Now.Day.ToString("00") + "_";
            string cvTime = DateTime.Now.ToString("HHmm") + "_";

            cvId = $"{companyFolder}{yearFolder}{monthFolder}{cvDay}{cvTime}{uqId}{counter}_{fileExtension.Remove(0, 1)}";
            string fileName = cvId + fileExtension;
            var fileNamePath = $"{_cvsRootFolder}\\{companyFolder}\\{yearFolder}\\{monthFolder}\\{fileName}";
            return fileNamePath;
        }

        private int GetCompanyIdFromAddress(InternetAddressList addressList)
        {
            string companyIdStr = "";

            try
            {
                addressList.ToList().ForEach(x =>
                {
                    if (companyIdStr == "")
                    {
                        var toAddress = x.ToString().Split('@')[0];

                        if (toAddress.IndexOf(_mailUserName) > -1)
                        {
                            companyIdStr = toAddress.Substring(toAddress.IndexOf("ci") + 2);
                        }
                    }

                });

                return Convert.ToInt32(companyIdStr);
            }
            catch (Exception)
            {
                return 0;
            }

        }
    }
}
