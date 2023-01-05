using Database.models;
using DataModelsLibrary.Models;
using GeneralLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DataModelsLibrary.Queries
{
    public class CvsPositionsQueries : ICvsPositionsQueries
    {

        public CvsPositionsQueries()
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
                                candidateName = cand.name,
                                candidateOpinion = cand.opinion,
                                phone = cand.phone,
                            };

                return query.ToList();
            }
        }

        public List<CvListItemModel> GetCvsList(int companyId, string encriptKey)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from cand in dbContext.candidates
                            join cvs in dbContext.cvs on cand.id equals cvs.candidate_id
                            where cand.company_id == companyId
                            select new CvListItemModel
                            {
                                cvId = Encriptor.Encrypt($"{cvs.key_id}~{DateTime.Now.ToString("yyyy-MM-dd")}", encriptKey),
                                fileType = cvs.key_id != null ? cvs.key_id.Substring(cvs.key_id.LastIndexOf('_')) : "",
                                candidateId = cand.id,
                                email = cand.email,
                                emailSubject = cvs.subject,
                                candidateName = cand.name,
                                phone = cand.phone,
                            };

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

        public List<PositionListItemModel> GetPositionsList(int companyId)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from p in dbContext.positions
                            where p.company_id == companyId
                            orderby p.name
                            select new PositionListItemModel
                            {
                                id = p.id,
                                name = p.name,
                                isActive = Convert.ToBoolean(p.is_active),
                                updated = p.date_updated
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
                                id = cvs.id,
                                opinion = cand.opinion,
                            };

                return query.FirstOrDefault();
            }
        }

    }
}
