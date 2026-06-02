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
        private IStoreService _storeService;


        public EmbedderStoreService(ICandsCvsQueries candsCvsQueries, IOpenAiEmbedderService openAiEmbedderService, IStoreService storeService)
        {
            _candsCvsQueries = candsCvsQueries;
            _openAiEmbedderService = openAiEmbedderService;
            _storeService = storeService;
        }

        public async Task EmbedAnalyzedCvs()
        {
            List<AnalyzedCvsForEmbeedingModel> analyzedCvsForEmbeeding = await _candsCvsQueries.GetAnalyzedCvsForEmbeeding();
            //var store = new StoreService(_openAiEmbedderService);

            await _storeService.EnsureCollectionAsync();
            await _storeService.UpsertBatchAsync(analyzedCvsForEmbeeding);
            await _candsCvsQueries.UpdateIsEmbeddedBatch(analyzedCvsForEmbeeding);

            Console.WriteLine($"[✓] Batch upserted candidates.");
        }

    }
}
