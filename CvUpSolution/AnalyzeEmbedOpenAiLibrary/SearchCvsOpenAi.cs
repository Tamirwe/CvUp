using Microsoft.Extensions.Configuration;
using OpenAI.Embeddings;

namespace AnalyzeEmbedOpenAiLibrary
{
    public class SearchCvsOpenAi : ISearchCvsOpenAi
    {
        private readonly EmbeddingClient _client;

        public SearchCvsOpenAi(IConfiguration configuration)
        {
            var apiKey = configuration["API_KEY"];
            _client = new EmbeddingClient(EmbeddingOpenAi.EmbeddingModel, apiKey);
        }

        public async Task<float[]> EmbedSearchQuery(string query)
        {
            var result = await _client.GenerateEmbeddingAsync(query);
            return result.Value.ToFloats().ToArray();
        }
    }
}
