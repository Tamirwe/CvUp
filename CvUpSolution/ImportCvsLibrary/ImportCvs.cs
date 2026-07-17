using CandsPositionsLibrary;
using Database.models;
using LuceneLibrary;
using QueueLibrary;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using GeneralLibrary;
using ImportCvsLibrary.RegularExpressions;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Spire.Pdf;
using Spire.Pdf.Texts;
using System.Text;
using System.Text.RegularExpressions;
using static DataModelsLibrary.GlobalConstant;

namespace ImportCvsLibrary
{
    public class ImportCvs : IImportCvs
    {

        ICandsServise _candsServise;
        IPositionsServise _positionsServise;
        IDbQueueService _queueService;
        string _filesRootFolder;
        string _cvupNotBackedUpRootFolder;
        string _gmailUserName;
        string _mailPassword;
        string _cvFolderPath = "";
        string _cvTempFolderPath = "";
        string _yearFolder = "";
        string _monthFolder = "";
        string _companyFolder = "";
        List<ParserRulesModel>? _parsersRulesAllCompanies;
        List<ParserRulesModel>? _parsersRules;
        ImportCvModel _importCv = new ImportCvModel();
        private List<blackCandModel>? _blackCandidatesList = null;
        private readonly IMemoryCache _cache;

        public ImportCvs(IMemoryCache cache, ICandsServise candsServise, IPositionsServise positionsServise, IConfiguration configuration, IDbQueueService queueService)
        {
            _filesRootFolder = configuration["CVS_ROOT_FOLDER"]!;
            _cvupNotBackedUpRootFolder = configuration["APP_LOCAL_ROOT_FOLDER"]!;
            _gmailUserName = configuration["IMPORT_GMAIL_USER_NAME"]!;
            _mailPassword = configuration["IMPORT_GMAIL_PASSWORD"]!;

            _candsServise = candsServise;
            _positionsServise = positionsServise;
            _queueService = queueService;
            _cache = cache;
        }

        public async Task ImportFromGmail()
        {

            _blackCandidatesList = _cache.Get<List<blackCandModel>>("blackCandidatesList");

            if (_blackCandidatesList == null)
            {
                _blackCandidatesList = await _candsServise.GetBlackCandidatesList();
                _cache.Set("blackCandidatesList", _blackCandidatesList, TimeSpan.FromHours(1));
            }

            using (var client = new ImapClient())
            {
                try
                {
                    client.Connect("imap.gmail.com", 993, true);
                    client.Authenticate(_gmailUserName, _mailPassword);

                    if (client != null && client.IsConnected)
                    {
                        await processUnReadEmails(client);
                        _importCv.exceptionRow = "1800";

                    }
                    _importCv.exceptionRow = "1900";

                }
                catch (Exception ex)
                {
                    addEventLogEntry(ex, 380);
                }
            }
        }

