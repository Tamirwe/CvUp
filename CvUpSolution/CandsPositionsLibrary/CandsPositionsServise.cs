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
        public int AddCv(ImportCvModel importCv)
        {
            return _cvsPositionsQueries.AddCv(importCv);
        }

        public void UpdateCvKeyId(ImportCvModel importCv)
        {
            _cvsPositionsQueries.UpdateCvKeyId(importCv);
        }

        public void AddNewCvToIndex(ImportCvModel importCv)
        {
            CvPropsToIndexModel cvPropsToIndex = new CvPropsToIndexModel
            {
                candName = importCv.candidateName,
                cvId = importCv.cvId,
                cvTxt = importCv.cvTxt,
                email = importCv.emailAddress,
                phone = importCv.phone,
                emailSubject = importCv.subject,
            };

            _luceneService.DocumentAdd(Convert.ToInt32(importCv.companyId), cvPropsToIndex);
        }

        public candidate? GetCandidateByEmail(string email)
        {
            return _cvsPositionsQueries.GetCandidateByEmail(email);
        }

        public candidate? GetCandidateByPhone(string phone)
        {
            return _cvsPositionsQueries.GetCandidateByPhone(phone);
        }

        public void AddUpdateCandidateFromCvImport(ImportCvModel importCv)
        {
            candidate? cand = null;

            if (!string.IsNullOrEmpty(importCv.emailAddress))
            {
                cand = GetCandidateByEmail(importCv.emailAddress);
            }
            else if (!string.IsNullOrEmpty(importCv.phone))
            {
                cand = GetCandidateByPhone(importCv.phone);
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
                    _cvsPositionsQueries.UpdateCandidate(cand);
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
                importCv.candidateId = _cvsPositionsQueries.AddCandidate(newCand);
            }
        }

        public void IndexCompanyCvs(int companyId)
        {
            List<CvPropsToIndexModel> cvPropsToIndexList = _cvsPositionsQueries.GetCompanyCvsToIndex(companyId);
            _luceneService.BuildCompanyIndex(companyId, cvPropsToIndexList);
        }

        public List<CandModel> GetCandsList(int companyId, int page, int take, int positionId, string? searchKeyWords)
        {
            return _cvsPositionsQueries.GetCandsList(companyId, _configuration["GlobalSettings:cvsEncryptorKey"], page, take, positionId, searchKeyWords);
        }

        public List<CandModel> GetCandCvsList(int companyId, int cvId, int candidateId)
        {
            List<CandModel> cvsList = _cvsPositionsQueries.GetCandCvsList(companyId, candidateId, _configuration["GlobalSettings:cvsEncryptorKey"]);
            cvsList.RemoveAll(x => x.cvId == cvId);
            return cvsList;
        }

        public List<CandModel> GetPosCandsList(int companyId, int positionId)
        {
            return _cvsPositionsQueries.GetPosCandsList(companyId, positionId, _configuration["GlobalSettings:cvsEncryptorKey"]);
        }

        public PositionClientModel GetPosition(int companyId, int positionId)
        {
            PositionClientModel pos = _cvsPositionsQueries.GetPosition(companyId, positionId);
            return pos;
        }

        public position? AddPosition(PositionClientModel data, int companyId, int userId)
        {
            position newRec = _cvsPositionsQueries.AddPosition(data, companyId, userId);
            return newRec;
        }

        public position? UpdatePosition(PositionClientModel data, int companyId, int userId)
        {
            position? updRec = _cvsPositionsQueries.UpdatePosition(data, companyId, userId);
            return updRec;
        }

        public List<PositionModel> GetPositionsList(int companyId)
        {
            List<PositionModel> qList = _cvsPositionsQueries.GetPositionsList(companyId);
            return qList;
        }

        public void DeletePosition(int companyId, int id)
        {
            _cvsPositionsQueries.DeletePosition(companyId, id);
        }

        public List<ParserRulesModel> GetParsersRules(int companyId)
        {
            return _cvsPositionsQueries.GetParsersRules(companyId);
        }

        public CvModel? GetCv(int cvId, int companyId)
        {
            return _cvsPositionsQueries.GetCv(cvId, companyId);
        }

        public void SaveCvReview(CvReviewModel cvReview)
        {
            _cvsPositionsQueries.SaveCvReview(cvReview);
        }

        public List<cv> CheckIsCvDuplicate(int companyId, int candidateId, int cvAsciiSum)
        {
            List<cv> cvs = _cvsPositionsQueries.CheckIsCvDuplicate(companyId, candidateId, cvAsciiSum);
            return cvs;
        }

        public void UpdateCandidateLastCv(ImportCvModel importCv)
        {
            _cvsPositionsQueries.UpdateCandidateLastCv(importCv);
        }

        public void UpdateSameCv(ImportCvModel importCv)
        {
            _cvsPositionsQueries.UpdateSameCv(importCv);
        }

        public CandPosModel AttachPosCandCv(AttachePosCandCvModel posCv)
        {
            return _cvsPositionsQueries.AttachPosCandCv(posCv);
        }

        public CandPosModel DetachPosCand(AttachePosCandCvModel posCv)
        {
            return _cvsPositionsQueries.DetachPosCand(posCv);
        }

        public List<company_cvs_email> GetCompaniesEmails()
        {
            return _cvsPositionsQueries.GetCompaniesEmails();
        }

    }
}
