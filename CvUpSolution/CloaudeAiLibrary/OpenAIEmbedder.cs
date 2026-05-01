using Microsoft.Extensions.Configuration;
using OpenAI.Embeddings;


namespace CloaudeAiLibrary
{

    public class OpenAIEmbedder
    {
        private readonly EmbeddingClient _client;

        // text-embedding-3-small is cheap, fast, and supports Hebrew well
        public OpenAIEmbedder(IConfiguration config)
        {
            _client = new EmbeddingClient(
                model: "text-embedding-3-small",
                apiKey: config["OpenAI:ApiKey"]
            );
        }

        public async Task<float[]> EmbedAsync(string text)
        {
            var result = await _client.GenerateEmbeddingAsync(text);
            return result.Value.ToFloats().ToArray();
        }

        public async Task<IEnumerable<float[]>> EmbedBatchAsync(IEnumerable<string> texts)
        {
            var result = await _client.GenerateEmbeddingsAsync(texts.ToList());
            return result.Value.Select(e => e.ToFloats().ToArray());
        }
    }
}
