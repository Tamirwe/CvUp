using Database.models;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface ICvsPositionsQueries
    {
        public int AddCv(ImportCvModel importCv);
        public int AddCandidate(candidate importCv);
        public void UpdateCandidate( candidate cand);
        public candidate? GetCandidateByEmail(string email);
        public List<CvPropsToIndexModel> GetCompanyCvsToIndex(int companyId);
        public List<CvListItemModel> GetCvsList(int companyId, string encriptKey);
        public department AddDepartment(IdNameModel data, int companyId);
        public department? UpdateDepartment(IdNameModel data, int companyId);
        public List<IdNameModel> GetDepartmentsList(int companyId);
        public void DeleteDepartment(int companyId, int id);
        public hr_company AddHrCompany(IdNameModel data, int companyId);
        public hr_company? UpdateHrCompany(IdNameModel data, int companyId);
        public List<IdNameModel> GetHrCompaniesList(int companyId);
        public void DeleteHrCompany(int companyId, int id);
        public position AddPosition(PositionClientModel data, int companyId, int userId);
        public position? UpdatePosition(PositionClientModel data, int companyId, int userId);
        public List<PositionListItemModel> GetPositionsList(int companyId);
        public void DeletePosition(int companyId, int id);
        public PositionClientModel GetPosition(int companyId, int positionId);
        public List<ParserRulesModel> GetParsersRules(int companyId);
        public List<int> GetCompaniesIds();
        public List<string?> GetCompanyCvsIds(int companyId);
        public CvModel? GetCv(int cvId, int companyId);
        public void UpdateCvKeyId(ImportCvModel importCv);
        public void SaveCvReview(CvReviewModel cvReview);
    }
}