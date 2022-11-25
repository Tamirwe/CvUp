using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
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

        public CvsPositionsServise(ICvsPositionsQueries cvsPositionsQueries)
        {
            _cvsPositionsQueries = cvsPositionsQueries;
        }

        public void AddImportedCv(string companyId, string cvId, int candidateId, int cvAsciiSum, string emailId, string subject, string from)
        {
            _cvsPositionsQueries.AddImportedCv(companyId, cvId, candidateId, cvAsciiSum, emailId, subject, from);
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

    }
}
