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
using System.ComponentModel.Design;

namespace ImportCvsLibrary
{
    public class ImportCvs : IImportCvs
    {

        ICvsPositionsServise _cvsPositionsServise;
        string _cvsRootFolder;
        string _gmailUserName;
        string _mailPassword;
        string _cvFolderPath = "";
        string _cvTempFolderPath = "";
        string _yearFolder = DateTime.Now.Year.ToString("0000") + "_";
        string _monthFolder = DateTime.Now.Month.ToString("00") + "_";
        string _companyFolder = "";

        public ImportCvs(IConfiguration config, ICvsPositionsServise cvsPositionsServise)
        {
            _cvsPositionsServise = cvsPositionsServise;

            _cvsRootFolder = config["GlobalSettings:CvsFilesRootFolder"];
            Directory.CreateDirectory(_cvsRootFolder);
            _gmailUserName = config["GlobalSettings:gmailUserName"];
            _mailPassword = config["GlobalSettings:gmailPassword"];
        }

        public void ImportFromGmail()
        {

            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, true);
                client.Authenticate(_gmailUserName, _mailPassword);
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
                    int companyId = GetCompanyIdFromAddress(message.To);

                    if (companyId > 0)
                    {
                        CreateCvFolder(companyId);

                        foreach (MimeEntity attachment in message.Attachments)
                        {
                            string originalFileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
                            string fileExtension = System.IO.Path.GetExtension(originalFileName).ToLower();

                            if (fileExtension == DOC_EXTENSION || fileExtension == DOCX_EXTENSION || fileExtension == PDF_EXTENSION)
                            {
                                ImportCvModel importCv = new ImportCvModel
                                {
                                    companyId = companyId,
                                    emailId = uid.ToString(),
                                    subject = Regex.Replace(message.Subject, "fwd:", "", RegexOptions.IgnoreCase).Trim(),
                                    from = message.From.ToString(),
                                    fileExtension = fileExtension,
                                };

                                SaveAttachmentToTemporaryFile(importCv, attachment);
                                ParseEmailSubject(importCv);
                                CvExtractDataAndSave(importCv);
                            }
                        }

                        //}
                        //catch (Exception)
                        //{
                        //    //log
                        //}
                    }
                }

                client.Disconnect(true);
            }
        }

        private void CvExtractDataAndSave(ImportCvModel importCv)
        {
            ExtractCvProps(importCv);
            CandidateFindOrCreate(importCv);
            CheckIsCvDuplicate(importCv);
            AddCvToDb(importCv);
            RenameAndMoveAttachmentToFolder(importCv);
            UpdateCvKeyId(importCv);
            UpdateDuplicateAndLastCv(importCv);
            AddCvToIndex(importCv);
        }

        private void UpdateDuplicateAndLastCv(ImportCvModel importCv)
        {
            _cvsPositionsServise.UpdateDuplicateAndLastCv(importCv);
        }

        private void UpdateCvKeyId(ImportCvModel importCv)
        {
            _cvsPositionsServise.UpdateCvKeyId(importCv);
        }

        private void CreateCvFolder(int companyId)
        {
            _companyFolder = companyId + "_";
            Directory.CreateDirectory($"{_cvsRootFolder}\\{_companyFolder}");
            _cvTempFolderPath = $"{_cvsRootFolder}\\{_companyFolder}\\temp";
            Directory.CreateDirectory(_cvTempFolderPath);
            Directory.CreateDirectory($"{_cvsRootFolder}\\{_companyFolder}\\{_yearFolder}");
            _cvFolderPath = $"{_cvsRootFolder}\\{_companyFolder}\\{_yearFolder}\\{_monthFolder}";
            Directory.CreateDirectory(_cvFolderPath);
        }

        private void SaveAttachmentToTemporaryFile(ImportCvModel importCv, MimeEntity attachment)
        {
            var myUniqueFileName = $@"{DateTime.Now.Ticks}{importCv.fileExtension}";
            importCv.tempFilePath = $"{_cvTempFolderPath}\\{myUniqueFileName}";

            using (var stream = File.Create(importCv.tempFilePath))
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

        private void RenameAndMoveAttachmentToFolder(ImportCvModel importCv)
        {
            string fileNamePath = GetAttachmentFileNamePath(importCv.cvId, importCv.fileExtension, out string cvKey);
            importCv.cvKey = cvKey;
            File.Move(importCv.tempFilePath, fileNamePath);
        }

        private void ExtractCvProps(ImportCvModel importCv)
        {
            if (importCv.fileExtension == PDF_EXTENSION)
            {
                importCv.cvTxt = GetCvTxtPdf(importCv.tempFilePath);
            }
            else
            {
                importCv.cvTxt = GetCvTxtWord(importCv.tempFilePath);
            }

            GetCandidateEmail(importCv);
            GetCandidatePhone(importCv);
        }

        private void CheckIsCvDuplicate(ImportCvModel importCv)
        {
            importCv.cvAsciiSum = GetCvAsciiSum(importCv.cvTxt);
            cv? cv = _cvsPositionsServise.CheckIsCvDuplicate(importCv.companyId, importCv.candidateId, importCv.subject, importCv.cvAsciiSum);

            if (cv != null)
            {
                importCv.isDuplicate = true;
            }
        }

        private void ParseEmailSubject(ImportCvModel cv)
        {
            List<ParserRulesModel> parsersRules = _cvsPositionsServise.GetParsersRules(cv.companyId);

            if (parsersRules.Count > 0)
            {
                List<int> parsersIds = parsersRules.DistinctBy(x => x.parser_id).Select(x => x.parser_id).ToList();
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

        private void CandidateFindOrCreate(ImportCvModel importCv)
        {
            importCv.candidateId = _cvsPositionsServise.AddUpdateCandidateFromCvImport(importCv);
        }

        private void AddCvToDb(ImportCvModel importCv)
        {
            importCv.cvId = _cvsPositionsServise.AddCv(importCv);
        }

        private void AddCvToIndex(ImportCvModel importCv)
        {
            if (importCv.email.Length > 0)
            {
                _cvsPositionsServise.AddNewCvToIndex(importCv);
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
            string cvTxt = document.GetText();
            System.Drawing.Image[] cvImages = document.SaveToImages(Spire.Doc.Documents.ImageType.Bitmap);
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

        private string GetAttachmentFileNamePath(int cvId, string fileExtension, out string cvKey)
        {
            string cvDay = DateTime.Now.Day.ToString("00") + "_";
            string cvTime = DateTime.Now.ToString("HHmm") + "_";
            cvKey = $"{_companyFolder}{_yearFolder}{_monthFolder}{cvDay}{cvTime}{cvId}_{fileExtension.Remove(0, 1)}";
            string fileName = cvKey + fileExtension;
            var fileNamePath = $"{_cvsRootFolder}\\{_companyFolder}\\{_yearFolder}\\{_monthFolder}\\{fileName}";
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

                        if (toAddress.IndexOf(_gmailUserName) > -1)
                        {
                            companyIdStr = toAddress.Split('+')[1].Substring(7);
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
