﻿using CvFilesLibrary;
using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using EmailsLibrary;
using EmailsLibrary.Models;
using LuceneLibrary;
using Microsoft.Extensions.Configuration;

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
                string candFirstName = importCv.firstName.Trim();
                string candLastName = importCv.lastName.Trim();


                if (cand != null)
                {
                    if (string.IsNullOrEmpty(cand.first_name))
                    {
                        cand.first_name = candFirstName;
                    }

                    if (string.IsNullOrEmpty(cand.last_name))
                    {
                        cand.last_name = candLastName;
                    }

                    await _cvsPositionsQueries.UpdateCandidate(cand);
                    importCv.candidateId = cand.id;
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

        public async Task<List<CandModel?>> GetCandsList(int companyId, int page, int take, List<int>? candsIds)
        {
            var result = await _cvsPositionsQueries.GetCandsList(companyId, _configuration["GlobalSettings:cvsEncryptorKey"], page, take, candsIds);
            return result;
        }

        public async Task<List<CandModel?>> GetFolderCandsList(int companyId, int folderId, List<int>? candsIds)
        {
            return await _cvsPositionsQueries.GetFolderCandsList(companyId, folderId, candsIds);
        }

        public async Task<List<CandModel?>> GetPosCandsList(int companyId, int positionId, List<int>? candsIds)
        {
            return await _cvsPositionsQueries.GetPosCandsList(companyId, positionId, candsIds);
        }

        public async Task<List<CandCvModel>> GetCandCvsList(int companyId, int cvId, int candidateId)
        {
            List<CandCvModel> cvsList = await _cvsPositionsQueries.GetCandCvsList(companyId, candidateId, _configuration["GlobalSettings:cvsEncryptorKey"]);
            //cvsList.RemoveAll(x => x.cvId == cvId);
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

        public async Task<List<cv>> CheckIsCvDuplicate(int companyId, int candidateId, int cvAsciiSum)
        {
            List<cv> cvs = await _cvsPositionsQueries.CheckIsCvDuplicate(companyId, candidateId, cvAsciiSum);
            return cvs;
        }

        public async Task UpdateCandidateLastCv(ImportCvModel importCv)
        {
            await _cvsPositionsQueries.UpdateCandidateLastCv(importCv);
        }

        public async Task UpdateSameCv(ImportCvModel importCv)
        {
            await _cvsPositionsQueries.UpdateSameCv(importCv);
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
        }

        public async Task<List<company_cvs_email>> GetCompaniesEmails()
        {
            return await _cvsPositionsQueries.GetCompaniesEmails();
        }

        public async Task<List<int>> SearchCands(int companyId, searchCandCvModel searchVals)
        {
            var results = await _luceneService.Search(companyId, searchVals);
            var distinctCandsList = results.Select(e => e.CandId).Distinct().ToList();
            return distinctCandsList;
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
                    From = from,
                    Body = $"{emailData.body} {user.signature} ",
                    Attachments = Attachments
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
    }
}