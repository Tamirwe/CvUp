using Database.models;
using DataModelsLibrary.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataModelsLibrary.Queries
{
    public class BlackCandQueries : IBlackCandQueries
    {
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
