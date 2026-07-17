using CvFilesLibrary;
using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using EmailsLibrary;
using LuceneLibrary;
using Newtonsoft.Json;
using QueueLibrary;

namespace CandsPositionsLibrary
{
    public class CandsServise : ICandsServise
    {
        private IPositionsQueries _cvsPositionsQueries;
        private ICandsCvsQueries _candsCvsQueries;
        private ILuceneSearchService _luceneSearchService;
        private IEmailService _emailService;
        private IEmailQueries _emailQueries;
        private ICvsFilesService _cvsFilesService;
        private IDbQueueService _queueService;
        private IBlackCandQueries _blackCandQueries;

        public CandsServise(IPositionsQueries cvsPositionsQueries,
            ICandsCvsQueries candsCvsQueries,
            ILuceneSearchService luceneSearchService,
            IEmailService emailService, IEmailQueries emailQueries,
            ICvsFilesService cvsFilesService, IDbQueueService queueService,
            IBlackCandQueries blackCandQueries)
        {
            _cvsPositionsQueries = cvsPositionsQueries;
            _candsCvsQueries = candsCvsQueries;
            _luceneSearchService = luceneSearchService;
            _emailService = emailService;
            _emailQueries = emailQueries;
            _cvsFilesService = cvsFilesService;
            _queueService = queueService;
            _blackCandQueries = blackCandQueries;
        }

        public async Task AddBlackCand(blackCandModel blackCand)
        {
            await _blackCandQueries.AddBlackCand(blackCand);
        }

        public async Task RemoveBlackCand(int candidateId)
        {
            await _blackCandQueries.RemoveBlackCand(candidateId);
        }

        public async Task<int> AddCv(ImportCvModel importCv)
        {
            return await _candsCvsQueries.AddCv(importCv);
        }

        public async Task DeleteCv(int companyId, int candidateId, int cvId)
        {
            await _candsCvsQueries.DeleteCv(companyId, candidateId, cvId);
            await _cvsPositionsQueries.UpdateCandPosArrays(companyId, candidateId);

            var tuple = await _candsCvsQueries.GetCandLastCv(companyId, candidateId);
            cv? lastCv = tuple.Item1;
            bool isDuplicate = tuple.Item2;

            if (lastCv != null)
            {
                await _candsCvsQueries.UpdateCandLastCv(companyId, candidateId, lastCv.id, isDuplicate, lastCv.date_created);
            }
            else
            {
                await DeleteCandidate(companyId, candidateId);
            }
        }

        public async Task DeleteCandidate(int companyId, int candidateId)
        {
            await _candsCvsQueries.DeleteCandidate(companyId, candidateId);
        }

        public async Task UpdateCvKeyId(ImportCvModel importCv)
        {
            await _candsCvsQueries.UpdateCvKeyId(importCv);
        }

        public async Task<candidate?> GetCandidateByEmail(string email)
        {
            return await _candsCvsQueries.GetCandidateByEmail(email);
        }

        public async Task<candidate?> GetCandidateByPhone(string phone)
        {
            return await _candsCvsQueries.GetCandidateByPhone(phone);
        }

        public async Task AddUpdateCandidateFromCvImport(ImportCvModel importCv)
        {
            candidate? cand = null;

            if (!string.IsNullOrEmpty(importCv.emailAddress))
            {
                cand = await GetCandidateByEmail(importCv.emailAddress);
            }
            else if (!string.IsNullOrEmpty(importCv.phone))
            {
                cand = await GetCandidateByPhone(importCv.phone);
            }

            if (cand != null)
            {
                importCv.candidateId = cand.id;

                bool isUpdate = false;

                if (string.IsNullOrEmpty(cand.first_name))
                {
                    cand.first_name = importCv.firstName.Trim();
                    isUpdate = true;
                }

                if (string.IsNullOrEmpty(cand.last_name))
                {
                    cand.last_name = importCv.lastName.Trim();
                    isUpdate = true;
                }

                if (string.IsNullOrEmpty(cand.city) && importCv.city != null)
                {
                    cand.city = importCv.city.Trim();
                    isUpdate = true;
                }

                if (isUpdate)
                {
                    await _candsCvsQueries.UpdateCandidate(cand);
                }
            }

            if (importCv.candidateId == 0)
            {
                string email = importCv.emailAddress.Length > 0 ? importCv.emailAddress : "";

                var newCand = new candidate
                {
                    company_id = importCv.companyId,
                    email = email,
                    phone = importCv.phone,
                    city = importCv.city,
                    first_name = importCv.firstName,
                    last_name = importCv.lastName
                };

                importCv.isNewCandidate = true;
                importCv.candidateId = await _candsCvsQueries.AddCandidate(newCand);
            }
        }

