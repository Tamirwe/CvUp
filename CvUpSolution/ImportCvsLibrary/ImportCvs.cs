using MailKit.Net.Imap;
using MailKit;
using System.Text;
using MailKit.Search;
using MimeKit;
using Microsoft.Extensions.Configuration;
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
using GeneralLibrary;
using CandsPositionsLibrary;

namespace ImportCvsLibrary
{
    public class ImportCvs : IImportCvs
    {

        ICandsPositionsServise _cvsPositionsServise;
        string _cvsRootFolder;
        string _gmailUserName;
        string _mailPassword;
        string _cvFolderPath = "";
        string _cvTempFolderPath = "";
        string _yearFolder = DateTime.Now.Year.ToString("0000");
        string _monthFolder = DateTime.Now.Month.ToString("00");
        string _companyFolder = "";
        List<company_cvs_email>? _companiesEmail;

        public ImportCvs(IConfiguration config, ICandsPositionsServise cvsPositionsServise)
        {
            _cvsPositionsServise = cvsPositionsServise;

            _cvsRootFolder = config["GlobalSettings:CvsFilesRootFolder"];
            Directory.CreateDirectory(_cvsRootFolder);
            _gmailUserName = config["GlobalSettings:gmailUserName"];
            _mailPassword = config["GlobalSettings:gmailPassword"];
        }

        public void ImportFromGmail()
        {
            _companiesEmail = _cvsPositionsServise.GetCompaniesEmails();

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
                                    emailId = message.MessageId,
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
            GetCvAsciiSum(importCv);
            CheckIsCvDuplicate(importCv);
            AddCvToDb(importCv);
            RenameAndMoveAttachmentToFolder(importCv);
            UpdateCvKeyId(importCv);
            UpdateCandidateLastCv(importCv);
            AddCvToIndex(importCv);
        }

        private void UpdateCandidateLastCv(ImportCvModel importCv)
        {
            _cvsPositionsServise.UpdateCandidateLastCv(importCv);
        }

        private void UpdateCvKeyId(ImportCvModel importCv)
        {
            if (!importCv.isSameCv)
            {
                _cvsPositionsServise.UpdateCvKeyId(importCv);
            }
        }

        private void CreateCvFolder(int companyId)
        {
            _companyFolder = companyId.ToString();
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
            if (!importCv.isSameCv)
            {
                string fileNamePath = GetAttachmentFileNamePath(importCv.cvId, importCv.fileExtension, out string cvKey);
                importCv.cvKey = cvKey;
                File.Move(importCv.tempFilePath, fileNamePath);
            }
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
            if (!importCv.isNewCandidate)
            {
                List<cv> cvs = _cvsPositionsServise.CheckIsCvDuplicate(importCv.companyId, importCv.candidateId, importCv.cvAsciiSum);

                if (cvs.Count > 0)
                {
                    importCv.isDuplicate = true;
                    importCv.duplicateCvId = cvs.First().id;

                    foreach (var cv in cvs)
                    {
                        if (cv.subject == importCv.subject && cv.date_created.AddDays(30) > DateTime.Now && cv.from == importCv.from)
                        {
                            importCv.isSameCv = true;
                            importCv.cvId = cv.id;
                            importCv.duplicateCvId = 0;
                            importCv.isDuplicate = false;
                            break;
                        }
                    }
                }
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
            _cvsPositionsServise.AddUpdateCandidateFromCvImport(importCv);
        }

        private void AddCvToDb(ImportCvModel importCv)
        {
            if (importCv.isSameCv)
            {
                _cvsPositionsServise.UpdateSameCv(importCv);
            }
            else
            {
                importCv.cvId = _cvsPositionsServise.AddCv(importCv);
            }
        }

        private void AddCvToIndex(ImportCvModel importCv)
        {
            if (!importCv.isDuplicate && !importCv.isSameCv)
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
                item.emailAddress = emailMatches[0].Value;
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

        private void GetCvAsciiSum(ImportCvModel importCv)
        {
            int docAsciiSum = 0;

            foreach (char c in importCv.cvTxt)
            {
                try
                {
                    docAsciiSum += Convert.ToInt32(c);
                }
                catch (Exception) { }
            }

            importCv.cvAsciiSum = docAsciiSum;
        }

        private string GetAttachmentFileNamePath(int cvId, string fileExtension, out string cvKey)
        {
            cvKey = $"{_companyFolder}-{_yearFolder}{_monthFolder}{Utils.FileTypeKey(fileExtension)}-{cvId}";
            string fileName = cvKey + fileExtension;
            var fileNamePath = $"{_cvsRootFolder}\\{_companyFolder}\\{_yearFolder}\\{_monthFolder}\\{fileName}";
            return fileNamePath;
        }

        private int GetCompanyIdFromAddress(InternetAddressList addressList)
        {
            foreach (var toEmail in addressList.ToList())
            {
                if (_companiesEmail != null)
                {
                    var toAddress = toEmail.ToString();

                    var companyEmail = _companiesEmail.Where(x => x.email == toAddress).FirstOrDefault();

                    if (companyEmail != null)
                    {
                        return companyEmail.company_id;
                    }
                }
            }

            return 0;
        }
    }
}
