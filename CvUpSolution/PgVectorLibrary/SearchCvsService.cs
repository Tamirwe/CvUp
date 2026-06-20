using AnalyzeEmbedOpenAiLibrary;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;

namespace PgVectorLibrary
{
    public class SearchCvsService : ISearchCvsService
    {
        private readonly ISearchCvsOpenAi _searchCvsOpenAi;
        private readonly IAiQueries _aiQueries;

        public SearchCvsService(ISearchCvsOpenAi searchCvsOpenAi, IAiQueries aiQueries)
        {
            _searchCvsOpenAi = searchCvsOpenAi;
            _aiQueries = aiQueries;
        }

        public async Task<List<AiCandidateSearchModel>> SearchCvs(string query, int limit = 20 )
        {
            float[] queryVector = await _searchCvsOpenAi.EmbedSearchQuery(query);
            return await _aiQueries.SearchCvsByEmbedding(queryVector,  limit);
        }
    }
}
