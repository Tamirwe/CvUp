using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;

namespace OpenAiLibrary.EmbeddingAndStore
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
        private IOpenAiEmbedderService _openAiEmbedderService;


        public EmbedderStoreService(ICandsCvsQueries candsCvsQueries, IOpenAiEmbedderService openAiEmbedderService)
        {
            _candsCvsQueries = candsCvsQueries;
            _openAiEmbedderService = openAiEmbedderService;
        }

        public async Task EmbedAnalyzedCvs()
        {
            List<EmbedCvDataModel> allCandidatesLastCvList = await _candsCvsQueries.GetAnalyzedCvsForEmbeeding();


            var store = new StoreService(_openAiEmbedderService);

            await store.EnsureCollectionAsync();
            await store.UpsertBatchAsync(allCandidatesLastCvList);
            await _candsCvsQueries.UpdateIsEmbeddedBatch(allCandidatesLastCvList);

            Console.WriteLine($"[✓] Batch upserted candidates.");
        }

    }
}
