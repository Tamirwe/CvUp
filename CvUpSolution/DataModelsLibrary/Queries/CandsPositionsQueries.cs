using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using GeneralLibrary;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DataModelsLibrary.Queries
{
    public class CandsPositionsQueries : ICandsPositionsQueries
    {

        public CandsPositionsQueries()
        {
        }

        public async Task<CandModel?> GetCandidate(int companyId, int candId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from cand in dbContext.candidates
                             join cvs in dbContext.cvs on cand.last_cv_id equals cvs.id
                             where cand.company_id == companyId && cand.id == candId
                             select new CandModel
                             {
                                 cvId = cvs.id,
                                 review = cand.review_text,
                                 keyId = cvs.key_id,
                                 candidateId = cand.id,
                                 email = cand.email,
                                 emailSubject = cvs.subject,
                                 firstName = cand.first_name,
                                 lastName = cand.last_name,
                                 phone = cand.phone,
                                 hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                                 cvSent = Convert.ToDateTime(cand.last_cv_sent),
                                 candFoldersIds = cand.folders_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.folders_ids),
                                 candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                                 posStages = cand.pos_stages == null ? null : JsonConvert.DeserializeObject<CandPosStageModel[]>(cand.pos_stages),
                             }).Take(300);

                var result = await query.FirstOrDefaultAsync();
                return result;
            }
        }

        public async Task<List<CandModel?>> GetCandsList(int companyId, string encriptKey, int page, int take, List<int>? candsIds)
        {
            int skip = (page - 1) * take;
            using (var dbContext = new cvup00001Context())
            {
                var query = (from cand in dbContext.candidates
                             join cvs in dbContext.cvs on cand.last_cv_id equals cvs.id
                             where cand.company_id == companyId && candsIds != null ? candsIds.Contains(cand.id) : 1 == 1
                             orderby cand.last_cv_sent descending
                             select new CandModel
                             {
                                 cvId = cvs.id,
                                 review = cand.review_text,
                                 keyId = cvs.key_id,
                                 candidateId = cand.id,
                                 email = cand.email,
                                 emailSubject = cvs.subject,
                                 firstName = cand.first_name,
                                 lastName = cand.last_name,
                                 phone = cand.phone,
                                 hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                                 cvSent = Convert.ToDateTime(cand.last_cv_sent),
                                 candFoldersIds = cand.folders_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.folders_ids),
                                 candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                                 posStages = cand.pos_stages == null ? null : JsonConvert.DeserializeObject<CandPosStageModel[]>(cand.pos_stages),
                             }).Take(300);

                var result = await query.ToListAsync();
                return result;
            }
        }

        public async Task<List<CandModel?>> GetPosCandsList(int companyId, int positionId, List<int>? candsIds)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from cand in dbContext.candidates
                             join pcv in dbContext.position_candidates on cand.id equals pcv.candidate_id
                             join cvs in dbContext.cvs on pcv.cv_id equals cvs.id
                             where pcv.company_id == companyId
                                    && pcv.position_id == positionId
                             orderby pcv.date_created descending
                             select new CandModel
                             {
                                 cvId = cvs.id,
                                 review = cand.review_text,
                                 keyId = cvs.key_id,
                                 candidateId = cand.id,
                                 email = cand.email,
                                 emailSubject = cvs.subject,
                                 firstName = cand.first_name,
                                 lastName = cand.last_name,
                                 phone = cand.phone,
                                 hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                                 cvSent = cvs.date_created,
                                 candFoldersIds = cand.folders_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.folders_ids),
                                 candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                                 posStages = cand.pos_stages == null ? null : JsonConvert.DeserializeObject<CandPosStageModel[]>(cand.pos_stages),
                             });

                return await query.ToListAsync();
            }
        }

        public async Task<List<CandModel?>> GetFolderCandsList(int companyId, int folderId, List<int>? candsIds)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from cand in dbContext.candidates
                             join fc in dbContext.folders_cands on cand.id equals fc.candidate_id
                             join cvs in dbContext.cvs on cand.last_cv_id equals cvs.id
                             where fc.company_id == companyId
                                    && fc.folder_id == folderId
                             orderby cand.last_cv_sent descending
                             select new CandModel
                             {
                                 cvId = cvs.id,
                                 review = cand.review_text,
                                 keyId = cvs.key_id,
                                 candidateId = cand.id,
                                 email = cand.email,
                                 emailSubject = cvs.subject,
                                 firstName = cand.first_name,
                                 lastName = cand.last_name,
                                 phone = cand.phone,
                                 hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                                 cvSent = cvs.date_created,
                                 candFoldersIds = cand.folders_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.folders_ids),
                                 candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                                 posStages = cand.pos_stages == null ? null : JsonConvert.DeserializeObject<CandPosStageModel[]>(cand.pos_stages),
                             });

                return await query.ToListAsync();
            }
        }

        public async Task<int> AddCv(ImportCvModel importCv)
        {
            using (var dbContext = new cvup00001Context())
            {
                var newCv = new cv
                {
                    company_id = Convert.ToInt32(importCv.companyId),
                    candidate_id = importCv.candidateId,
                    cv_ascii_sum = importCv.cvAsciiSum,
                    email_id = importCv.emailId,
                    subject = importCv.subject,
                    from = importCv.from,
                    duplicate_cv_id = importCv.duplicateCvId,
                    position = Utils.Truncate(importCv.positionRelated, 250)
                };

                dbContext.cvs.Add(newCv);
                await dbContext.SaveChangesAsync();

                var cvTxt = new cvs_txt
                {
                    cv_id = newCv.id,
                    company_id = importCv.companyId,
                    cv_txt = importCv.cvTxt.Length > 7999 ? importCv.cvTxt.Substring(0, 7999) : importCv.cvTxt
                };

                dbContext.cvs_txts.Add(cvTxt);

                await dbContext.SaveChangesAsync();

                return newCv.id;
            }
        }

        public async Task UpdateCvKeyId(ImportCvModel importCv)
        {
            using (var dbContext = new cvup00001Context())
            {
                cv? cv = dbContext.cvs.Where(x => x.id == importCv.cvId).First();
                cv.key_id = importCv.cvKey;
                var result = dbContext.cvs.Update(cv);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<int> AddCandidate(candidate newCand)
        {
            using (var dbContext = new cvup00001Context())
            {
                dbContext.candidates.Add(newCand);
                await dbContext.SaveChangesAsync();
                return newCand.id;
            }
        }

        public async Task UpdateCandidate(candidate cand)
        {
            using (var dbContext = new cvup00001Context())
            {
                var result = dbContext.candidates.Update(cand);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<candidate?> GetCandidateByEmail(string email)
        {
            using (var dbContext = new cvup00001Context())
            {
                candidate? cand = await dbContext.candidates.Where(x => x.email == email).FirstOrDefaultAsync();
                return cand;
            }
        }

        public async Task<candidate?> GetCandidateByPhone(string phone)
        {
            using (var dbContext = new cvup00001Context())
            {
                candidate? cand = await dbContext.candidates.Where(x => x.phone == phone).FirstOrDefaultAsync();
                return cand;
            }
        }

        public async Task<List<CvsToIndexModel>> GetCompanyCvsToIndex(int companyId, int candidateId=0)
        {
            using (var dbContext = new cvup00001Context())
            {
                string sql = $@" SELECT cvs.company_id companyId, cvs.id cvId, cvs.candidate_id candidateId , ctx.cv_txt cvTxt, cnd.phone, cnd.email
                        , cvs.subject emailSubject, cnd.first_name firstName , cnd.last_name lastName , cnd.review_text reviewText
	                            FROM candidates cnd 
	                            INNER JOIN cvs ON cnd.id = cvs.candidate_id
	                            INNER JOIN cvs_txt ctx ON cvs.id = ctx.cv_id
	                            INNER JOIN (SELECT MAX(cvs.id) cv_id
		                             FROM cvs
		                             WHERE cvs.company_id={companyId}                
                                     GROUP BY cvs.candidate_id, cvs.cv_ascii_sum ) tbl ON cvs.id = tbl.cv_id ";
                    sql += candidateId > 0 ? " AND cnd.id = " + candidateId : ""; ;

                var cvsResults = await dbContext.cvsToIndexDB.FromSqlRaw(sql).ToListAsync();
                return cvsResults;


                //var query = from cand in dbContext.candidates
                //            join cvs in dbContext.cvs on cand.id equals cvs.candidate_id
                //            join cvTxt in dbContext.cvs_txts on cvs.id equals cvTxt.cv_id
                //            where cand.company_id == companyId
                //            select new CvPropsToIndexModel
                //            {
                //                companyId = companyId,
                //                cvId = cvs.id,
                //                candidateId = cand.id,
                //                cvTxt = cvTxt.cv_txt,
                //                email = cand.email,
                //                emailSubject = cvs.subject,
                //                firstName = cand.first_name,
                //                lastName = cand.last_name,
                //                reviewText = cand.review_text,
                //                phone = cand.phone,
                //            };

                //return await query.ToListAsync();
            }
        }

        public async Task<List<CandCvModel>> GetCandCvsList(int companyId, int candidateId, string encriptKey)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from cvs in dbContext.cvs
                             where cvs.company_id == companyId
                             && cvs.candidate_id == candidateId
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

            using (var dbContext = new cvup00001Context())
            {
                var query = from cvs in dbContext.cvs
                            join cvTxt in dbContext.cvs_txts on cvs.id equals cvTxt.cv_id
                            where cvs.company_id == companyId && cvs.cv_ascii_sum == null
                            select new CvPropsToIndexModel
                            {
                                id = cvs.id,
                                companyId = companyId,
                                candidateId = cvs.candidate_id,
                                cvTxt = cvTxt.cv_txt,
                                cvdbid = cvs.cvdbid,
                            };

                var itli = await query.ToListAsync();

                foreach (var item in itli)
                {
                    int docAsciiSum = 0;

                    if (item.cvTxt != null)
                    {
                        foreach (char c in item.cvTxt)
                        {
                            try
                            {
                                docAsciiSum += Convert.ToInt32(c);
                            }
                            catch (Exception) { }
                        }
                    }

                    var cvAscii = new cvs_ascii_sum
                    {
                        cv_id = item.id,
                        company_id = companyId,
                        candidate_id = item.candidateId,
                        ascii_sum = docAsciiSum,
                        cvdbid = item.cvdbid,
                    };

                    dbContext.cvs_ascii_sums.Add(cvAscii);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<List<int>> getPositionContactsIds(int companyId,int positionId)
        {
            using (var dbContext = new cvup00001Context())
            {
                return await dbContext.position_contacts
                     .Where(p => p.company_id == companyId && p.position_id == positionId)
                     .Select(p => p.contact_id)
                     .ToListAsync();
            }
        }

        public async Task<PositionModel> GetPosition(int companyId, int positionId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var inter = await dbContext.position_interviewers
                          .Where(p => p.company_id == companyId && p.position_id == positionId)
                          .Select(p => p.user_id)
                          .ToArrayAsync();

                var conts = await getPositionContactsIds(companyId, positionId);

                var query = from p in dbContext.positions
                            where p.id == positionId && p.company_id == companyId
                            join c in dbContext.customers on p.customer_id equals c.id
                            orderby p.name
                            select new PositionModel
                            {
                                id = p.id,
                                name = p.name,
                                descr = p.descr ?? "",
                                requirements = p.requirements ?? "",
                                customerId = p.customer_id ?? 0,
                                customerName = c.name,
                                status = Enum.Parse<PositionStatusEnum>(p.status),
                                interviewersIds = inter.ToArray(),
                                contactsIds = conts,
                                emailsubjectAddon=p.customer_pos_num,
                                updated=p.date_updated
                            };

                dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                return await query.FirstAsync();
            }
        }

        public async Task<position> AddPosition(PositionModel data, int companyId, int userId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var ent = new position
                {
                    company_id = companyId,
                    name = data.name,
                    descr = data.descr,
                    requirements = data.requirements,
                    status = data.status.ToString(),
                    opener_id = userId,
                    updater_id = userId,
                    date_created = DateTime.Now,
                    date_updated = DateTime.Now,
                };

                if (data.customerId > 0)
                {
                    ent.customer_id = data.customerId;
                }

                var result = dbContext.positions.Add(ent);
                await dbContext.SaveChangesAsync();

                return result.Entity;
            }
        }

        public async Task<position> UpdatePosition(PositionModel data, int companyId, int userId)
        {

            using (var dbContext = new cvup00001Context())
            {
                position? pos = dbContext.positions.Where(x => x.company_id == companyId && x.id == data.id).First();

                pos.name = data.name;
                pos.descr = data.descr;
                pos.requirements = data.requirements;
                pos.customer_id = data.customerId == 0 ? null : data.customerId;
                pos.status = data.status.ToString();
                pos.updater_id = userId;
                pos.date_updated = DateTime.Now;

                var result = dbContext.positions.Update(pos);
                await dbContext.SaveChangesAsync();

                return result.Entity;
            }
        }

        public async Task<List<PositionModel>> GetPositionsList(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from p in dbContext.positions
                             join c in dbContext.customers on p.customer_id equals c.id
                            where p.company_id == companyId
                            orderby p.date_updated descending
                            select new PositionModel
                            {
                                id = p.id,
                                name = p.name,
                                status = Enum.Parse<PositionStatusEnum>(p.status),
                                updated = p.date_updated,
                                customerName = c.name,
                                customerId = p.customer_id
                            };

                return await query.ToListAsync();
            }
        }

        public async Task DeletePosition(int companyId, int id)
        {
            using (var dbContext = new cvup00001Context())
            {
                var ent = (from p in dbContext.positions
                           where p.id == id && p.company_id == companyId
                           select p).FirstOrDefault();

                if (ent != null)
                {
                    var result = dbContext.positions.Remove(ent);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task AddUpdateInterviewers(int companyId, int positionId, int[] interviewersIds)
        {
            using (var dbContext = new cvup00001Context())
            {
                var dbInterviewer = await (from i in dbContext.position_interviewers
                                           where i.company_id == companyId && i.position_id == positionId
                                           select i).ToListAsync();

                foreach (var id in interviewersIds)
                {
                    if (dbInterviewer.Find(x => x.user_id == id) == null)
                    {
                        dbContext.position_interviewers.Add(new position_interviewer
                        {
                            company_id = companyId,
                            position_id = positionId,
                            user_id = id,
                        });
                    }
                }

                foreach (var item in dbInterviewer)
                {
                    if (Array.IndexOf(interviewersIds, item.user_id) == -1)
                    {
                        dbContext.position_interviewers.Remove(item);
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task AddUpdatePositionContacts(int companyId, int positionId, List<int>? contactsIds)
        {
            using (var dbContext = new cvup00001Context())
            {
                var conts = await (from i in dbContext.position_contacts
                                   where i.company_id == companyId && i.position_id == positionId
                                   select i).ToListAsync();

                foreach (var id in contactsIds)
                {
                    if (conts.Find(x => x.contact_id == id) == null)
                    {
                        dbContext.position_contacts.Add(new position_contact
                        {
                            company_id = companyId,
                            position_id = positionId,
                            contact_id = id,
                        });
                    }
                }

                foreach (var item in conts)
                {
                    int contId = contactsIds.Where(x => x == item.contact_id).FirstOrDefault();

                    if (contId == 0)
                    {
                        dbContext.position_contacts.Remove(item);
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<ParserRulesModel>> GetParsersRules(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from p in dbContext.company_parsers
                            join r in dbContext.parser_rules on p.parser_id equals r.parser_id
                            where p.company_id == companyId
                            select new ParserRulesModel
                            {
                                parser_id = r.parser_id,
                                delimiter = r.delimiter,
                                value_type = r.value_type,
                                must_metch = r.must_metch,
                                order = r.order,
                            };

                return await query.ToListAsync();
            }
        }

        public async Task<List<int>> GetCompaniesIds()
        {
            using (var dbContext = new cvup00001Context())
            {
                return await dbContext.companies.Select(c => c.id).ToListAsync();
            }
        }

        public async Task<List<cv>> GetCompanyCvs(int companyId)
        {
            try
            {
                using (var dbContext = new cvup00001Context())
                {
                    List<cv> cvs = await dbContext.cvs.Where(x => x.company_id == companyId ).ToListAsync();
                    return cvs;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<string?>> GetCompanyCvsIds(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                return await dbContext.cvs.Where(x => x.company_id == companyId).Select(c => c.key_id).ToListAsync();
            }
        }

        public async Task<CvModel?> GetCv(int cvId, int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from cand in dbContext.candidates
                            join cvs in dbContext.cvs on cand.id equals cvs.candidate_id
                            where cvs.id == cvId && cand.company_id == companyId
                            select new CvModel
                            {
                                candId = cand.id,
                                cvId = cvs.id,
                                //reviewHtml = cand.review_html,
                            };

                return await query.FirstOrDefaultAsync();
            }
        }

        public async Task SaveCandReview(int companyId, CandReviewModel candReview)
        {
            using (var dbContext = new cvup00001Context())
            {
                candidate? cand = dbContext.candidates.Where(x => x.company_id == companyId && x.id == candReview.candidateId).First();
                cand.review_text = candReview.review;
                var result = dbContext.candidates.Update(cand);
                await dbContext.SaveChangesAsync();
            }
        }


        public async Task<List<cv>> CheckIsCvDuplicate(int companyId, int candidateId, int cvAsciiSum)
        {
            using (var dbContext = new cvup00001Context())
            {
                List<cv> cvs = await dbContext.cvs.Where(x => x.company_id == companyId && x.candidate_id == candidateId && x.cv_ascii_sum == cvAsciiSum).ToListAsync();
                return cvs;
            }
        }

        public async Task UpdateCandidateLastCv(ImportCvModel importCv)
        {
            using (var dbContext = new cvup00001Context())
            {
                candidate cand = dbContext.candidates.Where(x => x.id == importCv.candidateId).First();
                cand.has_duplicates_cvs = (sbyte?)(importCv.isDuplicate ? 1 : 0);
                cand.last_cv_id = importCv.cvId;
                cand.last_cv_sent = DateTime.Now;
                cand.date_updated = DateTime.Now;
                var result = dbContext.candidates.Update(cand);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateSameCv(ImportCvModel importCv)
        {
            using (var dbContext = new cvup00001Context())
            {
                cv cv = dbContext.cvs.Where(x => x.id == importCv.cvId).First();
                cv.date_created = DateTime.Now;
                var result = dbContext.cvs.Update(cv);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<int>> AttachPosCandCv(AttachePosCandCvModel posCandCv)
        {
            using (var dbContext = new cvup00001Context())
            {
                position_candidate? posCand = await dbContext.position_candidates.Where(x => x.company_id == posCandCv.companyId
                  && x.position_id == posCandCv.positionId
                  && x.candidate_id == posCandCv.candidateId).FirstOrDefaultAsync();

                string? posCvsStr = null;
                List<PosCvsModel>? posCvs = null;

                if (posCand != null && posCand.cand_cvs != null)
                {
                    posCvs = JsonConvert.DeserializeObject<List<PosCvsModel>>(posCand.cand_cvs);
                }

                if (posCvs == null)
                    posCvs = new List<PosCvsModel>();

                posCvs.Add(new PosCvsModel { cvId = posCandCv.cvId, isSentByEmail = false, keyId = posCandCv.keyId });
                posCvsStr = JsonConvert.SerializeObject(posCvs);

                var newPosCv = new position_candidate
                {
                    company_id = posCandCv.companyId,
                    position_id = posCandCv.positionId,
                    candidate_id = posCandCv.candidateId,
                    cv_id = posCandCv.cvId,
                    stage_id = 1,
                    cand_cvs = posCvsStr,
                    stage_date = DateTime.Now,
                    stage_type = "attached_to_position"
                };

                dbContext.position_candidates.Add(newPosCv);
                await dbContext.SaveChangesAsync();
            }

            return await UpdateCandPosCv(posCandCv.companyId, posCandCv.candidateId, posCandCv.cvId);
        }

        public async Task<List<int>> DetachPosCand(AttachePosCandCvModel posCandCv)
        {
            using (var dbContext = new cvup00001Context())
            {
                position_candidate? posCvs = dbContext.position_candidates.Where(x => x.company_id == posCandCv.companyId
                    && x.position_id == posCandCv.positionId
                    && x.candidate_id == posCandCv.candidateId
                    && x.cv_id == posCandCv.cvId).FirstOrDefault();

                if (posCvs != null)
                {
                    dbContext.position_candidates.Remove(posCvs);
                    await dbContext.SaveChangesAsync();
                }
            }

            return await UpdateCandPosCv(posCandCv.companyId, posCandCv.candidateId, posCandCv.cvId);
        }

        private async Task<List<int>> UpdateCandPosCv(int companyId, int candidateId, int cvId)
        {
            using (var dbContext = new cvup00001Context())
            {
                List<position_candidate>? candPosList = await dbContext.position_candidates.Where(x => x.company_id == companyId
                   && x.candidate_id == candidateId).ToListAsync();

                List<CandPosStageModel> candPosStages = new List<CandPosStageModel>();
                List<int> candPos = new List<int>();
                List<int> cvPos = new List<int>();

                foreach (var pcv in candPosList)
                {
                    candPos.Add(pcv.position_id);
                    candPosStages.Add(new CandPosStageModel { d = pcv.stage_date?.ToString("yyyy-MM-dd"), t = pcv.stage_type, id = pcv.position_id });

                    if (pcv.cv_id == cvId)
                    {
                        cvPos.Add(pcv.position_id);
                    }
                }

                var candPosStagesJson = JsonConvert.SerializeObject(candPosStages);

                candidate? cand = dbContext.candidates.Where(x => x.company_id == companyId && x.id == candidateId).FirstOrDefault();

                if (cand != null)
                {
                    cand.pos_ids = $"[{string.Join(",", candPos)}]";
                    cand.pos_stages = candPosStagesJson;
                    var result = dbContext.candidates.Update(cand);
                }

                //cv? cv = dbContext.cvs.Where(x => x.company_id == companyId && x.candidate_id == candidateId && x.id == cvId).FirstOrDefault();

                //if (cv != null)
                //{
                //    cv.pos_ids = $"[{string.Join(",", cvPos)}]";
                //    var result = dbContext.cvs.Update(cv);
                //}

                await dbContext.SaveChangesAsync();

                return  candPos;
            }

        }

        public async Task UpdateCandPositionStatus(CandPosStatusUpdateCvModel posStatus)
        {
            using (var dbContext = new cvup00001Context())
            {
                position_candidate? candPos = await dbContext.position_candidates.Where(x => x.company_id == posStatus.companyId
                  && x.candidate_id == posStatus.candidateId && x.position_id == posStatus.positionId).FirstOrDefaultAsync();

                if (candPos != null)
                {
                    candPos.stage_type = posStatus.stageType;
                    candPos.stage_date = DateTime.Now;
                    candPos.date_updated = DateTime.Now;

                    dbContext.position_candidates.Update(candPos);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<List<company_cvs_email>> GetCompaniesEmails()
        {
            using (var dbContext = new cvup00001Context())
            {
                return await dbContext.company_cvs_emails.ToListAsync();
            }
        }
      
        public async  Task<List<companyStagesTypesModel>> GetCompanyStagesTypes(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from st in dbContext.company_stages_types
                             where st.company_id == companyId
                             orderby st.order
                             select new companyStagesTypesModel
                             {
                                 name = st.name,
                                 stageType = st.stage_Type,
                                 order = st.order,
                                 isCustom = Convert.ToBoolean(st.is_custom),
                                 color = st.color
                             });

                return await query.ToListAsync();
            }
        }

        public async  Task<List<EmailTemplateModel>> GetEmailTemplates(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from et in dbContext.emails_templates
                             orderby et.name
                             select new EmailTemplateModel
                             {
                                 id = et.id,
                                 name = et.name,
                                  subject= et.subject,
                                   body= et.body,
                             });

                return await query.ToListAsync();
            }
        }

        public async Task AddUpdateEmailTemplate(EmailTemplateModel emailTemplate)
        {
            using (var dbContext = new cvup00001Context())
            {
                if (emailTemplate.id == 0)
                {
                    dbContext.emails_templates.Add(new emails_template { name= emailTemplate.name, subject= emailTemplate.subject, body= emailTemplate.body});
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    emails_template? etm = dbContext.emails_templates.Where(x => x.id == emailTemplate.id).First();
                    etm.name= emailTemplate.name;
                    etm.subject= emailTemplate.subject;
                    etm.body= emailTemplate.body;

                    var result = dbContext.emails_templates.Update(etm);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
        public async Task DeleteEmailTemplate(int companyId, int id)
        {
            using (var dbContext = new cvup00001Context())
            {
                emails_template? etm = dbContext.emails_templates.Where(x => x.id == id).FirstOrDefault();

                if (etm != null)
                {
                    dbContext.emails_templates.Remove(etm);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateCandDetails(CandDetailsModel candDetails)
        {
            using (var dbContext = new cvup00001Context())
            {
                candidate? cand = dbContext.candidates.Where(x => x.id == candDetails.candidateId).FirstOrDefault();

                if (cand != null)
                {
                    cand.first_name = candDetails.firstName;
                    cand.last_name = candDetails.lastName;
                    cand.email = candDetails.email;
                    cand.phone = candDetails.phone;
                    var result = dbContext.candidates.Update(cand);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

    }
}
