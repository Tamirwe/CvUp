using Database.models;
using DataModelsLibrary.Models;
using GeneralLibrary;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DataModelsLibrary.Queries
{
    public class CandsCvsQueries : ICandsCvsQueries
    {

        public async Task<CandModel?> GetCandidate(int companyId, int candId)
        {
            using (var dbContext = new cvupdbContext())
            {
                var query = (from cand in dbContext.candidates
                             join cvs in dbContext.cvs on cand.last_cv_id equals cvs.id
                             where cand.company_id == companyId && cand.id == candId
                             select new CandModel
                             {
                                 cvId = cvs.id,
                                 review = cand.review,
                                 reviewDate = cand.review_date,
                                 allCustomersReviews = cand.customers_reviews == null ? null : JsonConvert.DeserializeObject<CandCustomersReviewsModel[]>(cand.customers_reviews),
                                 keyId = cvs.key_id,
                                 candidateId = cand.id,
                                 email = cand.email,
                                 emailSubject = cvs.subject,
                                 firstName = cand.first_name,
                                 lastName = cand.last_name,
                                 phone = cand.phone,
                                 city = cand.city,
                                 hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                                 cvSent = Convert.ToDateTime(cand.last_cv_sent),
                                 candFoldersIds = cand.folders_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.folders_ids),
                                 candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                                 posStages = cand.pos_stages == null ? null : JsonConvert.DeserializeObject<CandPosStageModel[]>(cand.pos_stages),
                                 isSeen = Convert.ToBoolean(cvs.is_seen)
                             }).Take(300);

                var result = await query.FirstOrDefaultAsync();
                return result;
            }
        }

        public async Task<int> AddCv(ImportCvModel importCv)
        {
            using (var dbContext = new cvupdbContext())
            {
                var newCv = new cv
                {
                    company_id = Convert.ToInt32(importCv.companyId),
                    candidate_id = importCv.candidateId,
                    email_id = importCv.emailId,
                    subject = importCv.subject,
                    from = importCv.from,
                    //duplicate_cv_id = importCv.duplicateCvId,
                    position = Utils.Truncate(importCv.positionRelated, 250),
                    file_type = importCv.fileTypeKey,
                    date_created = importCv.dateCreated,
                    position_type_id = importCv.positionTypeId,

                };

                dbContext.cvs.Add(newCv);
                await dbContext.SaveChangesAsync();

                var cvTxt = new cvs_txt
                {
                    cv_id = newCv.id,
                    candidate_id = importCv.candidateId,
                    company_id = importCv.companyId,
                    cv_txt = importCv.cvTxt.Length > 7999 ? importCv.cvTxt.Substring(0, 7999) : importCv.cvTxt,
                    email_subject = importCv.subject,
                    ascii_sum = importCv.cvAsciiSum,
                };

                dbContext.cvs_txts.Add(cvTxt);

                await dbContext.SaveChangesAsync();

                return newCv.id;
            }
        }

        public async Task DeleteCv(int companyId, int candidateId, int cvId)
        {
            using (var dbContext = new cvupdbContext())
            {
                List<cv>? candCvs = await dbContext.cvs.Where(x => x.company_id == companyId && x.candidate_id == candidateId).ToListAsync();

                if (candCvs.Count > 1)
                {
                    cv? cvToDelete = candCvs.Where(x => x.id == cvId).FirstOrDefault();

                    if (cvToDelete != null)
                    {
                        var result = dbContext.cvs.Remove(cvToDelete);
                        await dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    await DeleteCandidate(companyId, candidateId);
                }
            }
        }

        public async Task<Tuple<cv?, bool>> GetCandLastCv(int companyId, int candidateId)
        {
            using (var dbContext = new cvupdbContext())
            {
                List<cv>? candCvs = await dbContext.cvs.Where(x => x.company_id == companyId && x.candidate_id == candidateId).OrderByDescending(x => x.date_created).ToListAsync();
                cv? lastCv = candCvs.OrderByDescending(x => x.date_created).FirstOrDefault();
                var isDuplicate = candCvs.Count > 1;
                return new Tuple<cv?, bool>(lastCv, isDuplicate);
            }
        }

        public async Task UpdateCandLastCv(int companyId, int candidateId, int cvId, bool isDuplicate, DateTime lastCvSent)
        {
            using (var dbContext = new cvupdbContext())
            {
                candidate cand = dbContext.candidates.Where(x => x.company_id == companyId && x.id == candidateId).First();
                cand.has_duplicates_cvs = isDuplicate;
                cand.last_cv_id = cvId;
                cand.last_cv_sent = lastCvSent;
                cand.date_updated = DateTime.Now;
                var result = dbContext.candidates.Update(cand);
                await dbContext.SaveChangesAsync();
            }
        }


        public async Task DeleteCandidate(int companyId, int candidateId)
        {
            using (var dbContext = new cvupdbContext())
            {
                candidate? candTodelete = dbContext.candidates.Where(x => x.company_id == companyId && x.id == candidateId).FirstOrDefault();

                if (candTodelete != null)
                {
                    var result = dbContext.candidates.Remove(candTodelete);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateCvDate(int cvId)
        {
            using (var dbContext = new cvupdbContext())
            {
                cv cv = dbContext.cvs.Where(x => x.id == cvId).First();
                cv.date_created = DateTime.Now;
                var result = dbContext.cvs.Update(cv);
                await dbContext.SaveChangesAsync();
            }
        }


        public async Task UpdateCvKeyId(ImportCvModel importCv)
        {
            using (var dbContext = new cvupdbContext())
            {
                cv? cv = dbContext.cvs.Where(x => x.id == importCv.cvId).First();
                cv.key_id = importCv.cvKey;
                var result = dbContext.cvs.Update(cv);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<int> AddCandidate(candidate newCand)
        {
            using (var dbContext = new cvupdbContext())
            {
                dbContext.candidates.Add(newCand);
                await dbContext.SaveChangesAsync();
                return newCand.id;
            }
        }

        public async Task UpdateCandidate(candidate cand)
        {
            using (var dbContext = new cvupdbContext())
            {
                var result = dbContext.candidates.Update(cand);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<candidate?> GetCandidateByEmail(string email)
        {
            using (var dbContext = new cvupdbContext())
            {
                List<candidate> cands = await dbContext.candidates.Where(x => x.email != null && x.email.ToLower() == email.ToLower()).ToListAsync();
                return cands.FirstOrDefault();
            }
        }

        public async Task<candidate?> GetCandidateByPhone(string phone)
        {
            using (var dbContext = new cvupdbContext())
            {
                candidate? cand = await dbContext.candidates.Where(x => x.phone == phone).FirstOrDefaultAsync();
                return cand;
            }
        }

        public async Task<List<CandCvModel>> GetCandCvsList(int companyId, int candidateId)
        {
            using (var dbContext = new cvupdbContext())
            {
                var query = (from cvs in dbContext.cvs
                             where cvs.company_id == companyId
                             && cvs.candidate_id == candidateId
                             //&& cvs.date_created >= DateTime.Now.AddDays(-30)
                             orderby cvs.date_created descending
                             select new CandCvModel
                             {
                                 candidateId = cvs.candidate_id,
                                 cvId = cvs.id,
                                 cvSent = cvs.date_created,
                                 keyId = cvs.key_id,
                                 emailSubject = cvs.subject,
                             });

                return await query.ToListAsync();
            }
        }

        public async Task UpdateCvsAsciiSum(int companyId)
        {

            using (var dbContext = new cvupdbContext())
            {
                List<cvs_txt>? cvsTxtList = await dbContext.cvs_txts.Where(x => x.ascii_sum == null).ToListAsync();
                dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                foreach (var itemCv in cvsTxtList)
                {
                    int docAsciiSum = 0;

                    if (itemCv.cv_txt != null)
                    {
                        foreach (char c in itemCv.cv_txt)
                        {
                            try
                            {
                                docAsciiSum += Convert.ToInt32(c);
                            }
                            catch (Exception) { }
                        }
                    }

                    itemCv.ascii_sum = docAsciiSum;
                }

                dbContext.cvs_txts.UpdateRange(cvsTxtList);

                await dbContext.SaveChangesAsync();
            }
        }

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

        private async Task<List<CvTxtModel>> GetCvsText(int companyId = 154, int candidateId = 0)
        {
            using (var dbContext = new cvupdbContext())
            {
                var cvsTxtQuery = from cv in dbContext.cvs_txts
                                  where cv.company_id == companyId 
                                  //&& candidateId > 0 ? cv.candidate_id == candidateId : 1 == 1
                                  orderby cv.candidate_id ascending, cv.ascii_sum
                                  select new CvTxtModel
                                  {
                                      id = cv.id,
                                      candidateId = cv.candidate_id ?? 0,
                                      cvTxt = cv.cv_txt,
                                      asciiSum = cv.ascii_sum,
                                  };

                // Conditionally append the candidateId filter strictly in C#
                if (candidateId > 0)
                {
                    cvsTxtQuery = cvsTxtQuery.Where(cv => cv.candidateId == candidateId);
                }

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
            using (var dbContext = new cvupdbContext())
            {
                var query = from cand in dbContext.candidates
                            where cand.company_id == companyId 
                            //&& candidateId > 0 ? cand.id == candidateId : 1 == 1
                            && cand.email != null && cand.name != null && cand.phone != null
                            select new AiCvModel
                            {
                                id = cand.id,
                                fullName = cand.name,
                                email = cand.email,
                                phone = cand.phone,
                            };

                // Conditionally append the candidateId filter strictly in C#
                if (candidateId > 0)
                {
                    query = query.Where(cand => cand.id == candidateId);
                }

                List<AiCvModel> candsParams = await query.ToListAsync();
                return candsParams;
            }
        }

        public async Task<List<DuplicateEmailCandModel>> GetDuplicateCandsByEmail()
        {
            using (var dbContext = new cvupdbContext())
            {
                return await dbContext.Database.SqlQuery<DuplicateEmailCandModel>($@"
                    SELECT LOWER(email) AS email, COUNT(*) AS cnt
                    FROM public.candidates
                    WHERE email IS NOT NULL
                    GROUP BY LOWER(email)
                    HAVING COUNT(*) > 1
                    ORDER BY cnt DESC").ToListAsync();
            }
        }

    }
}
