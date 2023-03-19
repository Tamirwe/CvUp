using Database.models;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface ICandsPositionsQueries
    {
        public Task<int> AddCv(ImportCvModel importCv);
        public Task<int> AddCandidate(candidate importCv);
        public Task UpdateCandidate(candidate cand);
        public Task<candidate?> GetCandidateByEmail(string email);
        public Task<List<CvPropsToIndexModel>> GetCompanyCvsToIndex(int companyId);
        public Task<List<CandModel?>> GetCandsList(int companyId, string encriptKey, int page, int take, List<int>? candsIds);
        public Task<position> AddPosition(PositionClientModel data, int companyId, int userId);
        public Task<position?> UpdatePosition(PositionClientModel data, int companyId, int userId);
        public Task<List<PositionModel>> GetPositionsList(int companyId);
        public Task DeletePosition(int companyId, int id);
        public Task<PositionClientModel> GetPosition(int companyId, int positionId);
        public Task<List<ParserRulesModel>> GetParsersRules(int companyId);
        public Task<List<int>> GetCompaniesIds();
        public Task<List<string?>> GetCompanyCvsIds(int companyId);
        public Task<CvModel?> GetCv(int cvId, int companyId);
        public Task UpdateCvKeyId(ImportCvModel importCv);
        public Task SaveCvReview(CvReviewModel cvReview);
        public Task<List<cv>> CheckIsCvDuplicate(int companyId, int candidateId, int cvAsciiSum);
        public Task UpdateCandidateLastCv(ImportCvModel importCv);
        public Task UpdateSameCv(ImportCvModel importCv);
        public Task<candidate?> GetCandidateByPhone(string phone);
        public Task<List<CandCvModel>> GetCandCvsList(int companyId, int candidateId, string encriptKey);
        public Task<List<CandModel>> GetPosCandsList(int companyId, int positionId, string encriptKey);
        public Task<CandPosModel> AttachPosCandCv(AttachePosCandCvModel posCv);
        public Task<CandPosModel> DetachPosCand(AttachePosCandCvModel posCv);
        public Task<List<company_cvs_email>> GetCompaniesEmails();
        Task<List<CandModel?>> GetFolderCandsList(int companyId, int folderId);
    }
}