        public async Task<CandModel?> GetCandidate(int companyId, int candId)
        {
            return await _candsCvsQueries.GetCandidate(companyId, candId);
        }

        public async Task<List<CandCvModel>> GetCandCvsList(int companyId, int candidateId)
        {
            return await _candsCvsQueries.GetCandCvsList(companyId, candidateId);
        }

        public async Task<CvModel?> GetCv(int cvId, int companyId)
        {
            return await _cvsPositionsQueries.GetCv(cvId, companyId);
        }

        public async Task<cvs_txt?> CheckIsSameCv(int companyId, int candidateId, int cvAsciiSum)
        {
            return await _cvsPositionsQueries.CheckIsSameCv(companyId, candidateId, cvAsciiSum);
        }

        public async Task UpdateCandLastCv(int companyId, int candidateId, int cvId, bool isDuplicate, DateTime lastCvSent)
        {
            await _candsCvsQueries.UpdateCandLastCv(companyId, candidateId, cvId, isDuplicate, lastCvSent);
        }

        public async Task UpdateCvDate(int cvId)
        {
            await _candsCvsQueries.UpdateCvDate(cvId);
        }

        public async Task AttachPosCandCv(AttachePosCandCvModel posCv)
        {
            await _cvsPositionsQueries.AttachPosCandCv(posCv);
            await _cvsPositionsQueries.UpdateCandPosArrays(posCv.companyId, posCv.candidateId);
        }

        public async Task DetachPosCand(AttachePosCandCvModel posCv)
        {
            await _cvsPositionsQueries.DetachPosCand(posCv);
            await _cvsPositionsQueries.UpdateCandPosArrays(posCv.companyId, posCv.candidateId);
        }

        public async Task UpdateCandPositionStatus(CandPosStageTypeUpdateModel posStatus)
        {
            await _cvsPositionsQueries.UpdateCandPositionStatus(posStatus);
            await _cvsPositionsQueries.UpdateCandPosArrays(posStatus.companyId, posStatus.candidateId);
        }

        public async Task UpdatePosStageDate(CandPosStageTypeUpdateModel posStatus)
        {
            await _cvsPositionsQueries.UpdatePosStageDate(posStatus);
            await _cvsPositionsQueries.UpdateCandPosArrays(posStatus.companyId, posStatus.candidateId);
        }

        public async Task RemovePosStage(CandPosStageTypeUpdateModel posStatus)
        {
            await _cvsPositionsQueries.RemovePosStage(posStatus);
            await _cvsPositionsQueries.UpdateCandPosArrays(posStatus.companyId, posStatus.candidateId);
        }

        public async Task<List<company_cvs_email>> GetCompaniesEmails()
        {
            return await _cvsPositionsQueries.GetCompaniesEmails();
        }

        public async Task<List<SearchEntry>> SearchCands(int companyId, searchCandCvModel searchVals)
        {
            return await _luceneSearchService.Search(companyId, searchVals);
        }

        public async Task<List<SearchEntry>> SearchForAiFilter( searchCandCvModel searchVals)
        {
            return await _luceneSearchService.SearchForAiFilter( searchVals);
        }

        public async Task UpdateCvsAsciiSum(int companyId)
        {
            await _candsCvsQueries.UpdateCvsAsciiSum(companyId);
        }

        public async Task<List<CandPosStageTypeModel>> GetCandPosStagesTypes(int companyId)
        {
            return await _cvsPositionsQueries.GetCandPosStagesTypes(companyId);
        }

        public async Task SendEmail(SendEmailModel emailData, UserModel? user)
        {
            if (user != null)
            {
                List<AttachmentModel> Attachments = new List<AttachmentModel>();

                if (emailData.attachCvs != null)
                {
                    foreach (var item in emailData.attachCvs)
                    {
                        var Attachment = _cvsFilesService.AddPdfLogo(emailData.companyId, item.cvKey);
                        Attachments.Add(new AttachmentModel { Attachment = Attachment, name = item.name });
                    }
                }

                var email = new EmailModel
                {
                    To = emailData.toAddresses,
                    Subject = emailData.subject,
                    Body = $"{emailData.body} {user.signature} ",
                    Attachments = Attachments,
                    From = new EmailAddress { Address = user.email, Name = $"{user.firstName} {user.lastName}" },
                    MailSenderUserName = user.mailUsername,
                    MailSenderPassword = user.mailPassword,
                };

                await _emailService.Send(email);
            }
        }

        public async Task SaveCandReview(int companyId, CandReviewModel candReview)
        {
            await _cvsPositionsQueries.SaveCandReview(companyId, candReview);
            await _queueService.EnqueueAsync("index cv", candReview.candidateId.ToString());
        }