        private async Task processUnReadEmails(ImapClient client)
        {
            IList<UniqueId>? uids = null;
            IMailFolder? inbox = client.Inbox;

            if (inbox == null || client.Inbox == null) return;

            inbox.Open(FolderAccess.ReadWrite);
            uids = client.Inbox.Search(SearchQuery.NotSeen);

            if (uids != null && uids.Count > 0)
            {
                _parsersRulesAllCompanies = await _positionsServise.GetParsersRules();

                foreach (var uid in uids)
                {
                    try
                    {
                        var message = client.Inbox.GetMessage(uid);

                        //************** SUBJECT: **************
                        Console.WriteLine("Subject: {0}", message.Subject);

                        // no need because app is only for bella
                        //int companyId = GetCompanyIdFromAddress(message.To);
                        int companyId = 154;

                        if (companyId > 0)
                        {
                            _companyFolder = companyId.ToString();
                            var dateCreated = DateTime.Now;
                            _yearFolder = dateCreated.Year.ToString("0000");
                            _monthFolder = dateCreated.Month.ToString("00");

                            CreateCvFolder(companyId);
                            _parsersRules = _parsersRulesAllCompanies.Where(x => x.company_id == companyId).ToList();

                            foreach (var bodyPart in message.BodyParts)
                            {
                                var contentBase = bodyPart.ContentDisposition;

                                if (bodyPart.ContentType.Name != null)
                                {
                                    var part = (MimePart)bodyPart;
                                    var originalFileName = part.FileName;
                                    string fileExtension = System.IO.Path.GetExtension(originalFileName).ToLower();
                                    int fileTypeKey = Utils.FileTypeKey(fileExtension);

                                    if (fileExtension == DOC_EXTENSION || fileExtension == DOCX_EXTENSION || fileExtension == PDF_EXTENSION)
                                    {
                                        _importCv = new ImportCvModel
                                        {
                                            companyId = companyId,
                                            emailId = message.MessageId,
                                            subject = Regex.Replace(message.Subject, "fwd:", "", RegexOptions.IgnoreCase).Trim(),
                                            from = message.From.ToString(),
                                            fileExtension = fileExtension,
                                            fileTypeKey = fileTypeKey,
                                            dateCreated = dateCreated,
                                            exceptionRow = "100"
                                        };

                                        var fromAddress = message.From.Mailboxes.Single().Address;

                                        if (fromAddress == "alljobs@alljob.co.il" && message.BodyParts.Count() > 0)
                                        {
                                            var txtPart = (TextPart)message.BodyParts.First();

                                            if (txtPart != null)
                                            {
                                                _importCv.body = txtPart.Text;
                                            }
                                        }

                                        _importCv.exceptionRow = "200";
                                        SaveAttachmentToTemporaryFile(part);
                                        _importCv.exceptionRow = "300";
                                        ParseEmailSubject();
                                        _importCv.exceptionRow = "400";
                                        await CvExtractDataAndSave();
                                        _importCv.exceptionRow = "1500";


                                    }
                                }
                            }

                            inbox.SetFlags(uid, MessageFlags.Seen, true);
                            _importCv.exceptionRow = "1600";
                        }

                        _importCv.exceptionRow = "1700";
                    }
                    catch (Exception ex)
                    {
                        addEventLogEntry(ex, 471);
                        Console.WriteLine(ex.ToString());
                        inbox.SetFlags(uid, MessageFlags.Seen, true);
                    }
                }
            }
        }

        private async Task CvExtractDataAndSave()
        {
            ExtractCvText();
            ExtractCvProps();

            bool isBlackCand = CheckIsBlackCand();

            if (!isBlackCand)
            {
                await CandidateFindOrCreate();

                GetCvAsciiSum();
                await CheckIsCvDuplicateOrSameCv();

                if (_importCv.isSameCvEmailSubject)
                {
                    await _candsServise.UpdateCvDate(_importCv.cvId);
                }
                else
                {
                    //await AddPositionName();
                    await AddCvToDb();
                    RenameAndMoveAttachmentToFolder();
                    await _candsServise.UpdateCvKeyId(_importCv);
                    await _candsServise.UpdateCandLastCv(_importCv.companyId, _importCv.candidateId, _importCv.cvId, _importCv.isDuplicate, _importCv.dateCreated);
                    await AddCandToMatchPosition();
                    await _queueService.EnqueueAsync("analyze and index new cv", _importCv.candidateId.ToString());
                }
            }
        }

        private async Task AddCandToMatchPosition()
        {
            position? matchPos = await _positionsServise.GetPositionByMatchStr(_importCv.companyId, _importCv.subject);

            if (matchPos != null)
            {
                await _candsServise.AttachPosCandCv(new AttachePosCandCvModel
                {
                    positionId = matchPos.id,
                    candidateId = _importCv.candidateId,
                    companyId = _importCv.companyId,
                    cvId = _importCv.cvId,
                    keyId = _importCv.cvKey
                });
            }
        }

        private void CreateCvFolder(int companyId)
        {
            Directory.CreateDirectory($"{_filesRootFolder}\\_{_companyFolder}\\cvs");
            _cvTempFolderPath = $"{_cvupNotBackedUpRootFolder}\\_{_companyFolder}\\temp";
            Directory.CreateDirectory(_cvTempFolderPath);
            Directory.CreateDirectory($"{_filesRootFolder}\\_{_companyFolder}\\cvs\\{_yearFolder}");
            _cvFolderPath = $"{_filesRootFolder}\\_{_companyFolder}\\cvs\\{_yearFolder}\\{_monthFolder}";
            Directory.CreateDirectory(_cvFolderPath);
        }

        private void SaveAttachmentToTemporaryFile(MimeEntity attachment)
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
            string fileNamePath = GetAttachmentFileNamePath(_importCv.cvId, _importCv.fileExtension, out string cvKey);
            _importCv.cvKey = cvKey;
            File.Move(_importCv.tempFilePath, fileNamePath);
        }

