using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using LuceneLibrary;

namespace CvsPositionsLibrary
{
    public class CvsPositionsServise: ICvsPositionsServise
    {
        private ICvsPositionsQueries _cvsPositionsQueries;
        private ILuceneService _luceneService;

        public CvsPositionsServise(ICvsPositionsQueries cvsPositionsQueries, ILuceneService luceneService)
        {
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
            return cvsList;
        }
    }
}
