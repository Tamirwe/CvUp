using DataModelsLibrary.Models;
using LuceneLibrary;

namespace CandsPositionsLibrary
{
    public interface ICandsListsServise
    {
        Task<CandModel?> GetPositionCandidate(int companyId, int candId, int positionId);
        Task<List<CandModel?>> GetCandsList(int companyId, List<int>? candsIds);
        Task<List<CandModel?>> GetPosCandsList(int companyId, int positionId);
        Task<List<CandModel?>> GetPosTypeCandsList(int companyId, int positionTypeId);
        Task<List<CandModel?>> GetFolderCandsList(int companyId, int folderId);

        Task<List<SearchEntry>> GetLuceneCandidatesForPosition(int positionId);
        Task<AnalyzedPositionModel?> GetAnalyzedPosition(int positionId);
        Task<SearchTermsModel?> GetPositionSearchTerms(int positionId, bool isReAnalyze = false, int companyId = 154);
        Task<List<SearchEntry>> ComplexSearchCands(int companyId, SearchTermsModel searchTerms);
        Task SaveSearchTerms(SearchTermsModel searchTerms);
        Task<List<SearchTermsListItemModel>> GetSearchTermsList(int companyId);
        Task<SearchTermsModel?> GetSearchTermsById(int id);
        Task DeleteSearchTerms(int id, int companyId);
    }
}
