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
        public department AddDepartment(IdNameModel data,int companyId);
        public department? UpdateDepartment(IdNameModel data, int companyId);
        public List<IdNameModel> GetDepartmentsList(int companyId);
        public void DeleteDepartment(int companyId, int id);
        public hr_company AddHrCompany(IdNameModel data, int companyId);
        public hr_company? UpdateHrCompany(IdNameModel data, int companyId);
        public List<IdNameModel> GetHrCompaniesList(int companyId);
        public void DeleteHrCompany(int companyId, int id);
        public position? AddPosition(PositionClientModel data, int companyId);
        public position? UpdatePosition(PositionClientModel data, int companyId);
        public List<PositionListItemModel> GetPositionsList(int companyId);
        public void DeletePosition(int companyId, int id);
    }
}