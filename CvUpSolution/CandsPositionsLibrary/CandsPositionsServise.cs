using CvFilesLibrary;
using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using EmailsLibrary;
using EmailsLibrary.Models;
using LuceneLibrary;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                string email = importCv.emailAddress.Length > 0 ? importCv.emailAddress : "Not Found";

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
            List<CvsToIndexModel> cvPropsToIndexList = await _cvsPositionsQueries.GetCompanyCvsToIndex(companyId,0);
            await _luceneService.CompanyIndexAddDocuments(companyId, cvPropsToIndexList, true);
        }

        public async Task<List<CandModel?>> GetCandsList(int companyId, int page, int take, List<int>? candsIds)
        {
            var result = await _cvsPositionsQueries.GetCandsList(companyId, _configuration["GlobalSettings:cvsEncryptorKey"], page, take, candsIds);
            return result;
        }

        public async Task<List<CandCvModel>> GetCandCvsList(int companyId, int cvId, int candidateId)
        {
            List<CandCvModel> cvsList = await _cvsPositionsQueries.GetCandCvsList(companyId, candidateId, _configuration["GlobalSettings:cvsEncryptorKey"]);
            //cvsList.RemoveAll(x => x.cvId == cvId);
            return cvsList;
        }

        public async Task<List<CandModel?>> GetPosCandsList(int companyId, int positionId, List<int>? candsIds)
        {
            return await _cvsPositionsQueries.GetPosCandsList(companyId, positionId,  candsIds);
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

        public async Task<List<ParserRulesModel>> GetParsersRules(int companyId)
        {
            return await _cvsPositionsQueries.GetParsersRules(companyId);
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

        public async Task<CandPosModel> AttachPosCandCv(AttachePosCandCvModel posCv)
        {
            return await _cvsPositionsQueries.AttachPosCandCv(posCv);
        }

        public async Task<CandPosModel> DetachPosCand(AttachePosCandCvModel posCv)
        {
            return await _cvsPositionsQueries.DetachPosCand(posCv);
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

        public async Task<List<CandModel?>> GetFolderCandsList(int companyId, int folderId, List<int>? candsIds)
        {
            return await _cvsPositionsQueries.GetFolderCandsList(companyId, folderId,  candsIds);
        }

        public async Task ActivatePosition( int companyId, PositionModel data)
        {
             await _cvsPositionsQueries.ActivatePosition(companyId, data);
        }

        public async Task DactivatePosition(int companyId, PositionModel data)
        {
             await _cvsPositionsQueries.DactivatePosition(companyId, data);
        }

        public async Task UpdateCvsAsciiSum(int companyId)
        {
            await _cvsPositionsQueries.UpdateCvsAsciiSum(companyId);
        }

        public async Task<List<companyStagesTypesModel>> GetCompanyStagesTypes(int companyId)
        {
            return await _cvsPositionsQueries.GetCompanyStagesTypes(companyId);
        }

        public async Task<bool> SendEmailToCand(EmailToCandModel emailToCand)
        {
            var email = new EmailModel
            {
                To = emailToCand.addresses,
                Subject = emailToCand.emailSubject,
                Body = emailToCand.emailBody
            };

            await _emailService.Send(email);

            return true;
        }

        public async Task<bool> SaveCandReview(int companyId ,CandReviewModel candReview)
        {
            await _cvsPositionsQueries.SaveCandReview(companyId,candReview);
            await SaveCandidateToIndex(companyId, candReview.candidateId);
            return true;
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

        public async Task SendEmail(SendEmailModel emailData)
        {
            List<AttachmentModel>? Attachments = new List<AttachmentModel>();

            if (emailData.attachCvs != null)
            {
                foreach (var item in emailData.attachCvs)
                {
                    var Attachment = _cvsFilesService.AddPdfLogo(emailData.companyId, item.cvKey);
                    Attachments.Add(new AttachmentModel { Attachment = Attachment, name = item.name });
                }
            }

            await _emailService.Send(new EmailModel { To = emailData.toAddresses, Subject = emailData.subject, Body = emailData.body, Attachments= Attachments });
        }

    }
}