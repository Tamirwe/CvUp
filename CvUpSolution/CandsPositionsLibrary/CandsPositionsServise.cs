using CvFilesLibrary;
using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using EmailsLibrary;
using EmailsLibrary.Models;
using LuceneLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.Design;

namespace CandsPositionsLibrary
{
    public partial class CandsPositionsServise : ICandsPositionsServise
    {
        private IConfiguration _configuration;
        private ICandsPositionsQueries _cvsPositionsQueries;
        private ILuceneService _luceneService;
        private IEmailService _emailService;
        private IEmailQueries _emailQueries;
        private ICvsFilesService _cvsFilesService;

        public CandsPositionsServise(IConfiguration config, ICandsPositionsQueries cvsPositionsQueries, ILuceneService luceneService,
            IEmailService emailService, IEmailQueries emailQueries, ICvsFilesService cvsFilesService)
        {
            _configuration = config;
            _cvsPositionsQueries = cvsPositionsQueries;
            _luceneService = luceneService;
            _emailService = emailService;
            _emailQueries = emailQueries;
            _cvsFilesService = cvsFilesService;
        }

        public async Task<int> AddCv(ImportCvModel importCv)
        {
            return await _cvsPositionsQueries.AddCv(importCv);
        }

        public async Task DeleteCv(int companyId, int candidateId, int cvId)
        {
            await _cvsPositionsQueries.DeleteCv(companyId, candidateId, cvId);
            await _cvsPositionsQueries.UpdateCandPosArrays(companyId, candidateId);

            var tuple = await _cvsPositionsQueries.GetCandLastCv(companyId, candidateId);
            cv? lastCv = tuple.Item1;
            bool isDuplicate = tuple.Item2;

            if (lastCv != null)
            {
                await _cvsPositionsQueries.UpdateCandLastCv(companyId, candidateId, lastCv.id, isDuplicate, lastCv.date_created);
            }
        }

        public async Task DeleteCandidate(int companyId, int candidateId)
        {
            await _cvsPositionsQueries.DeleteCandidate(companyId, candidateId);
        }

        public async Task UpdateCvKeyId(ImportCvModel importCv)
        {
            await _cvsPositionsQueries.UpdateCvKeyId(importCv);
        }

        public async Task<candidate?> GetCandidateByEmail(string email)
        {
            return await _cvsPositionsQueries.GetCandidateByEmail(email);
        }

        public async Task<candidate?> GetCandidateByPhone(string phone)
        {
            return await _cvsPositionsQueries.GetCandidateByPhone(phone);
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

                if (cand != null)
                {
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
                        await _cvsPositionsQueries.UpdateCandidate(cand);
                    }
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
                importCv.candidateId = await _cvsPositionsQueries.AddCandidate(newCand);
            }
        }

        public async Task IndexCompanyCvs(int companyId)
        {
            List<CvsToIndexModel> cvPropsToIndexList = await _cvsPositionsQueries.GetCompanyCvsToIndex(companyId, 0);
            await _luceneService.CompanyIndexAddDocuments(companyId, cvPropsToIndexList, true);
        }

        public async Task<CandModel?> GetCandidate(int companyId, int candId)
        {
            var result = await _cvsPositionsQueries.GetCandidate(companyId, candId);
            return result;
        }

        public async Task<List<CandModel?>> GetCandsList(int companyId,  List<int>? candsIds)
        {
            var result = await _cvsPositionsQueries.GetCandsList(companyId,  candsIds);
            return result;
        }

        public async Task<List<CandModel?>> GetPosCandsList(int companyId, int positionId)
        {
            return await _cvsPositionsQueries.GetPosCandsList(companyId, positionId);
        }

        public async Task<List<CandModel?>> GetPosTypeCandsList(int companyId, int positionTypeId)
        {
            return await _cvsPositionsQueries.GetPosTypeCandsList(companyId, positionTypeId);
        }

