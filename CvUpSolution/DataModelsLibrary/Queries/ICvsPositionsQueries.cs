using Database.models;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface ICvsPositionsQueries
    {
        public void AddNewCvToDb(ImportCvModel importCv);
        public candidate AddNewCandidate(int companyId, string email, string phone);
        public candidate? GetCandidateByEmail(string email);
        public List<CvPropsToIndexModel> GetCompanyCvsToIndex(int companyId);
        public int GetUniqueCvId();
        public List<CvListItemModel> GetCvsList(int companyId);
        public department AddDepartment(IdNameModel data);
        public department? UpdateDepartment(IdNameModel data);
        public List<IdNameModel> GetDepartments(int companyId);
        public department? DeleteDepartment(int companyId, int id);
        public hr_company AddHrCompany(IdNameModel data);
        public hr_company? UpdateHrCompany(IdNameModel data);
        public List<IdNameModel> GetHrCompanies(int companyId);
        public hr_company? DeleteHrCompany(int companyId, int id);
    }
}