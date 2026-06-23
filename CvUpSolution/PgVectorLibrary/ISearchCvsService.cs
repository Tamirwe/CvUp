using DataModelsLibrary.Models;

namespace PgVectorLibrary
{
    public interface ISearchCvsService
    {
        Task<List<AiCandidateSearchModel>> SearchCvs(string query, int limit = 20);
        Task<List<AiCandidateSearchModel>> SearchCvsByPositionFiltered(int positionId, List<int> candidateIds, int limit);
    }
}