        public async Task<List<CandModel?>> GetFolderCandsList(int companyId, int folderId)
        {
            return await _cvsPositionsQueries.GetFolderCandsList(companyId, folderId);
        }

        public async Task<List<CandCvModel>> GetCandCvsList(int companyId, int candidateId)
        {
            List<CandCvModel> cvsList = await _cvsPositionsQueries.GetCandCvsList(companyId, candidateId);
            return cvsList;
        }

        public async Task<List<int>> getPositionContactsIds(int companyId, int positionId)
        {
            List<int> posContacts = await _cvsPositionsQueries.getPositionContactsIds(companyId, positionId);
            return posContacts;
        }

        public async Task<PositionModel> GetPosition(int companyId, int positionId)
        {
            PositionModel pos = await _cvsPositionsQueries.GetPosition(companyId, positionId);
            return pos;
        }

        public async Task<int> AddPosition(PositionModel data, int companyId, int userId)
        {
            position newRec = await _cvsPositionsQueries.AddPosition(data, companyId, userId);
            await _cvsPositionsQueries.AddUpdateInterviewers(companyId, newRec.id, data.interviewersIds);
            await _cvsPositionsQueries.AddUpdatePositionContacts(companyId, newRec.id, data.contactsIds);
            return newRec.id;
        }

        public async Task<int> UpdatePosition(PositionModel data, int companyId, int userId)
        {
            position? updRec = await _cvsPositionsQueries.UpdatePosition(data, companyId, userId);
            //await _cvsPositionsQueries.AddUpdateInterviewers(companyId, data.id,data.interviewersIds);
            await _cvsPositionsQueries.AddUpdatePositionContacts(companyId, data.id, data.contactsIds);
            return data.id;
        }

        public async Task<List<PositionModel>> GetPositionsList(int companyId)
        {
            List<PositionModel> qList = await _cvsPositionsQueries.GetPositionsList(companyId);
            return qList;
        }

        public async Task DeletePosition(int companyId, int id)
        {
            await _cvsPositionsQueries.DeletePosition(companyId, id);
        }

        public async Task<List<ParserRulesModel>> GetParsersRules()
        {
            return await _cvsPositionsQueries.GetParsersRules();
        }

        public async Task<CvModel?> GetCv(int cvId, int companyId)
        {
            return await _cvsPositionsQueries.GetCv(cvId, companyId);
        }

        public async Task<cvs_txt?> CheckIsSameCv(int companyId, int candidateId, int cvAsciiSum)
        {
            cvs_txt? cvTxt = await _cvsPositionsQueries.CheckIsSameCv(companyId, candidateId, cvAsciiSum);
            return cvTxt;
        }

        public async Task UpdateCandLastCv(int companyId, int candidateId, int cvId, bool isDuplicate, DateTime lastCvSent)
        {
            await _cvsPositionsQueries.UpdateCandLastCv(companyId, candidateId, cvId, isDuplicate, lastCvSent);
        }

