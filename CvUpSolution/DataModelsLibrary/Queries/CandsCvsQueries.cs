using Database.models;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DataModelsLibrary.Queries
{
    public class CandsCvsQueries : ICandsCvsQueries
    {

        public async Task<List<AiCvModel>> GetDistinctCandsCvs(int companyId = 154, int candidateId = 0)
        {
            List<CvTxtModel> candsCvsTxts = await GetCvsText(companyId, candidateId);
            List<CvTxtModel> ConcatCandsCvsTxt = ConcatCandsCvsTxtAndRemoveDuplicates(candsCvsTxts);

            List<AiCvModel> candsParams = await GetCandidatesParams(companyId, candidateId);

            foreach (var item in candsParams)
            {
                var candCvsTxt = ConcatCandsCvsTxt.Find(x => x.candidateId == item.id);

                if (candCvsTxt != null)
                {
                    item.cvsTxt = candCvsTxt.cvTxt;
                }
            }

            return candsParams;
        }

        public async Task<List<CandCvTxtModel>> GetCandsLastCvText(int companyId = 154, int candidateId = 0)
        {
            using (var dbContext = new cvup00001Context())
            {
                dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                string sql = @"SELECT cands.id candidateId,cvs.id ,cvs.key_id keyId, ctx.cv_txt cvTxt
                                FROM candidates cands 
                                INNER JOIN cvs ON cands.last_cv_id = cvs.id
                                INNER JOIN cvs_txt ctx ON cvs.id = ctx.cv_id
                                WHERE cands.company_id=" + companyId + @" AND cands.is_cv_analyzed = 0 
                                ORDER BY cvs.id DESC 
                                LIMIT 0, 13";

                var candCvTxtModelList = await dbContext.candCvTxtModel.FromSqlRaw(sql).ToListAsync();
                return candCvTxtModelList;
            }
        }

        public async Task AddCandidateAnalyzeCv(ai_analyze_cv analyzeCv)
        {
            await DeleteCandidateAnalyzeCv(analyzeCv.candidate_id);

            using (var dbContext = new cvup00001Context())
            {
                dbContext.ai_analyze_cvs.Add(analyzeCv);
                await dbContext.SaveChangesAsync();
            }

            await UpdateCandIsAnalyzed(analyzeCv.candidate_id, true);

        }

        public async Task DeleteCandidateAnalyzeCv( int candidateId)
        {
            using (var dbContext = new cvup00001Context())
            {
                ai_analyze_cv? analyzeTodelete = dbContext.ai_analyze_cvs.FirstOrDefault(x =>  x.candidate_id == candidateId);

                if (analyzeTodelete != null)
                {
                    var result = dbContext.ai_analyze_cvs.Remove(analyzeTodelete);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateCandIsAnalyzed(int candidateId, bool isAnalyzed)
        {
            using (var dbContext = new cvup00001Context())
            {
                candidate? cand = dbContext.candidates.FirstOrDefault(x => x.id == candidateId);

                if (cand != null)
                {
                    cand.is_cv_analyzed = isAnalyzed;

                    var result = dbContext.candidates.Update(cand);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateIsEmbeddedBatch(List<EmbedCvDataModel> cvs)
        {
            using (var dbContext = new cvup00001Context())
            {
                foreach (var cv in cvs)
                {
                    ai_analyze_cv? cand = dbContext.ai_analyze_cvs.FirstOrDefault(x => x.candidate_id == cv.CandidateId);

                    if (cand != null)
                    {
                        cand.is_embedded = true;

                        var result = dbContext.ai_analyze_cvs.Update(cand);
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<EmbedCvDataModel>> GetAnalyzedCvsForEmbeeding()
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from ai in dbContext.ai_analyze_cvs
                                  where ai.is_embedded == false 
                                  select new EmbedCvDataModel
                                  {
                                      CandidateId = ai.candidate_id,
                                      Name = ai.name,
                                      Email = ai.email,
                                      Phone = ai.phone,
                                      Location = ai.city,
                                      Region = ai.region,
                                      Area = ai.area,
                                      Languages = ai.languages,
                                      CurrentTitleEn = ai.current_title_en ?? "",
                                      CurrentTitleHe = ai.current_title_he ?? "",
                                      Companies = ai.companies,
                                      Skills = StringToList(ai.skills),
                                      SummaryEn = ai.summary_en ?? "",
                                      SummaryHe = ai.summary_he ?? "",
                                      YearsExperience = ai.years_experience,
                                  };

                List<EmbedCvDataModel> dataForEmbeeding = await query.Take(300).ToListAsync();
                return dataForEmbeeding;
            }
        }

        private static List<string>? StringToList(string? str)
        {
            if (str == null) return null;

            return str.Split(',').ToList();
        }

        private async Task<List<CvTxtModel>> GetCvsText(int companyId = 154, int candidateId = 0)
        {
            using (var dbContext = new cvup00001Context())
            {
                var cvsTxtQuery = from cv in dbContext.cvs_txts
                                  where cv.company_id == companyId && candidateId > 0 ? cv.candidate_id == candidateId : 1 == 1
                                  orderby cv.candidate_id ascending, cv.ascii_sum
                                  select new CvTxtModel
                                  {
                                      id = cv.id,
                                      candidateId = cv.candidate_id ?? 0,
                                      cvTxt = cv.cv_txt,
                                      asciiSum = cv.ascii_sum,
                                  };

                List<CvTxtModel> cvsTxts = await cvsTxtQuery.ToListAsync();
                return cvsTxts;
            }
        }

        private List<CvTxtModel> ConcatCandsCvsTxtAndRemoveDuplicates(List<CvTxtModel> candCvsTxts)
        {

            int asciiSumCurrent = -1;
            int candIdCurrent = 0;
            StringBuilder? sb = null;

            List<CvTxtModel> candsGroupCvsTxt = new List<CvTxtModel>();

            foreach (var item in candCvsTxts)
            {
                if (candIdCurrent != item.candidateId)
                {
                    if (sb != null)
                    {
                        candsGroupCvsTxt.Add(new CvTxtModel { candidateId = candIdCurrent, cvTxt = sb.ToString() });
                    }

                    candIdCurrent = item.candidateId ?? 0;
                    asciiSumCurrent = item.asciiSum ?? -1;
                    sb = new StringBuilder(item.cvTxt);
                }
                else
                {
                    if (asciiSumCurrent != item.asciiSum && sb != null)
                    {
                        sb.Append("" + item.cvTxt);
                    }

                    candIdCurrent = item.candidateId ?? 0;
                    asciiSumCurrent = item.asciiSum ?? -1;
                }
            }

            if (sb != null)
            {
                candsGroupCvsTxt.Add(new CvTxtModel { candidateId = candIdCurrent, cvTxt = sb.ToString() });
            }

            return candsGroupCvsTxt;
        }

        private async Task<List<AiCvModel>> GetCandidatesParams(int companyId = 154, int candidateId = 0)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from cand in dbContext.candidates
                            where cand.company_id == companyId && candidateId > 0 ? cand.id == candidateId : 1 == 1
                            && cand.email != null && cand.name != null && cand.phone != null
                            select new AiCvModel
                            {
                                id = cand.id,
                                fullName = cand.name,
                                email = cand.email,
                                phone = cand.phone,
                            };

                List<AiCvModel> candsParams = await query.ToListAsync();
                return candsParams;
            }
        }

    }
}
