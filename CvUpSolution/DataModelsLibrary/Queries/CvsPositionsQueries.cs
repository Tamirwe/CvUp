using Database.models;
using DataModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Queries
{
    public class CvsPositionsQueries : ICvsPositionsQueries
    {
        private cvup00001Context dbContext;

        public CvsPositionsQueries()
        {
            dbContext = new cvup00001Context();
        }

        public void AddImportedCv(string companyId, string cvId, int candidateId, int cvAsciiSum, string emailId, string subject, string from)
        {
            var cv = new cv
            {
                id = cvId,
                company_id = Convert.ToInt32(companyId),
                candidate_id = candidateId,
                cv_ascii_sum = cvAsciiSum,
                email_id = emailId,
                subject = subject,
                from = from,
            };

            dbContext.cvs.Add(cv);
            dbContext.SaveChanges();
        }

        public int GetUniqueCvId()
        {
            var ci = new cvs_incremental
            {
                name = "cc"
            };

            dbContext.cvs_incrementals.Add(ci);
            dbContext.SaveChanges();
            return ci.id;
        }

        public candidate AddNewCandidate(int companyId, string email, string phone)
        {
            var cand = new candidate
            {
                company_id = companyId,
                email = email,
                phone = phone
            };

            dbContext.candidates.Add(cand);
            dbContext.SaveChanges();
            return cand;
        }

        public candidate? GetCandidateByEmail(string email)
        {
            candidate? cand = dbContext.candidates.Where(x => x.email == email).FirstOrDefault();
            return cand;
        }

        public List<CompanyTextToIndexModel> GetCompanyTexstsToIndex(int companyId)
        {
            var query = from cand in dbContext.candidates
                        join cvs in dbContext.cvs on cand.id equals cvs.candidate_id
                        join cvTxt in dbContext.cvs_txts on cvs.id equals cvTxt.id
                        where cand.company_id == companyId
                        select new CompanyTextToIndexModel
                        {
                            companyId = companyId,
                            cvId = cvs.id,
                            cvTxt = cvTxt.cv_txt,
                            email = cand.email,
                            emailSubject = cvs.subject,
                            candidateName = cand.name,
                            candidateOpinion = cand.opinion,
                            phone = cand.phone,
                        };

            return query.ToList();
        }

    }
}
