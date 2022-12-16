using Database.models;
using DataModelsLibrary.Models;

namespace CvsPositionsLibrary
{
    public interface ICvsPositionsServise
    {
        public void AddNewCvToDb(ImportCvModel importCv);
        public void AddNewCvToIndex(ImportCvModel item);
        public int GetAddCandidateId(int companyId, string email, string phone);
        public int GetUniqueCvId();
        public void IndexCompanyCvs(int companyId);
        public List<CvListItemModel> GetCvsList(int companyId);
        public position AddUpdatePosition(position data);
        public hr_company AddUpdateHrCompany(IdNameModel data);
        public department AddDepartment(IdNameModel data);
        public department? UpdateDepartment(IdNameModel data);
        public List<IdNameModel> GetCompanyDepartments(int companyId);
        public department? DeleteCompanyDepartment(int companyId, int id);
    }
}