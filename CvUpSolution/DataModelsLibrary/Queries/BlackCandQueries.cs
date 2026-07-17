using Database.models;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataModelsLibrary.Queries
{
    public class BlackCandQueries : IBlackCandQueries
    {
        public async Task<List<blackCandModel>> GetBlackCandidatesList()
        {
            using (var dbContext = new cvupdbContext())
            {

                var query = (from b in dbContext.black_cands
                             select new blackCandModel
                             {
                                 id = b.id,
                                 candidate_id = b.candidate_id,
                                 email = b.email,
                                 phone = b.phone,
                                 cvs_count = b.cvs_count
                             });

                var bList = await query.ToListAsync();

                return bList;
            }
        }

        public async Task UpdateBlackCandidateEmailCount(blackCandModel blackCand)
        {
            using (var dbContext = new cvupdbContext())
            {
                black_cand? bCand = dbContext.black_cands.Where(x => x.candidate_id == blackCand.candidate_id).FirstOrDefault();

                if (bCand != null)
                {
                    bCand.cvs_count = blackCand.cvs_count;
                    bCand.updated = DateTime.Now;
                    var result = dbContext.black_cands.Update(bCand);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task AddBlackCand(blackCandModel blackCand)
        {
            using (var dbContext = new cvupdbContext())
            {
                black_cand bCand = new black_cand
                {
                    candidate_id = blackCand.candidate_id,
                    email = blackCand.email,
                    phone = blackCand.phone,
                    cvs_count = blackCand.cvs_count,
                    updated = DateTime.Now
                };

                dbContext.black_cands.Add(bCand);

                candidate? cand = dbContext.candidates.Where(x => x.id == blackCand.candidate_id).FirstOrDefault();

                if (cand != null)
                {
                    cand.is_black_list = true;
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveBlackCand(int candidateId)
        {
            using (var dbContext = new cvupdbContext())
            {
                black_cand? bCand = dbContext.black_cands.Where(x => x.candidate_id == candidateId).FirstOrDefault();

                if (bCand != null)
                {
                    dbContext.black_cands.Remove(bCand);
                }

                candidate? cand = dbContext.candidates.Where(x => x.id == candidateId).FirstOrDefault();

                if (cand != null)
                {
                    cand.is_black_list = false;
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
