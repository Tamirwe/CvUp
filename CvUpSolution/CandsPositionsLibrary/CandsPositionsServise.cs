using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using LuceneLibrary;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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

        public CandsPositionsServise(IConfiguration config, ICandsPositionsQueries cvsPositionsQueries, ILuceneService luceneService)
        {
            _configuration = config;
            _cvsPositionsQueries = cvsPositionsQueries;
            _luceneService = luceneService;

        }
        public async Task<int> AddCv(ImportCvModel importCv)
        {
            return await _cvsPositionsQueries.AddCv(importCv);
        }

        public async Task UpdateCvKeyId(ImportCvModel importCv)
        {
            await _cvsPositionsQueries.UpdateCvKeyId(importCv);
        }

        public async Task AddNewCvToIndex(ImportCvModel importCv)
        {
            CvPropsToIndexModel cvPropsToIndex = new CvPropsToIndexModel
            {
                candName = importCv.candidateName,
                cvId = importCv.cvId,
                candidateId = importCv.candidateId,
                cvTxt = importCv.cvTxt,
                email = importCv.emailAddress,
                phone = importCv.phone,
                emailSubject = importCv.subject,
            };

            await _luceneService.DocumentAdd(Convert.ToInt32(importCv.companyId), cvPropsToIndex);
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
                string newCandName = importCv.candidateName.Trim();


                if (cand != null)
                {
                    if (cand.name != null)
                    {
                        if (newCandName == "")
                        {
                            newCandName = cand.name;
                        }
                        else if (newCandName != cand.name)
                        {
                            newCandName = cand.name;
                        }
                    }

                    cand.name = newCandName;
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
                    name = importCv.candidateName
                };

                importCv.isNewCandidate = true;
                importCv.candidateId = await _cvsPositionsQueries.AddCandidate(newCand);
            }
        }

        public async Task IndexCompanyCvs(int companyId)
        {
            List<CvPropsToIndexModel> cvPropsToIndexList = await _cvsPositionsQueries.GetCompanyCvsToIndex(companyId);
            await _luceneService.BuildCompanyIndex(companyId, cvPropsToIndexList);
        }

        public async Task<List<CandModel?>> GetCandsList(int companyId, int page, int take, List<int>? candsIds)
        {
            var result = await _cvsPositionsQueries.GetCandsList(companyId, _configuration["GlobalSettings:cvsEncryptorKey"], page, take, candsIds);
            return result;
        }

        public async Task<List<CandCvModel>> GetCandCvsList(int companyId, int cvId, int candidateId)
        {
            List<CandCvModel> cvsList = await _cvsPositionsQueries.GetCandCvsList(companyId, candidateId, _configuration["GlobalSettings:cvsEncryptorKey"]);
            cvsList.RemoveAll(x => x.cvId == cvId);
            return cvsList;
        }

        public async Task<List<CandModel>> GetPosCandsList(int companyId, int positionId)
        {
            return await _cvsPositionsQueries.GetPosCandsList(companyId, positionId, _configuration["GlobalSettings:cvsEncryptorKey"]);
        }

        public async Task<PositionClientModel> GetPosition(int companyId, int positionId)
        {
            PositionClientModel pos = await _cvsPositionsQueries.GetPosition(companyId, positionId);
            return pos;
        }

        public async Task<position?> AddPosition(PositionClientModel data, int companyId, int userId)
        {
            position newRec = await _cvsPositionsQueries.AddPosition(data, companyId, userId);
            return newRec;
        }

        public async Task<position?> UpdatePosition(PositionClientModel data, int companyId, int userId)
        {
            position? updRec = await _cvsPositionsQueries.UpdatePosition(data, companyId, userId);
            return updRec;
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

        public async Task SaveCvReview(CvReviewModel cvReview)
        {
            await _cvsPositionsQueries.SaveCvReview(cvReview);
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

        public async Task<List<int>> SearchCands(int companyId, string searchKeyWords)
        {
            var results = await _luceneService.Search(companyId, searchKeyWords);
            var distinctCandsList = results.Select(e => e.CandId).Distinct().ToList();
            return distinctCandsList;
        }

        public async Task<List<CandModel?>> GetFolderCandsList(int companyId, int folderId)
        {
            return await _cvsPositionsQueries.GetFolderCandsList(companyId, folderId);
        }
    }
}