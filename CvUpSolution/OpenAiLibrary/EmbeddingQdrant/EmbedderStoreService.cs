using OpenAI.Embeddings;
using dotenv.net;

namespace OpenAiLibrary.EmbeddingQdrant
{
    // ── Constants ─────────────────────────────────────────────────────────────────

    public static class QdrantConfig
    {
        public const string CollectionName = "candidates";
        public const uint VectorSize = 1536;           // text-embedding-3-small
        public const string EmbeddingModel = "text-embedding-3-small";
    }

    public class EmbedderStoreService
    {

        public async Task EmbedAnalyzedCvs()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                            ?? throw new Exception("OPENAI_API_KEY not set.");



         



            var embedder = new Embedder(openAiKey);
            var store = new StoreQdrant(embedder);

            await store.EnsureCollectionAsync();
            //await store.UpsertBatchAsync(results);

        }

    }
}
