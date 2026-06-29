using Microsoft.Extensions.Configuration;
using OpenAI.Embeddings;

namespace OpenAiLibrary
{
    public class OpenAiEmbedding : IOpenAiEmbedding
    {
        private readonly EmbeddingClient _client;
        public const string EmbeddingModel = "text-embedding-3-small";

        public OpenAiEmbedding(IConfiguration configuration)
        {
            var apiKey = configuration["API_KEY"];
            _client = new EmbeddingClient(EmbeddingModel, apiKey);
        }

        public async Task<float[]?> EmbedText(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var result = await _client.GenerateEmbeddingAsync(text);
            return result.Value.ToFloats().ToArray();
        }
    }
}
