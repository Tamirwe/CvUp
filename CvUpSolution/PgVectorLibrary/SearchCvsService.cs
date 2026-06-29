using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using OpenAiLibrary;
using OpenAiLibrary.SearchCvs;

namespace PgVectorLibrary
{
    public class SearchCvsService : ISearchCvsService
    {
        private readonly IOpenAiSearchCvs _searchCvsOpenAi;
        private readonly IAiQueries _aiQueries;

        public SearchCvsService(IOpenAiSearchCvs searchCvsOpenAi, IAiQueries aiQueries)
        {
            _searchCvsOpenAi = searchCvsOpenAi;
            _aiQueries = aiQueries;
        }

        public async Task<List<AiCandidateSearchModel>> SearchCvs(searchCandCvModel searchVals, List<int>? candidateIds = null, int limit = 20)
        {
            float[] queryVector = await _searchCvsOpenAi.EmbedSearchQuery(searchVals.value);
            return await _aiQueries.SearchCvsByEmbedding(queryVector, candidateIds, limit);
        }

        public async Task<List<AiCandidateSearchModel>> SearchCvsByPositionFiltered(int positionId, List<int> candidateIds, int limit)
        {
            return await _aiQueries.SearchCvsByPositionFiltered(positionId, candidateIds, limit);
        }
    }
}
