using Database.models;
using DataModelsLibrary.Models;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
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

        public void AddNewCvToDb(ImportCvModel importCv)
        {
            var cv = new cv
            {
                id = importCv.cvId,
                company_id = Convert.ToInt32(importCv.companyId),
                candidate_id = importCv.candidateId,
                cv_ascii_sum = importCv.cvAsciiSum,
                email_id = importCv.emailId,
                subject = importCv.subject,
                from = importCv.from,
            };

            dbContext.cvs.Add(cv);

            var cvTxt = new cvs_txt
            {
                cv_id = importCv.cvId,
                company_id = Convert.ToInt32(importCv.companyId),
                cv_txt = importCv.cvTxt.Length > 7999 ? importCv.cvTxt.Substring(0, 7999) : importCv.cvTxt
            };

            dbContext.cvs_txts.Add(cvTxt);

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

        public List<CvPropsToIndexModel> GetCompanyCvsToIndex(int companyId)
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

        public List<CvListItemModel> GetCvsList(int companyId)
        {
            var query = from cand in dbContext.candidates
                        join cvs in dbContext.cvs on cand.id equals cvs.candidate_id
                        join cvTxt in dbContext.cvs_txts on cvs.id equals cvTxt.cv_id
                        where cand.company_id == companyId
                        select new CvListItemModel
                        {
                            cvId = cvs.id,
                            email = cand.email,
                            emailSubject = cvs.subject,
                            candidateName = cand.name,
                            phone = cand.phone,
                        };

            return query.ToList();
        }

        public department AddDepartment(IdNameModel data, int companyId)
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

        public department? UpdateDepartment(IdNameModel data, int companyId)
        {
                department dep = new department { id = data.id, name = data.name, company_id = companyId };
                var result = dbContext.departments.Update(dep);
                dbContext.SaveChanges();
                return result.Entity;
        }

        public List<IdNameModel> GetDepartments(int companyId)
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

        public void DeleteDepartment(int companyId, int id)
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

        public hr_company AddHrCompany(IdNameModel data, int companyId)
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

        public hr_company? UpdateHrCompany(IdNameModel data, int companyId)
        {
                hr_company hr = new hr_company { id = data.id, name = data.name, company_id = companyId };
                var result = dbContext.hr_companies.Update(hr);
                dbContext.SaveChanges();
                return result.Entity;
        }
        public List<IdNameModel> GetHrCompanies(int companyId)
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

        public void DeleteHrCompany(int companyId, int id)
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
}
