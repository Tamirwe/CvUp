using Qdrant.Client;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloaudeAiLibrary
{
    public class CvSearchService
    {
        private readonly QdrantClient _qdrant;
        private readonly OpenAIEmbedder _embedder;
        private const string Collection = "cvs";

        public CvSearchService(QdrantClient qdrant, OpenAIEmbedder embedder)
        {
            _qdrant = qdrant;
            _embedder = embedder;
        }

        public async Task<List<ScoredPoint>> SearchAsync(string query, int limit = 10)
        {
            var embedding = await _embedder.EmbedAsync(query);

            return (await _qdrant.QueryAsync(
                collectionName: Collection,
                query: new Query { Nearest = new VectorInput(embedding) },
                limit: (ulong)limit
                //withPayload: true
            )).ToList();
        }
    }
}
