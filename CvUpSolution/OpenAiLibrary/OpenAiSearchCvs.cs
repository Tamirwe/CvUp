using Microsoft.Extensions.Configuration;
using OpenAI.Embeddings;

namespace OpenAiLibrary
{
    public class OpenAiSearchCvs : IOpenAiSearchCvs
    {
        private readonly EmbeddingClient _client;

        public OpenAiSearchCvs(IConfiguration configuration)
        {
            var apiKey = configuration["API_KEY"];
            _client = new EmbeddingClient(OpenAiEmbedding.EmbeddingModel, apiKey);
        }

        public async Task<float[]> EmbedSearchQuery(string query)
        {
            var result = await _client.GenerateEmbeddingAsync(query);
            return result.Value.ToFloats().ToArray();
        }
    }
}
