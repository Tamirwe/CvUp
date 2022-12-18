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
        public department AddDepartment(IdNameModel data,int companyId);
        public department? UpdateDepartment(IdNameModel data, int companyId);
        public List<IdNameModel> GetDepartments(int companyId);
        public department? DeleteDepartment(int companyId, int id);
        public hr_company AddHrCompany(IdNameModel data, int companyId);
        public hr_company? UpdateHrCompany(IdNameModel data, int companyId);
        public List<IdNameModel> GetHrCompanies(int companyId);
        public hr_company? DeleteHrCompany(int companyId, int id);

    }
}