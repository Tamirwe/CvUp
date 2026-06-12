using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface ICandsListsQueries
    {
        Task<CandModel?> GetPositionCandidate(int companyId, int candId, int positionId);
        Task<List<CandModel?>> GetCandsList(int companyId, List<int>? candsIds);
        Task<List<CandModel?>> GetPosCandsList(int companyId, int positionId);
        Task<List<CandModel?>> GetPosTypeCandsList(int companyId, int positionTypeId);
        Task<List<CandModel?>> GetFolderCandsList(int companyId, int folderId);
    }
}
