using Amazon.Runtime.Internal.Util;
using CandsPositionsLibrary;
using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using GeneralLibrary;
using Google.Protobuf;
using ImportCvsLibrary.RegularExpressions;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Spire.Doc;
using Spire.Pdf;
using Spire.Pdf.Exporting.Text;
using Spire.Pdf.Texts;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static DataModelsLibrary.GlobalConstant;

namespace ImportCvsLibrary
{
    public partial class ImportCvs : IImportCvs
    {

        ICandsPositionsServise _cvsPositionsServise;
        string _filesRootFolder;
        string _cvupNotBackedUpRootFolder;
        string _gmailUserName;
        string _mailPassword;
        string _cvFolderPath = "";
        string _cvTempFolderPath = "";
        string _yearFolder = "";
        string _monthFolder = "";
        string _companyFolder = "";
        //List<company_cvs_email>? _companiesEmail;
        List<ParserRulesModel> _parsersRulesAllCompanies;
        List<ParserRulesModel> _parsersRules;
        ImportCvModel _importCv = new ImportCvModel();

        public ImportCvs(IConfiguration config, ICandsPositionsServise cvsPositionsServise)
        {
            _cvsPositionsServise = cvsPositionsServise;

            _filesRootFolder = config["GlobalSettings:CvUpFilesRootFolder"];
            _cvupNotBackedUpRootFolder = $"{config["GlobalSettings:CvUp-not-backed-up-Root-Folder"]}";

            //Directory.CreateDirectory(_filesRootFolder);
            _gmailUserName = config["GlobalSettings:ImportGmailUserName"];
            _mailPassword = config["GlobalSettings:ImportGmailPassword"];
        }

