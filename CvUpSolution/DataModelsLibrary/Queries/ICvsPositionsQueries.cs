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
        public department AddNewDepartment(IdNameModel newDepartment);
        public department updateDepartment(department dep);
        public hr_company AddNewHrCompany(IdNameModel data);
        public List<IdNameModel> GetCompanyDepartments(int companyId);
        public department? DeleteCompanyDepartment(int companyId, int id);
    }
}