using DataModelsLibrary.Models;

namespace AiLibrary.SearchCvs
{
    public interface ISearchCvsService
    {
        Task<List<AiCandidateSearchModel>> SearchCvs(searchCandCvModel searchVals, List<int>? candidateIds = null, int limit = 20);
        Task<List<AiCandidateSearchModel>> SearchCvsByPositionFiltered(int positionId, List<int> candidateIds, int limit);
    }
}