        public async Task<List<EmailTemplateModel>> GetEmailTemplates(int companyId)
        {
            return await _cvsPositionsQueries.GetEmailTemplates(companyId);
        }

        public async Task AddUpdateEmailTemplate(EmailTemplateModel emailTemplate)
        {
            await _cvsPositionsQueries.AddUpdateEmailTemplate(emailTemplate);
        }

        public async Task DeleteEmailTemplate(int companyId, int id)
        {
            await _cvsPositionsQueries.DeleteEmailTemplate(companyId, id);
        }

        public async Task UpdateCandDetails(CandDetailsModel candDetails)
        {
            await _cvsPositionsQueries.UpdateCandDetails(candDetails);
            await _queueService.EnqueueAsync("index cv", candDetails.candidateId.ToString());
        }

        public async Task UpdateIsSeen(int companyId, int cvId)
        {
            await _cvsPositionsQueries.UpdateIsSeen(companyId, cvId);
        }

        public async Task<List<CandReportModel?>> CandsReport(int companyId, string stageType)
        {
            return await _cvsPositionsQueries.CandsReport(companyId, stageType);
        }

        public async Task AddSendEmail(SendEmailModel emailData, int userId)
        {
            await _cvsPositionsQueries.AddSendEmail(emailData, userId);
        }

        public async Task SaveCustomerCandReview(int companyId, CandReviewModel customerCandReview)
        {
            await _cvsPositionsQueries.SaveCustomerCandReview(companyId, customerCandReview);
            await _cvsPositionsQueries.UpdateCandCustomersReviews(companyId, customerCandReview.candidateId);
        }

        public async Task<List<SearchModel>> GetSearches(int companyId)
        {
            return await _cvsPositionsQueries.GetSearches(companyId);
        }

        public async Task SaveSearch(int companyId, SearchModel searchVals)
        {
            await _cvsPositionsQueries.SaveSearch(companyId, searchVals);
        }

        public async Task StarSearch(int companyId, SearchModel searchVals)
        {
            await _cvsPositionsQueries.StarSearch(companyId, searchVals);
        }

        public async Task DeleteSearch(int companyId, SearchModel searchVals)
        {
            await _cvsPositionsQueries.DeleteSearch(companyId, searchVals);
        }

        public async Task DeleteAllNotStarSearches(int companyId)
        {
            await _cvsPositionsQueries.DeleteAllNotStarSearches(companyId);
        }

        public async Task<List<keywordsGroupModel>> GetKeywordsGroups(int companyId)
        {
            return await _cvsPositionsQueries.GetKeywordsGroups(companyId);
        }

        public async Task SaveKeywordsGroup(int companyId, keywordsGroupModel keywordsGroup)
        {
            await _cvsPositionsQueries.SaveKeywordsGroup(companyId, keywordsGroup);
        }

        public async Task DeleteKeywordsGroup(int companyId, int id)
        {
            await _cvsPositionsQueries.DeleteKeywordsGroup(companyId, id);
        }

        public async Task<List<keywordModel>> GetKeywords(int companyId)
        {
            return await _cvsPositionsQueries.GetKeywords(companyId);
        }

        public async Task SaveKeyword(int companyId, keywordModel keyword)
        {
            await _cvsPositionsQueries.SaveKeyword(companyId, keyword);
        }

        public async Task DeleteKeyword(int companyId, int id)
        {
            await _cvsPositionsQueries.DeleteKeyword(companyId, id);
        }

        public async Task<List<blackCandModel>> GetBlackCandidatesList()
        {
            return await _cvsPositionsQueries.GetBlackCandidatesList();
        }

        public async Task UpdateBlackCandidateEmailCount(blackCandModel blackCand)
        {
            await _cvsPositionsQueries.UpdateBlackCandidateEmailCount(blackCand);
        }

        public List<CandModel> MergeAiResultsWithCandsList(List<CandModel> candsList, List<AiCandidateSearchModel> aiResults)
        {
            var candsDict = candsList
                .Where(x => x != null)
                .ToDictionary(x => x.candidateId);

            var result = new List<CandModel>();

            foreach (var aiItem in aiResults)
            {
                if (!candsDict.TryGetValue(aiItem.candidateId, out var cand))
                    continue;

                cand.NameAI = aiItem.name;
                cand.LocationAI = aiItem.city;
                cand.JobsTitlesAI = aiItem.jobsTitlesHe.ToArray();
                cand.EstimateAgeAI = aiItem.age;
                cand.EducationAI = aiItem.education == null ? null : JsonConvert.DeserializeObject<EducationItemModel[]>(aiItem.education);
                cand.CompaniesAI = aiItem.companies.ToArray();
                cand.SummaryAI = aiItem.summary;
                cand.score = aiItem.score;
                result.Add(cand);
            }

            return result;
        }
    }
}
