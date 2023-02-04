using Database.models;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface ICandsPositionsQueries
    {
        public int AddCv(ImportCvModel importCv);
        public int AddCandidate(candidate importCv);
        public void UpdateCandidate( candidate cand);
        public candidate? GetCandidateByEmail(string email);
        public List<CvPropsToIndexModel> GetCompanyCvsToIndex(int companyId);
        public List<CvListItemModel> GetCvsList(int companyId, string encriptKey, int page, int take, int positionId, string? searchKeyWords);
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
        public List<PositionModel> GetPositionsList(int companyId);
        public void DeletePosition(int companyId, int id);
        public PositionClientModel GetPosition(int companyId, int positionId);
        public List<ParserRulesModel> GetParsersRules(int companyId);
        public List<int> GetCompaniesIds();
        public List<string?> GetCompanyCvsIds(int companyId);
        public CvModel? GetCv(int cvId, int companyId);
        public void UpdateCvKeyId(ImportCvModel importCv);
        public void SaveCvReview(CvReviewModel cvReview);
        public List<cv> CheckIsCvDuplicate(int companyId, int candidateId,  int cvAsciiSum);
        public void UpdateCandidateLastCv(ImportCvModel importCv);
        public void UpdateSameCv(ImportCvModel importCv);
        public candidate? GetCandidateByPhone(string phone);
        public List<CvListItemModel> GetCandCvsList(int companyId, int candidateId, string encriptKey);
        public List<CvListItemModel> GetPosCandList(int companyId, int positionId, string encriptKey);
        public CandPosModel AttachPosCandCv(AttachePosCandCvModel posCv);
        public CandPosModel DetachPosCand(AttachePosCandCvModel posCv);
        public List<company_cvs_email> GetCompaniesEmails();

    }
}