using Microsoft.Extensions.Configuration;
using OpenAI.Embeddings;

namespace CvAnalyzeEmbedOpenAiLibrary
{
    public class SearchCvsOpenAi : ISearchCvsOpenAi
    {
        private readonly EmbeddingClient _client;

        public SearchCvsOpenAi(IConfiguration configuration)
        {
            var apiKey = configuration["API_KEY"];
            _client = new EmbeddingClient(EmbedCvOpenAi._embeddingModel, apiKey);
        }

        public async Task<float[]> EmbedSearchQuery(string query)
        {
            var result = await _client.GenerateEmbeddingAsync(query);
            return result.Value.ToFloats().ToArray();
        }
    }
}
