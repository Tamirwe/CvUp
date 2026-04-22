using Google.Protobuf.WellKnownTypes;
using OpenAiLibrary.Models;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAiLibrary.EmbeddingQdrant
{

    public class StoreQdrant
    {
        private readonly QdrantClient _qdrant;
        private readonly Embedder _embedder;

        public StoreQdrant(Embedder embedder, string host = "localhost", int port = 6334)
        {
            _qdrant = new QdrantClient(host, port);
            _embedder = embedder;
        }

        // ── Collection setup ──────────────────────────────────────────────────────

        public async Task EnsureCollectionAsync()
        {
            var collections = await _qdrant.ListCollectionsAsync();
            bool exists = collections.Any(c => c == QdrantConfig.CollectionName);

            if (!exists)
            {
                await _qdrant.CreateCollectionAsync(
            collectionName: QdrantConfig.CollectionName,
            vectorsConfig: new VectorParams
            {
                Size = QdrantConfig.VectorSize,
                Distance = Distance.Cosine
            }
                );

                // Payload indexes for fast filtering
                await _qdrant.CreatePayloadIndexAsync(
                    QdrantConfig.CollectionName, "seniority",
                    PayloadSchemaType.Keyword);

                await _qdrant.CreatePayloadIndexAsync(
                    QdrantConfig.CollectionName, "location",
                    PayloadSchemaType.Keyword);

                await _qdrant.CreatePayloadIndexAsync(
                    QdrantConfig.CollectionName, "years_experience",
                    PayloadSchemaType.Integer);

                await _qdrant.CreatePayloadIndexAsync(
                    QdrantConfig.CollectionName, "skills",
                    PayloadSchemaType.Keyword);

                Console.WriteLine($"[✓] Collection '{QdrantConfig.CollectionName}' created.");
            }
            else
            {
                Console.WriteLine($"[✓] Collection '{QdrantConfig.CollectionName}' already exists.");
            }
        }

        // ── Upsert single CV ──────────────────────────────────────────────────────

        public async Task UpsertAsync(Guid id, AnalyzedCvModel cv)
        {
            var embedText = Embedder.BuildEmbedText(cv);
            var vector = await _embedder.EmbedAsync(embedText);
            var payload = BuildPayload(cv);

            var point = new PointStruct
            {
                Id = new PointId { Uuid = id.ToString() },
                Vectors = vector,
                Payload = { payload }
            };

            await _qdrant.UpsertAsync(QdrantConfig.CollectionName, [point]);

            Console.WriteLine($"  [✓] Upserted: {cv.Name} ({cv.Seniority}, {cv.Location})");
        }

        // ── Batch upsert ──────────────────────────────────────────────────────────

        public async Task UpsertBatchAsync(List<AnalyzedCvModel> cvs)
        {
            var points = new List<PointStruct>();

            foreach (var cv in cvs)
            {
                var embedText = Embedder.BuildEmbedText(cv);
                var vector = await _embedder.EmbedAsync(embedText);

                points.Add(new PointStruct
                {
                    Id = new PointId { Uuid = Guid.NewGuid().ToString() },
                    Vectors = vector,
                    Payload = { BuildPayload(cv) }
                });

                Console.WriteLine($"  [embed] {cv.Name}");
            }

            await _qdrant.UpsertAsync(QdrantConfig.CollectionName, points);
            Console.WriteLine($"[✓] Batch upserted {points.Count} candidates.");
        }

        // ── Build Qdrant payload from CvAnalysisResult ────────────────────────────

        private static Dictionary<string, Qdrant.Client.Grpc.Value> BuildPayload(AnalyzedCvModel cv)
        {
            var skillValues = cv.Skills
                .Select(s => new Qdrant.Client.Grpc.Value { StringValue = s })
                .ToList();

            var skillsList = new Qdrant.Client.Grpc.ListValue();

            skillsList.Values.AddRange(skillValues);

            return new Dictionary<string, Qdrant.Client.Grpc.Value>
            {
                ["name"] = new() { StringValue = cv.Name },
                ["email"] = new() { StringValue = cv.Email },
                ["phone"] = new() { StringValue = cv.Phone },
                ["location"] = new() { StringValue = cv.Location },
                ["Region"] = new() { StringValue = cv.Region },
                ["Area"] = new() { StringValue = cv.Area },
                ["skills"] = new() { ListValue = skillsList },
                ["seniority"] = new() { StringValue = cv.Seniority.ToString() },
                ["years_experience"] = new() { IntegerValue = cv.YearsExperience ?? 0 },
                ["current_title"] = new() { StringValue = cv.CurrentTitle },
                ["languages"] = new() { StringValue = cv.Languages },
                ["summary"] = new() { StringValue = cv.Summary },
                ["cv_language"] = new() { StringValue = cv.CvLanguage.ToString() },
                ["indexed_at"] = new() { StringValue = DateTime.UtcNow.ToString("o") },
            };
        }
    }
}
