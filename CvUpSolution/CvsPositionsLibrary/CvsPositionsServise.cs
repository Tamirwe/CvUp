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

        public void AddImportedCv(ImportCvModel importCv)
        {
            _cvsPositionsQueries.AddImportedCv(importCv);
        }

        public void GetAddCandidateId(ImportCvModel item)
        {
           candidate? cand = _cvsPositionsQueries.GetCandidateByEmail(item.email);

            if (cand == null)
            {
                cand = _cvsPositionsQueries.AddNewCandidate(Convert.ToInt32(item.companyId), item.email,item.phone);
            }

            if (cand != null)
            {
                item.candidateId = cand.id;
            }
        }

        public int GetUniqueCvId()
        {
            return _cvsPositionsQueries.GetUniqueCvId();
        }

        public void IndexCompanyCvs(int companyId)
        {
            List<CompanyTextToIndexModel> CompanyTextToIndexList = _cvsPositionsQueries.GetCompanyCvsToIndex(companyId);
            _luceneService.BuildCompanyIndex(companyId, CompanyTextToIndexList);
        }
    }
}
