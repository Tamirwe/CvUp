using Database.models;
using DataModelsLibrary.Models;
using GeneralLibrary;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DataModelsLibrary.Queries
{
    public class CandsPositionsQueries : ICandsPositionsQueries
    {

        public CandsPositionsQueries()
        {
        }

        public int AddCv(ImportCvModel importCv)
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
                dbContext.SaveChanges();

                var cvTxt = new cvs_txt
                {
                    cv_id = newCv.id,
                    company_id = importCv.companyId,
                    cv_txt = importCv.cvTxt.Length > 7999 ? importCv.cvTxt.Substring(0, 7999) : importCv.cvTxt
                };

                dbContext.cvs_txts.Add(cvTxt);

                dbContext.SaveChanges();

                return newCv.id;
            }
        }

        public void UpdateCvKeyId(ImportCvModel importCv)
        {
            using (var dbContext = new cvup00001Context())
            {
                cv? cv = dbContext.cvs.Where(x => x.id == importCv.cvId).First();
                cv.key_id = importCv.cvKey;
                var result = dbContext.cvs.Update(cv);
                dbContext.SaveChanges();
            }
        }

        public int AddCandidate(candidate newCand)
        {
            using (var dbContext = new cvup00001Context())
            {
                dbContext.candidates.Add(newCand);
                dbContext.SaveChanges();
                return newCand.id;
            }
        }

        public void UpdateCandidate(candidate cand)
        {
            using (var dbContext = new cvup00001Context())
            {
                var result = dbContext.candidates.Update(cand);
                dbContext.SaveChanges();
            }
        }

        public candidate? GetCandidateByEmail(string email)
        {
            using (var dbContext = new cvup00001Context())
            {
                candidate? cand = dbContext.candidates.Where(x => x.email == email).FirstOrDefault();
                return cand;
            }
        }

        public candidate? GetCandidateByPhone(string phone)
        {
            using (var dbContext = new cvup00001Context())
            {
                candidate? cand = dbContext.candidates.Where(x => x.phone == phone).FirstOrDefault();
                return cand;
            }
        }

        public List<CvPropsToIndexModel> GetCompanyCvsToIndex(int companyId)
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
                                cvTxt = cvTxt.cv_txt,
                                email = cand.email,
                                emailSubject = cvs.subject,
                                candName = cand.name,
                                reviewText = cand.review_text,
                                phone = cand.phone,
                            };

                return query.ToList();
            }
        }

        public List<CandModel> GetCandsList(int companyId, string encriptKey, int page, int take, int positionId, string? searchKeyWords)
        {
            int skip = (page - 1) * take;
            using (var dbContext = new cvup00001Context())
            {
                var query = (from cand in dbContext.candidates
                             join cvs in dbContext.cvs on cand.last_cv_id equals cvs.id
                             where cand.company_id == companyId
                             orderby cand.last_cv_sent descending
                             select new CandModel
                             {
                                 cvId = cvs.id,
                                 //keyId = Encriptor.Encrypt($"{cvs.key_id}~{DateTime.Now.ToString("yyyy-MM-dd")}", encriptKey),
                                 keyId= cvs.key_id,
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

                return query.ToList();
            }
        }

        public List<CandModel> GetCandCvsList(int companyId, int candidateId, string encriptKey)
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
                                 //keyId = Encriptor.Encrypt($"{cvs.key_id}~{DateTime.Now.ToString("yyyy-MM-dd")}", encriptKey),
                                 keyId= cvs.key_id,
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

                return query.ToList();
            }
        }

        public List<CandModel> GetPosCandsList(int companyId, int positionId, string encriptKey)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = (from cand in dbContext.candidates
                             join cvs in dbContext.cvs on cand.id equals cvs.candidate_id
                             join pcv in dbContext.position_candidates on cvs.id equals pcv.cv_id
                             where pcv.company_id == companyId
                             && pcv.position_id == positionId
                             orderby cand.last_cv_sent descending
                             select new CandModel
                             {
                                 cvId = cvs.id,
                                 //keyId = Encriptor.Encrypt($"{cvs.key_id}~{DateTime.Now.ToString("yyyy-MM-dd")}", encriptKey),
                                 keyId= cvs.key_id,
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
                             });

                return query.ToList();
            }
        }

        public department AddDepartment(IdNameModel data, int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var dep = new department
                {
                    name = data.name,
                    company_id = companyId
                };

                var result = dbContext.departments.Add(dep);
                dbContext.SaveChanges();
                return result.Entity;
            }
        }

        public department? UpdateDepartment(IdNameModel data, int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                department dep = new department { id = data.id, name = data.name, company_id = companyId };
                var result = dbContext.departments.Update(dep);
                dbContext.SaveChanges();
                return result.Entity;
            }
        }

        public List<IdNameModel> GetDepartmentsList(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from dep in dbContext.departments
                            where dep.company_id == companyId
                            orderby dep.name
                            select new IdNameModel
                            {
                                id = dep.id,
                                name = dep.name,
                            };

                return query.ToList();
            }
        }

        public void DeleteDepartment(int companyId, int id)
        {
            using (var dbContext = new cvup00001Context())
            {
                var dep = (from d in dbContext.departments
                           where d.id == id && d.company_id == companyId
                           select d).FirstOrDefault();

                if (dep != null)
                {
                    var result = dbContext.departments.Remove(dep);
                    dbContext.SaveChanges();
                }
            }
        }

        public hr_company AddHrCompany(IdNameModel data, int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var hr = new hr_company
                {
                    name = data.name,
                    company_id = companyId
                };

                var result = dbContext.hr_companies.Add(hr);
                dbContext.SaveChanges();
                return result.Entity;
            }
        }

        public hr_company? UpdateHrCompany(IdNameModel data, int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                hr_company hr = new hr_company { id = data.id, name = data.name, company_id = companyId };
                var result = dbContext.hr_companies.Update(hr);
                dbContext.SaveChanges();
                return result.Entity;
            }
        }

        public List<IdNameModel> GetHrCompaniesList(int companyId)
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

                return query.ToList();
            }
        }

        public void DeleteHrCompany(int companyId, int id)
        {
            using (var dbContext = new cvup00001Context())
            {
                var hr = (from h in dbContext.hr_companies
                          where h.id == id && h.company_id == companyId
                          select h).FirstOrDefault();

                if (hr != null)
                {
                    var result = dbContext.hr_companies.Remove(hr);
                    dbContext.SaveChanges();
                }
            }
        }

        public PositionClientModel GetPosition(int companyId, int positionId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var hrs = dbContext.position_hr_companies
                            .Where(p => p.company_id == companyId && p.position_id == positionId)
                            .Select(p => p.hr_company_id)
                            .ToArray();

                var inter = dbContext.position_interviewers
                          .Where(p => p.company_id == companyId && p.position_id == positionId)
                          .Select(p => p.user_id)
                          .ToArray();

                var query = from p in dbContext.positions
                            where p.id == positionId && p.company_id == companyId
                            orderby p.name
                            select new PositionClientModel
                            {
                                id = p.id,
                                name = p.name,
                                descr = p.descr ?? "",
                                companyId = companyId,
                                departmentId = p.department_id ?? 0,
                                isActive = Convert.ToBoolean(p.is_active),
                                hrCompaniesIds = hrs.ToArray(),
                                interviewersIds = inter.ToArray()
                            };

                dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                return query.First();
            }
        }

        public position AddPosition(PositionClientModel data, int companyId, int userId)
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

                if (data.departmentId > 0)
                {
                    ent.department_id = data.departmentId;
                }

                var result = dbContext.positions.Add(ent);
                dbContext.SaveChanges();

                AddHrCompanies(companyId, result.Entity.id, data.hrCompaniesIds);
                AddInterviewers(companyId, result.Entity.id, data.interviewersIds);

                return result.Entity;
            }
        }

        public position? UpdatePosition(PositionClientModel data, int companyId, int userId)
        {
            using (var dbContext = new cvup00001Context())
            {
                position ent = new position
                {
                    id = data.id,
                    company_id = companyId,
                    name = data.name,
                    descr = data.descr,
                    department_id = data.departmentId == 0 ? null : data.departmentId,
                    is_active = Convert.ToSByte(data.isActive),
                    updater_id = userId,
                    date_created = DateTime.Now,
                    date_updated = DateTime.Now,
                };

                var result = dbContext.positions.Update(ent);
                dbContext.SaveChanges();

                UpdateHrCompanies(companyId, data.id, data.hrCompaniesIds);
                UpdateInterviewers(companyId, data.id, data.interviewersIds);

                return result.Entity;
            }
        }

        public List<PositionModel> GetPositionsList(int companyId)
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

                return query.ToList();
            }
        }

        public void DeletePosition(int companyId, int id)
        {
            using (var dbContext = new cvup00001Context())
            {
                var ent = (from p in dbContext.positions
                           where p.id == id && p.company_id == companyId
                           select p).FirstOrDefault();

                if (ent != null)
                {
                    var result = dbContext.positions.Remove(ent);
                    dbContext.SaveChanges();
                }
            }
        }

        private void UpdateHrCompanies(int companyId, int positionId, int[] hrCompaniesIds)
        {
            using (var dbContext = new cvup00001Context())
            {
                var dbHrs = (from h in dbContext.position_hr_companies
                             where h.company_id == companyId && h.position_id == positionId
                             select h).ToList();

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

                dbContext.SaveChanges();
            }
        }

        private void UpdateInterviewers(int companyId, int positionId, int[] interviewersIds)
        {
            using (var dbContext = new cvup00001Context())
            {
                var dbInterviewer = (from i in dbContext.position_interviewers
                                     where i.company_id == companyId && i.position_id == positionId
                                     select i).ToList();

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

                dbContext.SaveChanges();
            }
        }

        private void AddHrCompanies(int companyId, int positionId, int[] hrCompaniesIds)
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

                dbContext.SaveChanges();
            }
        }

        private void AddInterviewers(int companyId, int positionId, int[] interviewersIds)
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

                dbContext.SaveChanges();
            }
        }

        public List<ParserRulesModel> GetParsersRules(int companyId)
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

                return query.ToList();
            }
        }

        public List<int> GetCompaniesIds()
        {
            using (var dbContext = new cvup00001Context())
            {
                return dbContext.companies.Select(c => c.id).ToList();
            }
        }

        public List<string?> GetCompanyCvsIds(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                return dbContext.cvs.Where(x => x.company_id == companyId).Select(c => c.key_id).ToList();
            }
        }

        public CvModel? GetCv(int cvId, int companyId)
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

                return query.FirstOrDefault();
            }
        }

        public void SaveCvReview(CvReviewModel cvReview)
        {
            using (var dbContext = new cvup00001Context())
            {
                candidate? cand = dbContext.candidates.Where(x => x.id == cvReview.candidateId).First();
                cand.review_html = cvReview.reviewHtml;
                cand.review_text = cvReview.reviewText;
                var result = dbContext.candidates.Update(cand);
                dbContext.SaveChanges();
            }
        }

        public List<cv> CheckIsCvDuplicate(int companyId, int candidateId, int cvAsciiSum)
        {
            using (var dbContext = new cvup00001Context())
            {
                List<cv> cvs = dbContext.cvs.Where(x => x.company_id == companyId && x.candidate_id == candidateId && x.cv_ascii_sum == cvAsciiSum).ToList();
                return cvs;
            }
        }

        public void UpdateCandidateLastCv(ImportCvModel importCv)
        {
            using (var dbContext = new cvup00001Context())
            {
                candidate cand = dbContext.candidates.Where(x => x.id == importCv.candidateId).First();
                cand.has_duplicates_cvs = (sbyte?)(importCv.isDuplicate ? 1 : 0);
                cand.last_cv_id = importCv.cvId;
                cand.last_cv_sent = DateTime.Now;
                cand.date_updated = DateTime.Now;
                var result = dbContext.candidates.Update(cand);
                dbContext.SaveChanges();
            }
        }

        public void UpdateSameCv(ImportCvModel importCv)
        {
            using (var dbContext = new cvup00001Context())
            {
                cv cv = dbContext.cvs.Where(x => x.id == importCv.cvId).First();
                cv.date_created = DateTime.Now;
                var result = dbContext.cvs.Update(cv);
                dbContext.SaveChanges();
            }
        }

        public CandPosModel AttachPosCandCv(AttachePosCandCvModel posCandCv)
        {
            using (var dbContext = new cvup00001Context())
            {
                position_candidate? posCand = dbContext.position_candidates.Where(x => x.company_id == posCandCv.companyId
                  && x.position_id == posCandCv.positionId
                  && x.candidate_id == posCandCv.candidateId).FirstOrDefault();

                string? posCvsStr = null;
                List<PosCvsModel>? posCvs = null;

                if (posCand != null && posCand.cvs != null)
                {
                    posCvs = JsonConvert.DeserializeObject<List<PosCvsModel>>(posCand.cvs);
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
                    cvs = posCvsStr
                };

                dbContext.position_candidates.Add(newPosCv);
                dbContext.SaveChanges();
            }

            return UpdateCandPosCv(posCandCv.companyId, posCandCv.candidateId, posCandCv.cvId);
        }

        public CandPosModel DetachPosCand(AttachePosCandCvModel posCandCv)
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
                    dbContext.SaveChanges();
                }
            }

            return UpdateCandPosCv(posCandCv.companyId, posCandCv.candidateId, posCandCv.cvId);
        }

        private CandPosModel UpdateCandPosCv(int companyId, int candidateId, int cvId)
        {
            using (var dbContext = new cvup00001Context())
            {
                List<position_candidate>? candPosList = dbContext.position_candidates.Where(x => x.company_id == companyId
                   && x.candidate_id == candidateId).ToList();

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

                dbContext.SaveChanges();

                return new CandPosModel { candPosIds = candPos, cvPosIds = cvPos };
            }

        }

        public List<company_cvs_email> GetCompaniesEmails()
        {
            using (var dbContext = new cvup00001Context())
            {
                return dbContext.company_cvs_emails.ToList();
            }
        }
    }
}
