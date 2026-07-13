using Database.models;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface ICandsCvsQueries
    {
        Task<List<AiCvModel>> GetDistinctCandsCvs(int companyId = 154, int candidateId = 0);
        Task<CandModel?> GetCandidate(int companyId, int candId);
        Task<int> AddCv(ImportCvModel importCv);
        Task DeleteCv(int companyId, int candidateId, int cvId);
        Task<Tuple<cv?, bool>> GetCandLastCv(int companyId, int candidateId);
        Task UpdateCandLastCv(int companyId, int candidateId, int cvId, bool isDuplicate, DateTime lastCvSent);
        Task DeleteCandidate(int companyId, int candidateId);
        Task UpdateCvDate(int cvId);
        Task UpdateCvKeyId(ImportCvModel importCv);
        Task<int> AddCandidate(candidate newCand);
        Task UpdateCandidate(candidate cand);
        Task<candidate?> GetCandidateByEmail(string email);
        Task<candidate?> GetCandidateByPhone(string phone);
        Task<List<CandCvModel>> GetCandCvsList(int companyId, int candidateId);
        Task UpdateCvsAsciiSum(int companyId);
        Task<List<DuplicateEmailCandModel>> GetDuplicateCandsByEmail();
        Task<List<candidate>> GetCandsByEmail(string candEmail);
        Task UpdateCvsCandId(int candMainId, List<int> candIds);
        Task UpdateCvsTxtCandId(int candMainId, List<int> candIds);
        Task UpdateFoldersCandsCandId(int candMainId, List<int> candIds);
        Task UpdatePositionCandidatesCandId(int candMainId, List<int> candIds);
        Task DeleteCands(List<int> candIds);
    }
}
