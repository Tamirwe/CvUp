﻿using MailKit.Net.Imap;
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
    public partial class ImportCvs : IImportCvs
    {

        ICandsPositionsServise _cvsPositionsServise;
        string _filesRootFolder;
        string _gmailUserName;
        string _mailPassword;
        string _cvFolderPath = "";
        string _cvTempFolderPath = "";
        string _yearFolder = DateTime.Now.Year.ToString("0000");
        string _monthFolder = DateTime.Now.Month.ToString("00");
        string _companyFolder = "";
        List<company_cvs_email>? _companiesEmail;
        List<ParserRulesModel> _parsersRulesAllCompanies;
        List<ParserRulesModel> _parsersRules;
        ImportCvModel _importCv = new ImportCvModel();

        public ImportCvs(IConfiguration config, ICandsPositionsServise cvsPositionsServise)
        {
            _cvsPositionsServise = cvsPositionsServise;

            _filesRootFolder = config["GlobalSettings:CvUpFilesRootFolder"];
            //Directory.CreateDirectory(_filesRootFolder);
            _gmailUserName = config["GlobalSettings:GmailUserName"];
            _mailPassword = config["GlobalSettings:GmailPassword"];
        }

        public async void ImportFromGmail()
        {
            _companiesEmail = await _cvsPositionsServise.GetCompaniesEmails();
            _parsersRulesAllCompanies = await _cvsPositionsServise.GetParsersRules();

            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, true);
                client.Authenticate(_gmailUserName, _mailPassword);
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);
                var uids = client.Inbox.Search(SearchQuery.NotSeen);

                foreach (var uid in uids)
                {
                    var message = inbox.GetMessage(uid);
                    Console.WriteLine("Subject: {0}", message.Subject);

                    // no need because app is only for bella
                    //int companyId = GetCompanyIdFromAddress(message.To);
                    int companyId = 154;

                    if (companyId > 0)
                    {
                        CreateCvFolder(companyId);
                        _parsersRules = _parsersRulesAllCompanies.Where(x=>x.company_id == companyId).ToList();

                        foreach (MimeEntity attachment in message.Attachments)
                        {
                            string originalFileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
                            string fileExtension = System.IO.Path.GetExtension(originalFileName).ToLower();

                            if (fileExtension == DOC_EXTENSION || fileExtension == DOCX_EXTENSION || fileExtension == PDF_EXTENSION)
                            {
                                _importCv = new ImportCvModel
                                {
                                    companyId = companyId,
                                    emailId = message.MessageId,
                                    subject = Regex.Replace(message.Subject, "fwd:", "", RegexOptions.IgnoreCase).Trim(),
                                    from = message.From.ToString(),
                                    fileExtension = fileExtension,
                                };

                                SaveAttachmentToTemporaryFile( attachment);
                                ParseEmailSubject();
                                await CvExtractDataAndSave();
                            }
                        }
                    }

                    inbox.SetFlags(uid, MessageFlags.Seen, true);
                }

                client.Disconnect(true);
            }
        }

        private async Task CvExtractDataAndSave()
        {
            ExtractCvProps();
            await CandidateFindOrCreate();
            GetCvAsciiSum();
            await CheckIsCvDuplicate();
            await AddCvToDb();
            RenameAndMoveAttachmentToFolder();
            await UpdateCvKeyId();
            await UpdateCandidateLastCv();
            await AddCvToIndex();
        }

        private async Task UpdateCandidateLastCv()
        {
            await _cvsPositionsServise.UpdateCandidateLastCv(_importCv);
        }

        private async Task UpdateCvKeyId()
        {
            if (!_importCv.isSameCv)
            {
                await _cvsPositionsServise.UpdateCvKeyId(_importCv);
            }
        }

        private void CreateCvFolder(int companyId)
        {
            _companyFolder = companyId.ToString();
            Directory.CreateDirectory($"{_filesRootFolder}\\_{_companyFolder}\\cvs");
            _cvTempFolderPath = $"{_filesRootFolder}\\_{_companyFolder}\\temp";
            Directory.CreateDirectory(_cvTempFolderPath);
            Directory.CreateDirectory($"{_filesRootFolder}\\_{_companyFolder}\\cvs\\{_yearFolder}");
            _cvFolderPath = $"{_filesRootFolder}\\_{_companyFolder}\\cvs\\{_yearFolder}\\{_monthFolder}";
            Directory.CreateDirectory(_cvFolderPath);
        }

        private void SaveAttachmentToTemporaryFile( MimeEntity attachment)
        {
            var myUniqueFileName = $@"{DateTime.Now.Ticks}{_importCv.fileExtension}";
            _importCv.tempFilePath = $"{_cvTempFolderPath}\\{myUniqueFileName}";

            using (var stream = File.Create(_importCv.tempFilePath))
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

        private void RenameAndMoveAttachmentToFolder()
        {
            if (!_importCv.isSameCv)
            {
                string fileNamePath = GetAttachmentFileNamePath(_importCv.cvId, _importCv.fileExtension, out string cvKey);
                _importCv.cvKey = cvKey;
                File.Move(_importCv.tempFilePath, fileNamePath);
            }
        }

        private void ExtractCvProps()
        {
            if (_importCv.fileExtension == PDF_EXTENSION)
            {
                _importCv.cvTxt = GetCvTxtPdf(_importCv.tempFilePath);
            }
            else
            {
                _importCv.cvTxt = GetCvTxtWord(_importCv.tempFilePath);
            }

            GetCandidateEmail();
            GetCandidatePhone();
        }

        private async Task CheckIsCvDuplicate()
        {
            if (!_importCv.isNewCandidate)
            {
                List<cv> cvs = await _cvsPositionsServise.CheckIsCvDuplicate(_importCv.companyId, _importCv.candidateId, _importCv.cvAsciiSum);

                if (cvs.Count > 0)
                {
                    _importCv.isDuplicate = true;
                    _importCv.duplicateCvId = cvs.First().id;

                    foreach (var cv in cvs)
                    {
                        if (cv.subject == _importCv.subject && cv.date_created.AddDays(30) > DateTime.Now && cv.from == _importCv.from)
                        {
                            _importCv.isSameCv = true;
                            _importCv.cvId = cv.id;
                            _importCv.duplicateCvId = 0;
                            _importCv.isDuplicate = false;
                            break;
                        }
                    }
                }
            }
        }

        private  void ParseEmailSubject()
        {
            if (_parsersRules.Count > 0)
            {
                List<int> parsersIds = _parsersRules.DistinctBy(x => x.parser_id).Select(x => x.parser_id).ToList();
                string seperator = "~~";

                foreach (int id in parsersIds)
                {
                    List<ParserRulesModel> parserRules = _parsersRules.Where(x => x.parser_id == id).OrderBy(x => x.order).ToList();
                    string subject = _importCv.subject;
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
                                    //to be check
                                    var nameParts = subjectArr[i].Split(' ',StringSplitOptions.RemoveEmptyEntries);
                                    _importCv.firstName = nameParts[nameParts.Length - 1].Trim();
                                    _importCv.lastName = subjectArr[i].Replace(_importCv.firstName, "").Trim();
                                    _importCv.candidateName = subjectArr[i];
                                    break;
                                case nameof(ParserValueType.Position):
                                    _importCv.positionRelated = subjectArr[i];
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

        private async Task CandidateFindOrCreate()
        {
           await _cvsPositionsServise.AddUpdateCandidateFromCvImport(_importCv);
        }

        private async Task AddCvToDb()
        {
            if (_importCv.isSameCv)
            {
                await _cvsPositionsServise.UpdateSameCv(_importCv);
            }
            else
            {
                _importCv.cvId = await _cvsPositionsServise.AddCv(_importCv);
            }
        }

        private async Task AddCvToIndex()
        {
            if (!_importCv.isDuplicate && !_importCv.isSameCv)
            {
                await _cvsPositionsServise.SaveCandidateToIndex(_importCv.companyId, _importCv.candidateId);
            }
        }

        private void GetCandidatePhone()
        {
            const string MatchPhonePattern = @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}";
            Regex rx = new Regex(MatchPhonePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = rx.Matches(_importCv.cvTxt);

            if (matches.Count > 0)
            {
                _importCv.phone = matches[0].Value;
            }
        }

        private void GetCandidateEmail()
        {
            Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
            RegexOptions.IgnoreCase);
            MatchCollection emailMatches = emailRegex.Matches(_importCv.cvTxt);

            if (emailMatches.Count > 0)
            {
                _importCv.emailAddress = emailMatches[0].Value;
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

        private void GetCvAsciiSum()
        {
            int docAsciiSum = 0;

            foreach (char c in _importCv.cvTxt)
            {
                try
                {
                    docAsciiSum += Convert.ToInt32(c);
                }
                catch (Exception) { }
            }

            _importCv.cvAsciiSum = docAsciiSum;
        }

        private string GetAttachmentFileNamePath(int cvId, string fileExtension, out string cvKey)
        {
            int fileTypeKey = Utils.FileTypeKey(fileExtension);
            cvKey = $"{_companyFolder}-{_yearFolder}{_monthFolder}{fileTypeKey}-{cvId}";

            string fileName = $"{_companyFolder}-{_yearFolder}{_monthFolder}-{cvId}{fileExtension}";

            //string fileName = cvKey + fileExtension;
            var fileNamePath = $"{_filesRootFolder}\\_{_companyFolder}\\cvs\\{_yearFolder}\\{_monthFolder}\\{fileName}";
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
