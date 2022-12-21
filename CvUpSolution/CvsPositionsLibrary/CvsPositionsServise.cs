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

        public void AddNewCvToDb(ImportCvModel importCv)
        {
            _cvsPositionsQueries.AddNewCvToDb(importCv);
        }

        public void AddNewCvToIndex(ImportCvModel importCv)
        {
            CvPropsToIndexModel cvPropsToIndex = new CvPropsToIndexModel
            {
                candidateName = importCv.candidateName,
                cvId = importCv.cvId,
                cvTxt = importCv.cvTxt,
                email = importCv.email,
                phone = importCv.phone,
                emailSubject = importCv.subject
            };

            _luceneService.DocumentAdd(Convert.ToInt32(importCv.companyId), cvPropsToIndex);
        }


        public int GetAddCandidateId(int companyId, string email, string phone)
        {
            candidate? cand = _cvsPositionsQueries.GetCandidateByEmail(email);

            if (cand == null)
            {
                cand = _cvsPositionsQueries.AddNewCandidate(companyId, email, phone);
            }

            return cand.id;
        }

        public int GetUniqueCvId()
        {
            return _cvsPositionsQueries.GetUniqueCvId();
        }

        public void IndexCompanyCvs(int companyId)
        {
            List<CvPropsToIndexModel> cvPropsToIndexList = _cvsPositionsQueries.GetCompanyCvsToIndex(companyId);
            _luceneService.BuildCompanyIndex(companyId, cvPropsToIndexList);
        }

        public List<CvListItemModel> GetCvsList(int companyId)
        {
            List<CvListItemModel> cvsList = _cvsPositionsQueries.GetCvsList(companyId);

            foreach (var item in cvsList)
            {
                item.encriptedId= Encriptor.Encrypt($"{item.cvId}~{DateTime.Now.ToString("yyyy-MM-dd")}", _configuration["GlobalSettings:cvsEncryptorKey"]);
            }

            return cvsList;
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

    }
}