        private void ExtractCvText()
        {
            if (_importCv.fileExtension == PDF_EXTENSION)
            {
                _importCv.cvTxt = CvParser.ExtractPdfTextByDocnetCore(_importCv.tempFilePath);
            }
            else
            {
                _importCv.cvTxt = CvParser.ExtractWordText(_importCv.tempFilePath);
            }
        }

        private void ExtractCvProps()
        {
            GetCandidateEmail();

            if (string.IsNullOrWhiteSpace(_importCv.firstName) || string.IsNullOrWhiteSpace(_importCv.lastName))
            {
                string fullName = ExtractCandidateName.GetName(_importCv.cvTxt);

                var (firstName, lastName) = ExtractCandidateName.SplitName(fullName);

                _importCv.firstName = firstName;
                _importCv.lastName = lastName;
                _importCv.candidateName = fullName;
            }

            GetCandidatePhone();
            GetCandidateCity();
        }

        private bool CheckIsBlackCand()
        {
            bool isBlackCand = false;
            blackCandModel? blackCand = null;

            if (!string.IsNullOrEmpty(_importCv.emailAddress))
            {
                blackCand = _blackCandidatesList.FirstOrDefault(x => (x.email ?? "").ToLower() == _importCv.emailAddress.ToLower());
            }
            else if (!string.IsNullOrEmpty(_importCv.phone))
            {
                blackCand = _blackCandidatesList.FirstOrDefault(x => x.phone == _importCv.phone);
            }

            if (blackCand != null)
            {
                isBlackCand = true;
                blackCand.cvs_count = blackCand.cvs_count + 1;
                Task.Run(() => _candsServise.UpdateBlackCandidateEmailCount(blackCand));
                Task.Run(() => _candsServise.UpdateCandLastCvSent(blackCand.candidate_id, DateTime.Now));
            }

            return isBlackCand;
        }

        private async Task CheckIsCvDuplicateOrSameCv()
        {
            if (!_importCv.isNewCandidate)
            {
                List<CandCvModel> cvs = await _candsServise.GetCandCvsList(_importCv.companyId, _importCv.candidateId);

                if (cvs.Count > 0)
                {
                    _importCv.isDuplicate = true;
                    bool isSubjectduplicate = false;

                    foreach (var cv in cvs)
                    {
                        if (cv.emailSubject == _importCv.subject && cv.cvSent >= DateTime.Now.AddDays(-30))
                        {
                            isSubjectduplicate = true;
                            break;
                        }
                    }

                    if (isSubjectduplicate)
                    {
                        cvs_txt? cvTxt = await _candsServise.CheckIsSameCv(_importCv.companyId, _importCv.candidateId, _importCv.cvAsciiSum);

                        if (cvTxt != null)
                        {
                            _importCv.isSameCvEmailSubject = true;
                            _importCv.cvId = cvTxt.cv_id;
                        }
                    }
                }
            }
        }

        private void ParseEmailSubject()
        {
            if (_parsersRules != null && _parsersRules.Count > 0)
            {
                List<int> parsersIds = _parsersRules.DistinctBy(x => x.parser_id).Select(x => x.parser_id).ToList();
                string seperator = "~~";

                foreach (int id in parsersIds)
                {
                    List<ParserRulesModel> parserRules = _parsersRules.Where(x => x.parser_id == id).OrderBy(x => x.order).ToList();
                    string subject = _importCv.subject;
                    bool isCorrectParser = false;

                    string firstDelimiter = parserRules[0].delimiter;

                    if (subject.Contains(firstDelimiter) && firstDelimiter == subject.Substring(0, firstDelimiter.Length))
                    {
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
                            string newSubject = _importCv.subject;
                            string[] subjectArr = subject.Split(seperator);
                            subjectArr = subjectArr.Skip(1).ToArray();//remove first entry, we take the value after the seperator.

                            for (int i = 0; i < subjectArr.Length; i++)
                            {
                                string delimiterStr = parserRules[i].delimiter;

                                switch (parserRules[i].value_type)
                                {
                                    case nameof(ParserValueType.Name):
                                        //to be check
                                        var nameParts = subjectArr[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                        _importCv.firstName = nameParts[nameParts.Length - 1].Trim();
                                        _importCv.lastName = subjectArr[i].Replace(_importCv.firstName, "").Trim();
                                        _importCv.candidateName = subjectArr[i];
                                        newSubject = newSubject.Replace(delimiterStr, "");
                                        break;
                                    case nameof(ParserValueType.Position):
                                        _importCv.positionRelated = subjectArr[i].Trim();
                                        newSubject = newSubject.Replace(delimiterStr, " - ");
                                        break;
                                    case nameof(ParserValueType.CompanyType):
                                        break;
                                    case nameof(ParserValueType.Address):
                                        break;
                                    default:
                                        break;
                                }
                            }

                            _importCv.subject = newSubject;
                        }
                    }
                }
            }
        }

