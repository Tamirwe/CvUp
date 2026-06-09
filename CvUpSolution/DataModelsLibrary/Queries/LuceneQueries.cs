using Database.models;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataModelsLibrary.Queries
{
    public class LuceneQueries : ILuceneQueries
    {
        public async Task<List<CvsToIndexModel>> GetCandidatesLastCvsToIndex(int companyId, int candidateId = 0)
        {
            using var dbContext = new cvupdbContext();

            var query = from cand in dbContext.candidates
                        join cvt in dbContext.cvs_txts
                        on new { ID = cand.id, cvId = (int)cand.last_cv_id! }
                        equals new { ID = (int)cvt.candidate_id!, cvId = cvt.cv_id }
                        where cand.company_id == companyId
                           && cand.is_black_list == false
                           && cand.last_cv_id != null
                           && cvt.candidate_id != null
                        select new CvsToIndexModel
                        {
                            candidateId = cand.id,
                            firstName = cand.first_name,
                            lastName = cand.last_name,
                            reviewText = cand.review,
                            cvsTxt = cvt.cv_txt,
                            email = cand.email,
                            phone = cand.phone,
                        };

            if (candidateId > 0)
                query = query.Where(c => c.candidateId == candidateId);

            List<CvsToIndexModel> candsCvs = await query.ToListAsync();

            foreach (var cnd in candsCvs)
            {
                cnd.cvsTxt = $"{cnd.candidateId} {cnd.email} {cnd.phone} {cnd.firstName} {cnd.lastName} {cnd.reviewText} {cnd.cvsTxt}";
            }

            return candsCvs;
        }
    }
}