        public async Task ImportFromGmail()
        {
            IList<UniqueId>? uids = null;
            IMailFolder? inbox = null;

            _parsersRulesAllCompanies = await _cvsPositionsServise.GetParsersRules();

            using (var client = new ImapClient())
            {
                try
                {
                    client.Connect("imap.gmail.com", 993, true);
                    client.Authenticate(_gmailUserName, _mailPassword);

                    inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadWrite);
                    uids = client.Inbox.Search(SearchQuery.NotSeen);
                }
                catch (Exception)
                {
                    if (client != null)
                    {
                        client.Disconnect(true);
                        client.Dispose();
                    }

                    return;
                }

                if (client != null && client.IsConnected)
                {
                    foreach (var uid in uids)
                    {
                        try
                        {
                            var message = client.Inbox.GetMessage(uid);

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

                                            SaveAttachmentToTemporaryFile(part);
                                            ParseEmailSubject();
                                            await CvExtractDataAndSave();
                                        }
                                    }
                                }

                                inbox.SetFlags(uid, MessageFlags.Seen, true);
                            }
                        }
                        catch (Exception ex)
                        {
                            client.Disconnect(true);
                            client.Dispose();

                            addEventLogEntry(ex);
                            Console.WriteLine(ex.ToString());
                            inbox.SetFlags(uid, MessageFlags.Seen, true);
                        }
                    }

                    client.Disconnect(true);
                }
            }
        }

        private void addEventLogEntry(Exception ex)
        {
            using (EventLog eventLog = new())
            {
                if (!EventLog.SourceExists("CvUpImport"))
                {
                    EventLog.CreateEventSource("CvUpImport", "CvUpImport");
                }

                string cvData = "";

                if (_importCv != null)
                {
                    cvData = @$"candidateId: {_importCv.candidateId}, cvId: {_importCv.cvId}, 
                                        candName: {_importCv.firstName} {_importCv.lastName}, emailId: {_importCv.emailId}, ";
                }

                eventLog.Source = "CvUpImport";
                eventLog.WriteEntry(cvData + ex.ToString(), EventLogEntryType.Information);
            }
        }


        private async Task CvExtractDataAndSave()
        {
            ExtractCvProps();
            await CandidateFindOrCreate();
            GetCvAsciiSum();
            await CheckIsCvDuplicateOrSameCv();

            if (_importCv.isSameCvEmailSubject)
            {
                await _cvsPositionsServise.UpdateCvDate(_importCv.cvId);
            }
            else
            {
                //await AddPositionName();
                await AddCvToDb();
                RenameAndMoveAttachmentToFolder();
                await _cvsPositionsServise.UpdateCvKeyId(_importCv);
                await _cvsPositionsServise.UpdateCandLastCv(_importCv.companyId, _importCv.candidateId, _importCv.cvId, _importCv.isDuplicate, _importCv.dateCreated);
                await _cvsPositionsServise.SaveCandidateToIndex(_importCv.companyId, _importCv.candidateId);
                await AddCandToMatchPosition();
            }
        }

        private async Task AddCandToMatchPosition()
        {
            position? matchPos = await _cvsPositionsServise.GetPositionByMatchStr(_importCv.companyId, _importCv.subject);

            if (matchPos != null)
            {
                await _cvsPositionsServise.AttachPosCandCv(new AttachePosCandCvModel
                {
                    positionId = matchPos.id,
                    candidateId = _importCv.candidateId,
                    companyId = _importCv.companyId,
                    cvId = _importCv.cvId,
                    keyId = _importCv.cvKey
                });
            }
        }


        //private async Task UpdateCandidateLastCv()
        //{
        //    await _cvsPositionsServise.UpdateCandLastCv(_importCv.companyId, _importCv.candidateId, _importCv.cvId, _importCv.isDuplicate, _importCv.dateCreated);
        //}

        //private async Task UpdateCvKeyId()
        //{
        //    if (!_importCv.isSameCv)
        //    {
        //        await _cvsPositionsServise.UpdateCvKeyId(_importCv);
        //    }
        //}

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
            GetCandidateCity();
        }

        private async Task CheckIsCvDuplicateOrSameCv()
        {
            if (!_importCv.isNewCandidate)
            {
                List<CandCvModel> cvs = await _cvsPositionsServise.GetCandCvsList(_importCv.companyId, _importCv.candidateId);

                if (cvs.Count > 0)
                {
                    _importCv.isDuplicate = true;
                    bool isSubjectduplicate = false;

                    foreach (var cv in cvs)
                    {
                        if (cv.emailSubject == _importCv.subject)
                        {
                            isSubjectduplicate = true;
                            break;
                        }
                    }

                    if (isSubjectduplicate)
                    {
                        cvs_txt? cvTxt = await _cvsPositionsServise.CheckIsSameCv(_importCv.companyId, _importCv.candidateId, _importCv.cvAsciiSum);

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
            if (_parsersRules.Count > 0)
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
            await _cvsPositionsServise.AddUpdateCandidateFromCvImport(_importCv);
        }

        private async Task AddCvToDb()
        {
            //if (_importCv.isSameCv)
            //{
            //    await _cvsPositionsServise.UpdateCvDate(int cvId);
            //}
            //else
            //{
            if (!string.IsNullOrEmpty(_importCv.positionRelated))
            {
                int? posTypeId = await _cvsPositionsServise.GetPositionTypeId(_importCv.companyId, _importCv.positionRelated);

                if (posTypeId == null)
                {
                    posTypeId = await _cvsPositionsServise.AddPositionTypeName(_importCv.companyId, _importCv.positionRelated);
                }

                _importCv.positionTypeId = posTypeId;

            }

            _importCv.cvId = await _cvsPositionsServise.AddCv(_importCv);
            //}
        }

        //private async Task AddCvToIndex()
        //{
        //    await _cvsPositionsServise.SaveCandidateToIndex(_importCv.companyId, _importCv.candidateId);
        //}

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

        private void GetCandidateCity()
        {
            if (_importCv.body != null)
            {
                var ind0 = _importCv.body.IndexOf("<!--candidate details-->");
                var ind1 = _importCv.body.IndexOf("<!--candidate details-->", ind0 + 1);
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
            cvKey = $"{_companyFolder}-{_yearFolder}{_monthFolder}{_importCv.fileTypeKey}-{cvId}";

            string fileName = $"{_companyFolder}-{_yearFolder}{_monthFolder}-{cvId}{fileExtension}";

            //string fileName = cvKey + fileExtension;
            var fileNamePath = $"{_filesRootFolder}\\_{_companyFolder}\\cvs\\{_yearFolder}\\{_monthFolder}\\{fileName}";
            return fileNamePath;
        }

        #region Candidate Name

        private void GetCandidateName()
        {
            string[] cvWords = _importCv.cvTxt.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string firstName = "";
            string lastName1 = "";
            string lastName2 = "";


            for (int i = 0; i < cvWords.Length - 10; i++)
            {
                firstName= cvWords[i];
                lastName1 = cvWords[i + 1];

                if (isCanBeFirstOrLastName(firstName) && isCanBeFirstOrLastName(lastName1))
                {
                    

                    if (isCanBeFirstOrLastName(cvWords[i + 2]))
                    {
                        lastName2 = cvWords[i + 2];
                    }
                    else if (isCanBeFirstOrLastName(cvWords[i + 3]))
                    {
                        lastName2 = cvWords[i + 3];
                    }
                }
                
            }

            string hebChars = Rx.RemoveNoneHebChars(_importCv.cvTxt);

            if (hebChars.Length > 50)
            {

            }
            else
            {

            }

            //string name;
            //List<string> lines;

            //lines = getCvLines(_importCv.cvTxt);
            //if (lines.Count < 6)//cv contains less then 5 rows
            //{
            //    return;
            //}

            //name = findNameByPrefixPattern(lines);

            //if (name == string.Empty)
            //{
            //    name = findNameByNamesList(lines);
            //}


        }

        private void potentialLastNames(string lastName1, string lastName2, string lastName3)
        {
            string lastName = "";

            if (isCanBeFirstOrLastName(lastName1))
            {
                if (isCanBeFirstOrLastName(lastName2))
                {

                }
                else if (isCanBeFirstOrLastName(lastName3))
                {

                }
            }
            else if (isCanBeFirstOrLastName(lastName2))
            {

            }
        }

        private bool isCanBeFirstOrLastName(string name)
        {
            bool isAble = true;

            if (name.Length < 2)
            {
                isAble = false;
            }

            return isAble;
        }

        //private string findNameByPrefixPattern(List<string> lines)
        //{
        //    //שם: שמלא
        //    //שם: שמפרטי
        //    //משפחה: שמשפחה
        //    //שם פרטי: שמפרטי
        //    //שם משפחה: שמשפחה

        //    string line, name = "";

        //    for (int i = 0; i < 6; i++)
        //    {
        //        name = "";
        //        line = lines[i];
        //        line = Rx.MultipleSpacesToOneSpace(line);
        //        line = Rx.RemoveEnglishLetters(line);
        //        line = Rx.RemoveSpacesBeforeColon(line);

        //        if (line.IndexOf("שם:") > -1)
        //        {
        //            line = line.Replace("שם:", "");
        //            name = getNameFromString(line);
        //            break;
        //        }
        //        else if (line.IndexOf("שם מלא") > -1)
        //        {
        //            line = line.Replace("שם מלא", "");
        //            name = getNameFromString(line);
        //            break;
        //        }
        //        else if (line.IndexOf("שם ומשפחה") > -1)
        //        {
        //            line = line.Replace("שם ומשפחה", "");
        //            name = getNameFromString(line);
        //            break;
        //        }
        //        else if (line.IndexOf("שם פרטי:") > -1)
        //        {

        //        }
        //        else if (line.IndexOf("משפחה") > -1)
        //        {

        //        }
        //        else if (line.IndexOf("שם") > -1)
        //        {
        //            line = line.Replace("שם", "");
        //            name = getNameFromString(line);
        //            break;
        //        }
        //    }

        //    return name;
        //}

        //private string findNameByNamesList(List<string> lines)
        //{
        //    string name1 = "", name2 = "", name3 = "", name4 = "";

        //    for (int i = 0; i < 6; i++)
        //    {
        //        if (name1 != "" && name2 != "")
        //        {
        //            if (name3 != "")
        //            {
        //                return name1 + " " + name2 + " " + name3;
        //            }
        //            else
        //            {
        //                return name1 + " " + name2;
        //            }
        //        }

        //        name1 = "";

        //        var lineTxt = Rx.ReplaceNoneHebNameCharsToSpace(lines[i]);
        //        lineTxt = lineTxt.Replace("קורות חיים", " ");
        //        lineTxt = lineTxt.Replace("תז", " ");

        //        var lineSplited = lineTxt.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        //        foreach (var name in lineSplited)
        //        {
        //            if (UniqueCandNames.Contains(rp(name)))
        //            {
        //                if (name1 == "")
        //                {
        //                    name1 = name;
        //                }
        //                else if (name2 == "")
        //                {
        //                    name2 = name;
        //                }
        //                else if (name3 == "")
        //                {
        //                    name3 = name;
        //                }
        //                else if (name4 == "")
        //                {
        //                    name4 = name;
        //                    return name1 + " " + name2 + " " + name3 + " " + name4;
        //                }
        //            }
        //            else if (name1 != "" && name2 != "")
        //            {
        //                break;
        //            }
        //        }
        //    }

        //    return "";
        //}

        //private string getNameFromString(string line)
        //{
        //    string name = "";
        //    string[] lineArr;

        //    if (line.IndexOf(',') > -1)
        //    {
        //        line = line.Substring(0, line.IndexOf(','));
        //    }

        //    line = removeNotNamesWords(line);
        //    line = Rx.TrimNoneAlfabeit(line);

        //    line = Rx.RemoveSpacesBeforeAfterDash(line);//בן - ארי , בן-ארי
        //    lineArr = line.Split(new char[] { ' ' });

        //    if (lineArr.Length > 4)
        //    {
        //        name = "";
        //    }
        //    else
        //    {
        //        foreach (var item in lineArr)
        //        {
        //            if (Rx.IsHebrewCharsExist(item))
        //            {
        //                name += Rx.TrimNoneAlfabeit(item) + " ";
        //            }
        //        }
        //        name = name.Trim();
        //    }
        //    return name;
        //}

        //private List<string> getCvLines(string cvText)
        //{
        //    List<string> lines = new List<string>();

        //    using (System.IO.StringReader reader = new System.IO.StringReader(cvText))
        //    {
        //        string line;
        //        while ((line = reader.ReadLine()) != null)
        //        {
        //            if (line.Trim() != string.Empty)
        //            {
        //                lines.Add(line.Trim());
        //            }
        //        }
        //    }
        //    return lines;
        //}

        //private string removeNotNamesWords(string str)
        //{
        //    string newStr = "";
        //    str = str.Replace("\t", " ");
        //    str = str.Replace("\v", " ");
        //    str = str.Replace("-", "").Replace("\"", "");//בן-דוד TO בןדוד, באב"ד TO באבד

        //    foreach (var item in notNameMultiWordsBefore)
        //    {
        //        str = str.Replace(item, "");
        //    }

        //    foreach (var item in notNameMultiWordsAfter)
        //    {
        //        if (str.IndexOf(item) > -1)
        //        {
        //            str = str.Substring(0, str.IndexOf(item));
        //        }
        //    }

        //    //str = rx.RemovePunctuation(str);
        //    str = Rx.RemoveDigits(str);
        //    str = str.Trim();

        //    if (str.Length > 0)
        //    {
        //        string[] wordsArr = str.Split(new char[] { ' ' });

        //        for (int i = 0; i < wordsArr.Length; i++)
        //        {
        //            if (notNameWordsBefore.Contains(Rx.RemovePunctuation(wordsArr[i])))
        //            {
        //            }
        //            else
        //            {
        //                newStr += wordsArr[i] + " ";
        //            }
        //        }

        //        wordsArr = newStr.Trim().Split(new char[] { ' ' });
        //        newStr = "";

        //        for (int i = 0; i < wordsArr.Length; i++)
        //        {
        //            if (notNameWordsAfter.Contains(Rx.RemovePunctuation(wordsArr[i])))
        //            {
        //                for (int j = i; j < wordsArr.Length; j++)
        //                {
        //                    wordsArr[j] = "";
        //                }
        //            }
        //            else
        //            {
        //                newStr += wordsArr[i] + " ";
        //            }
        //        }
        //    }

        //    return newStr.Trim();
        //}

        #endregion

        //private int GetCompanyIdFromAddress(InternetAddressList addressList)
        //{
        //    foreach (var toEmail in addressList.ToList())
        //    {
        //        if (_companiesEmail != null)
        //        {
        //            var toAddress = toEmail.ToString();

        //            var companyEmail = _companiesEmail.Where(x => x.email == toAddress).FirstOrDefault();

        //            if (companyEmail != null)
        //            {
        //                return companyEmail.company_id;
        //            }
        //        }
        //    }

        //    return 0;
        //}
    }
}