        public async Task UpdateCvDate(int cvId)
        {
            await _cvsPositionsQueries.UpdateCvDate(cvId);
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

        public async Task UpdateCandPositionStatus(CandPosStatusUpdateCvModel posStatus)
        {
            await _cvsPositionsQueries.UpdateCandPositionStatus(posStatus);
            await _cvsPositionsQueries.UpdateCandPosArrays(posStatus.companyId, posStatus.candidateId);

            //cand_pos_stage? posSatge = await _cvsPositionsQueries.getPosStage(posStatus.companyId, posStatus.stageType);

            //if (posSatge != null)
            //{
            //    switch (posSatge.stage_event)
            //    {
            //        case "call_email_to_candidate":
            //            await _cvsPositionsQueries.updateCandPosCallEmailToCandidate(posStatus.companyId, posStatus.candidateId, posStatus.positionId);
            //            break;
            //        case "email_to_customer":
            //            await _cvsPositionsQueries.updateCandPosEmailToCustomer(posStatus.companyId, posStatus.candidateId, posStatus.positionId);
            //            break;
            //        case "reject_email_to_candidate":
            //            await _cvsPositionsQueries.updateCandPosRejectEmailToCandidate(posStatus.companyId, posStatus.candidateId, posStatus.positionId);
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }

        public async Task<List<company_cvs_email>> GetCompaniesEmails()
        {
            return await _cvsPositionsQueries.GetCompaniesEmails();
        }

        public async Task<List<SearchEntry>> SearchCands(int companyId, searchCandCvModel searchVals)
        {
            var results = await _luceneService.Search(companyId, searchVals);
            return results;
        }

        public async Task UpdateCvsAsciiSum(int companyId)
        {
            await _cvsPositionsQueries.UpdateCvsAsciiSum(companyId);
        }

        public async Task<List<CandPosStageTypeModel>> GetCandPosStagesTypes(int companyId)
        {
            return await _cvsPositionsQueries.GetCandPosStagesTypes(companyId);
        }

        public async Task SendEmail(SendEmailModel emailData, UserModel? user)
        {
            if (user != null)
            {
                EmailAddress from = new EmailAddress { Address = user.email, Name = $"{user.firstName} {user.lastName}" };

                List<AttachmentModel>? Attachments = new List<AttachmentModel>();

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
            await SaveCandidateToIndex(companyId, candReview.candidateId);
        }

        public async Task SaveCandidateToIndex(int companyId, int candidateId)
        {
            List<CvsToIndexModel> cvPropsToIndexList = await _cvsPositionsQueries.GetCompanyCvsToIndex(companyId, candidateId);
            await _luceneService.DocumentUpdate(companyId, cvPropsToIndexList);
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
        }

        public async Task UpdateIsSeen(int companyId, int cvId)
        {
            await _cvsPositionsQueries.UpdateIsSeen(companyId, cvId);
        }
        public async Task<List<CandReportModel?>> CandsReport(int companyId, string stageType)
        {
            return await _cvsPositionsQueries.CandsReport(companyId, stageType);
        }

        public async Task UpdatePositionDate(int companyId, int positionId, bool isUpdateCount)
        {
            await _cvsPositionsQueries.UpdatePositionDate(companyId, positionId, isUpdateCount);
        }
        public async Task<position?> GetPositionByMatchStr(int companyId, string matchStr)
        {
            return await _cvsPositionsQueries.GetPositionByMatchStr(companyId, matchStr);
        }

        public async Task AddSendEmail(SendEmailModel emailData, int userId)
        {
            await _cvsPositionsQueries.AddSendEmail(emailData, userId);
        }

        public async Task<int?> GetPositionTypeId(int companyId, string positionRelated)
        {
            return await _cvsPositionsQueries.GetPositionTypeId(companyId, positionRelated);
        }

        public async Task<int> AddPositionTypeName(int companyId, string positionRelated)
        {
            return await _cvsPositionsQueries.AddPositionTypeName(companyId, positionRelated);
        }

        public async Task<List<PositionTypeModel>> GetPositionsTypes(int companyId)
        {
            return await _cvsPositionsQueries.GetPositionsTypes(companyId);
        }

        public async Task SaveCustomerCandReview(int companyId, CandReviewModel customerCandReview)
        {
            await _cvsPositionsQueries.SaveCustomerCandReview(companyId, customerCandReview);
            await _cvsPositionsQueries.UpdateCandCustomersReviews(companyId, customerCandReview.candidateId);
        }

        public async Task CalculatePositionTypesCount(int companyId)
        {
            await _cvsPositionsQueries.CalculatePositionTypesCount(companyId);
        }

        public async Task<List<PositionTypeCountModel>> PositionsTypesCvsCount(int companyId)
        {
            return await _cvsPositionsQueries.PositionsTypesCvsCount(companyId);
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

    }
}