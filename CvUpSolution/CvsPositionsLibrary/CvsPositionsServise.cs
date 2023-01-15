using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using GeneralLibrary;
using LuceneLibrary;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.Design;

namespace CvsPositionsLibrary
{
    public partial class CvsPositionsServise: ICvsPositionsServise
    {
        private IConfiguration _configuration;
        private ICvsPositionsQueries _cvsPositionsQueries;
        private ILuceneService _luceneService;

        public CvsPositionsServise(IConfiguration config, ICvsPositionsQueries cvsPositionsQueries, ILuceneService luceneService)
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

        public candidate? GetCandidate(string email)
        {
            return _cvsPositionsQueries.GetCandidateByEmail(email);
        }

        public void AddUpdateCandidateFromCvImport(ImportCvModel importCv)
        {
            int candId = 0;

            if (importCv.emailAddress.Length > 0)
            {
                candidate? cand = GetCandidate(importCv.emailAddress);
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
                    cand.has_duplicates_cvs = 1;
                    _cvsPositionsQueries.UpdateCandidate(cand);
                    importCv.candidateId = cand.id;
                }
            }

            if (candId == 0)
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

        public List<CvListItemModel> GetCvsList(int companyId)
        {
            return _cvsPositionsQueries.GetCvsList(companyId, _configuration["GlobalSettings:cvsEncryptorKey"]);
        }

        public PositionClientModel GetPosition(int companyId, int positionId)
        {
            PositionClientModel pos = _cvsPositionsQueries.GetPosition( companyId, positionId);
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

        public List<PositionListItemModel> GetPositionsList(int companyId)
        {
            List<PositionListItemModel> qList = _cvsPositionsQueries.GetPositionsList(companyId);
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
            return _cvsPositionsQueries.GetCv( cvId,  companyId);
        }

        public void SaveCvReview(CvReviewModel cvReview)
        {
            _cvsPositionsQueries.SaveCvReview(cvReview);
        }

        public List<cv> CheckIsCvDuplicate(int companyId, int candidateId,  int cvAsciiSum)
        {
            List<cv> cvs = _cvsPositionsQueries.CheckIsCvDuplicate( companyId,  candidateId,   cvAsciiSum);
            return cvs;
        }

        public void UpdateDuplicateAndLastCv(ImportCvModel importCv)
        {
            _cvsPositionsQueries.UpdateDuplicateAndLastCv(importCv);
        }


    }
}
