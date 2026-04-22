using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;

namespace OpenAiLibrary.EmbeddingQdrant
{
    // ── Constants ─────────────────────────────────────────────────────────────────

    public static class QdrantConfig
    {
        public const string CollectionName = "candidates";
        public const uint VectorSize = 1536;           // text-embedding-3-small
        public const string EmbeddingModel = "text-embedding-3-small";
    }

    public class EmbedderStoreService : IEmbedderStoreService
    {
        private ICandsCvsQueries _candsCvsQueries;


        public EmbedderStoreService(ICandsCvsQueries candsCvsQueries)
        {
            _candsCvsQueries = candsCvsQueries;
        }

        public async Task EmbedAnalyzedCvs(string apiKey, int companyId = 154)
        {
            List<EmbedCvDataModel> allCandidatesLastCvList = await _candsCvsQueries.GetAnalyzedCvsForEmbeeding();

            var embedder = new Embedder(apiKey);
            var store = new StoreQdrant(embedder);

            await store.EnsureCollectionAsync();
            await store.UpsertBatchAsync(allCandidatesLastCvList);
            await _candsCvsQueries.UpdateIsEmbeddedBatch(allCandidatesLastCvList);

            Console.WriteLine($"[✓] Batch upserted candidates.");
        }

    }
}
