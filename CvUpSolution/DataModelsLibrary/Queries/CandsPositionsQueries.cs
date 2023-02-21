using Database.models;
using DataModelsLibrary.Models;
using GeneralLibrary;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace DataModelsLibrary.Queries
{
    public class CandsPositionsQueries : ICandsPositionsQueries
    {

        public CandsPositionsQueries()
        {
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

        public async Task<List<CvPropsToIndexModel>> GetCompanyCvsToIndex(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from cand in dbContext.candidates
                            join cvs in dbContext.cvs on cand.id equals cvs.candidate_id
                            join cvTxt in dbContext.cvs_txts on cvs.id equals cvTxt.cv_id
                            where cand.company_id == companyId
                            select new CvPropsToIndexModel
                            {
                                companyId = companyId,
                                cvId = cvs.id,
                                candidateId = cand.id,
                                cvTxt = cvTxt.cv_txt,
                                email = cand.email,
                                emailSubject = cvs.subject,
                                candName = cand.name,
                                reviewText = cand.review_text,
                                phone = cand.phone,
                            };

                return await query.ToListAsync();
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
                                 review = cand.review_html,
                                 //keyId = Encriptor.Encrypt($"{cvs.key_id}~{DateTime.Now.ToString("yyyy-MM-dd")}", encriptKey),
                                 keyId = cvs.key_id,
                                 //fileType = cvs.key_id != null ? cvs.key_id.Substring(cvs.key_id.LastIndexOf('_')) : "",
                                 candidateId = cand.id,
                                 email = cand.email,
                                 emailSubject = cvs.subject,
                                 candidateName = cand.name,
                                 phone = cand.phone,
                                 hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                                 cvSent = Convert.ToDateTime(cand.last_cv_sent),
                                 candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                                 cvPosIds = cvs.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cvs.pos_ids),
                             }).Skip(skip).Take(take);

                var result = await query.ToListAsync();
                return result;
            }
        }

        public async Task<List<CandModel>> GetCandCvsList(int companyId, int candidateId, string encriptKey)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from cand in dbContext.candidates
                             join cvs in dbContext.cvs on cand.id equals cvs.candidate_id
                             where cand.company_id == companyId
                             && cand.id == candidateId
                             orderby cand.last_cv_sent descending
                             select new CandModel
                             {
                                 cvId = cvs.id,
                                 review = cand.review_html,
                                 //keyId = Encriptor.Encrypt($"{cvs.key_id}~{DateTime.Now.ToString("yyyy-MM-dd")}", encriptKey),
                                 keyId = cvs.key_id,
                                 //fileType = cvs.key_id != null ? cvs.key_id.Substring(cvs.key_id.LastIndexOf('_')) : "",
                                 candidateId = cand.id,
                                 email = cand.email,
                                 emailSubject = cvs.subject,
                                 candidateName = cand.name,
                                 phone = cand.phone,
                                 hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                                 cvSent = cvs.date_created,
                                 candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                                 cvPosIds = cvs.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cvs.pos_ids),
                             });

                return await query.ToListAsync();
            }
        }

        public async Task<List<CandModel>> GetPosCandsList(int companyId, int positionId, string encriptKey)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from pcv in dbContext.position_candidates
                             join cand in dbContext.candidates on pcv.candidate_id equals cand.id
                             join cvs in dbContext.cvs on pcv.cv_id equals cvs.id
                             where pcv.company_id == companyId
                                    && pcv.position_id == positionId
                             select new CandModel
                             {
                                 cvId = cvs.id,
                                 review = cand.review_html,
                                 //keyId = Encriptor.Encrypt($"{cvs.key_id}~{DateTime.Now.ToString("yyyy-MM-dd")}", encriptKey),
                                 keyId = cvs.key_id,
                                 //fileType = cvs.key_id != null ? cvs.key_id.Substring(cvs.key_id.LastIndexOf('_')) : "",
                                 candidateId = cand.id,
                                 email = cand.email,
                                 emailSubject = cvs.subject,
                                 candidateName = cand.name,
                                 phone = cand.phone,
                                 hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                                 cvSent = cvs.date_created,
                                 candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                                 cvPosIds = cvs.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cvs.pos_ids),
                                 stageId = pcv.stage_id,
                                 dateAttached = pcv.date_created,
                                 candCvs = pcv.cand_cvs == null ? new List<PosCandCvsModel> { } : JsonConvert.DeserializeObject<List<PosCandCvsModel>>(pcv.cand_cvs),

                             });

                return await query.ToListAsync();
            }
        }

        public async Task<customer> AddCustomer(IdNameModel data, int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var dep = new customer
                {
                    name = data.name,
                    company_id = companyId
                };

                var result = dbContext.customers.Add(dep);
                await dbContext.SaveChangesAsync();
                return result.Entity;
            }
        }

        public async Task<customer?> UpdateCustomer(IdNameModel data, int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                customer dep = new customer { id = data.id, name = data.name, company_id = companyId };
                var result = dbContext.customers.Update(dep);
                await dbContext.SaveChangesAsync();
                return result.Entity;
            }
        }

        public async Task<List<IdNameModel>> GetCustomersList(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from dep in dbContext.customers
                            where dep.company_id == companyId
                            orderby dep.name
                            select new IdNameModel
                            {
                                id = dep.id,
                                name = dep.name,
                            };

                return await query.ToListAsync();
            }
        }

        public async Task DeleteCustomer(int companyId, int id)
        {
            using (var dbContext = new cvup00001Context())
            {
                var dep = await (from d in dbContext.customers
                                 where d.id == id && d.company_id == companyId
                                 select d).FirstOrDefaultAsync();

                if (dep != null)
                {
                    var result = dbContext.customers.Remove(dep);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<hr_company> AddHrCompany(IdNameModel data, int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var hr = new hr_company
                {
                    name = data.name,
                    company_id = companyId
                };

                var result = dbContext.hr_companies.Add(hr);
                await dbContext.SaveChangesAsync();
                return result.Entity;
            }
        }

        public async Task<hr_company?> UpdateHrCompany(IdNameModel data, int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                hr_company hr = new hr_company { id = data.id, name = data.name, company_id = companyId };
                var result = dbContext.hr_companies.Update(hr);
                await dbContext.SaveChangesAsync();
                return result.Entity;
            }
        }

        public async Task<List<IdNameModel>> GetHrCompaniesList(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from hr in dbContext.hr_companies
                            where hr.company_id == companyId
                            orderby hr.name
                            select new IdNameModel
                            {
                                id = hr.id,
                                name = hr.name,
                            };

                return await query.ToListAsync();
            }
        }

        public async Task DeleteHrCompany(int companyId, int id)
        {
            using (var dbContext = new cvup00001Context())
            {
                var hr = await (from h in dbContext.hr_companies
                                where h.id == id && h.company_id == companyId
                                select h).FirstOrDefaultAsync();

                if (hr != null)
                {
                    var result = dbContext.hr_companies.Remove(hr);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<PositionClientModel> GetPosition(int companyId, int positionId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var hrs = await dbContext.position_hr_companies
                            .Where(p => p.company_id == companyId && p.position_id == positionId)
                            .Select(p => p.hr_company_id)
                            .ToArrayAsync();

                var inter = await dbContext.position_interviewers
                          .Where(p => p.company_id == companyId && p.position_id == positionId)
                          .Select(p => p.user_id)
                          .ToArrayAsync();

                var query = from p in dbContext.positions
                            where p.id == positionId && p.company_id == companyId
                            orderby p.name
                            select new PositionClientModel
                            {
                                id = p.id,
                                name = p.name,
                                descr = p.descr ?? "",
                                companyId = companyId,
                                customerId = p.customer_id ?? 0,
                                isActive = Convert.ToBoolean(p.is_active),
                                hrCompaniesIds = hrs.ToArray(),
                                interviewersIds = inter.ToArray()
                            };

                dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                return await query.FirstAsync();
            }
        }

        public async Task<position> AddPosition(PositionClientModel data, int companyId, int userId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var ent = new position
                {
                    company_id = companyId,
                    name = data.name,
                    descr = data.descr,
                    is_active = Convert.ToSByte(data.isActive),
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

                await AddHrCompanies(companyId, result.Entity.id, data.hrCompaniesIds);
                await AddInterviewers(companyId, result.Entity.id, data.interviewersIds);

                return result.Entity;
            }
        }

        public async Task<position?> UpdatePosition(PositionClientModel data, int companyId, int userId)
        {
            using (var dbContext = new cvup00001Context())
            {
                position ent = new position
                {
                    id = data.id,
                    company_id = companyId,
                    name = data.name,
                    descr = data.descr,
                    customer_id = data.customerId == 0 ? null : data.customerId,
                    is_active = Convert.ToSByte(data.isActive),
                    updater_id = userId,
                    date_created = DateTime.Now,
                    date_updated = DateTime.Now,
                };

                var result = dbContext.positions.Update(ent);
                await dbContext.SaveChangesAsync();

                UpdateHrCompanies(companyId, data.id, data.hrCompaniesIds);
                UpdateInterviewers(companyId, data.id, data.interviewersIds);

                return result.Entity;
            }
        }

        public async Task<List<PositionModel>> GetPositionsList(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from p in dbContext.positions
                            where p.company_id == companyId
                            orderby p.name
                            select new PositionModel
                            {
                                id = p.id,
                                name = p.name,
                                isActive = Convert.ToBoolean(p.is_active),
                                updated = p.date_updated,
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

        private async Task UpdateHrCompanies(int companyId, int positionId, int[] hrCompaniesIds)
        {
            using (var dbContext = new cvup00001Context())
            {
                var dbHrs = await (from h in dbContext.position_hr_companies
                                   where h.company_id == companyId && h.position_id == positionId
                                   select h).ToListAsync();

                foreach (var id in hrCompaniesIds)
                {
                    if (dbHrs.Find(x => x.hr_company_id == id) == null)
                    {
                        dbContext.position_hr_companies.Add(new position_hr_company
                        {
                            company_id = companyId,
                            position_id = positionId,
                            hr_company_id = id
                        });
                    }
                }

                foreach (var item in dbHrs)
                {
                    if (Array.IndexOf(hrCompaniesIds, item.hr_company_id) == -1)
                    {
                        dbContext.position_hr_companies.Remove(item);
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }

        private async Task UpdateInterviewers(int companyId, int positionId, int[] interviewersIds)
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

        private async Task AddHrCompanies(int companyId, int positionId, int[] hrCompaniesIds)
        {
            using (var dbContext = new cvup00001Context())
            {
                foreach (var item in hrCompaniesIds)
                {
                    var hr = new position_hr_company
                    {
                        company_id = companyId,
                        hr_company_id = item,
                        position_id = positionId
                    };

                    dbContext.position_hr_companies.Add(hr);
                }

                await dbContext.SaveChangesAsync();
            }
        }

        private async Task AddInterviewers(int companyId, int positionId, int[] interviewersIds)
        {
            using (var dbContext = new cvup00001Context())
            {
                foreach (var item in interviewersIds)
                {
                    var interviewer = new position_interviewer
                    {
                        company_id = companyId,
                        user_id = item,
                        position_id = positionId
                    };

                    dbContext.position_interviewers.Add(interviewer);
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
                                reviewHtml = cand.review_html,
                            };

                return await query.FirstOrDefaultAsync();
            }
        }

        public async Task SaveCvReview(CvReviewModel cvReview)
        {
            using (var dbContext = new cvup00001Context())
            {
                candidate? cand = dbContext.candidates.Where(x => x.id == cvReview.candidateId).First();
                cand.review_html = cvReview.reviewHtml;
                cand.review_text = cvReview.reviewText;
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

        public async Task<CandPosModel> AttachPosCandCv(AttachePosCandCvModel posCandCv)
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
                    cand_cvs = posCvsStr
                };

                dbContext.position_candidates.Add(newPosCv);
                await dbContext.SaveChangesAsync();
            }

            return await UpdateCandPosCv(posCandCv.companyId, posCandCv.candidateId, posCandCv.cvId);
        }

        public async Task<CandPosModel> DetachPosCand(AttachePosCandCvModel posCandCv)
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

        private async Task<CandPosModel> UpdateCandPosCv(int companyId, int candidateId, int cvId)
        {
            using (var dbContext = new cvup00001Context())
            {
                List<position_candidate>? candPosList = await dbContext.position_candidates.Where(x => x.company_id == companyId
                   && x.candidate_id == candidateId).ToListAsync();

                List<int> candPos = new List<int>();
                List<int> cvPos = new List<int>();

                foreach (var pcv in candPosList)
                {
                    candPos.Add(pcv.position_id);

                    if (pcv.cv_id == cvId)
                    {
                        cvPos.Add(pcv.position_id);
                    }
                }

                candidate? cand = dbContext.candidates.Where(x => x.company_id == companyId && x.id == candidateId).FirstOrDefault();

                if (cand != null)
                {
                    cand.pos_ids = $"[{string.Join(",", candPos)}]";
                    var result = dbContext.candidates.Update(cand);
                }

                cv? cv = dbContext.cvs.Where(x => x.company_id == companyId && x.candidate_id == candidateId && x.id == cvId).FirstOrDefault();

                if (cv != null)
                {
                    cv.pos_ids = $"[{string.Join(",", cvPos)}]";
                    var result = dbContext.cvs.Update(cv);
                }

                await dbContext.SaveChangesAsync();

                return new CandPosModel { candPosIds = candPos, cvPosIds = cvPos };
            }

        }

        public async Task<List<company_cvs_email>> GetCompaniesEmails()
        {
            using (var dbContext = new cvup00001Context())
            {
                return await dbContext.company_cvs_emails.ToListAsync();
            }
        }
    }
}