        private async Task CandidateFindOrCreate()
        {
            await _candsServise.AddUpdateCandidateFromCvImport(_importCv);
        }

        private async Task AddCvToDb()
        {
            
            if (!string.IsNullOrEmpty(_importCv.positionRelated))
            {
                int? posTypeId = await _positionsServise.GetPositionTypeId(_importCv.companyId, _importCv.positionRelated);

                if (posTypeId == null)
                {
                    posTypeId = await _positionsServise.AddPositionTypeName(_importCv.companyId, _importCv.positionRelated);
                }

                _importCv.positionTypeId = posTypeId;

            }

            _importCv.cvId = await _candsServise.AddCv(_importCv);
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
            string text = _importCv.cvTxt;
            var tokens = Regex.Split(text, @"\s+");

            Regex emailCore = new Regex(
                @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}",
                RegexOptions.IgnoreCase);

            foreach (var token in tokens)
            {
                if (!token.Contains("@"))
                    continue;

                string email = null;

                var match = emailCore.Match(token);
                if (match.Success)
                {
                    email = match.Value;
                    email = Regex.Replace(email, @"^\d+", "");
                    email = Regex.Replace(email, @"\d+$", "");
                }
                else
                {
                    email = TryExtractReversedEmail(token);
                }

                if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                    continue;

                _importCv.emailAddress = email.ToLowerInvariant();
                break;
            }
        }

        /// <summary>
        /// Handles rare bidi/RTL extraction artifacts where the email segments
        /// come out reversed, e.g. "com.gmail@ariel881" -> "ariel881@gmail.com".
        /// </summary>
        private string TryExtractReversedEmail(string token)
        {
            var reversedEmail = new Regex(
                @"^(?<tld>[A-Za-z]{2,6})\.(?<domain>[A-Za-z0-9-]+)@(?<local>[A-Za-z0-9._%+-]+)$",
                RegexOptions.IgnoreCase);

            var match = reversedEmail.Match(token);
            if (!match.Success)
                return null;

            var local = match.Groups["local"].Value;
            var domain = match.Groups["domain"].Value;
            var tld = match.Groups["tld"].Value;

            local = Regex.Replace(local, @"^\d+", "").TrimEnd('.', ',', ':', ';');

            if (string.IsNullOrEmpty(local))
                return null;

            return $"{local}@{domain}.{tld}";
        }


        private void GetCandidateCity()
        {
            if (_importCv.body != null)
            {
                var ind0 = _importCv.body.IndexOf("<!--candidate details-->");
                var ind1 = _importCv.body.IndexOf("<!--candidate details-->", ind0 + 1);

                if (ind0 > -1 && ind1 > -1)
                {
                    var candDetails = _importCv.body.Substring(ind0, ind1);
                    ind0 = candDetails.IndexOf("<strong>", candDetails.IndexOf("<strong>", 0) + 1);

                    if (ind0 > -1)
                    {
                        candDetails = candDetails.Substring(ind0 + 8);
                        ind1 = candDetails.IndexOf("</strong>", 0);
                        if (ind1 > -1)
                        {
                            _importCv.city = System.Net.WebUtility.HtmlDecode(candDetails.Substring(0, ind1));
                        }
                    }
                }
            }
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
            cvKey = $"{_companyFolder}-{_yearFolder}{_monthFolder}{_importCv.fileTypeKey}-{cvId}";

            string fileName = $"{_companyFolder}-{_yearFolder}{_monthFolder}-{cvId}{fileExtension}";

            //string fileName = cvKey + fileExtension;
            var fileNamePath = $"{_filesRootFolder}\\_{_companyFolder}\\cvs\\{_yearFolder}\\{_monthFolder}\\{fileName}";
            return fileNamePath;
        }

        private void addEventLogEntry(Exception ex, int id)
        {
            string cvData = "";

            if (_importCv != null)
            {
                cvData = @$"Import CVs from Gmail failed,  Exception Row: {_importCv.exceptionRow}, candidateId: {_importCv.candidateId}, cvId: {_importCv.cvId}, 
                                        candName: {_importCv.firstName} {_importCv.lastName}, emailId: {_importCv.emailId}, ";
            }

            EventViewerWriter.ErrorMessage(cvData + ex.ToString());

        }

    }
}
