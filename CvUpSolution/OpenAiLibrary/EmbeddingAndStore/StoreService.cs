using DataModelsLibrary.Models;
using Google.Protobuf.WellKnownTypes;
using OpenAiLibrary.Models;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAiLibrary.EmbeddingAndStore
{

    public class StoreService : IStoreService
    {
        private readonly QdrantClient _qdrant;
        private readonly IOpenAiEmbedderService _embedder;

        public StoreService(IOpenAiEmbedderService embedder, string host = "localhost", int port = 6334)
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

        public async Task UpsertAsync(Guid id, EmbedCvDataModel cv)
        {
            var embedText = OpenAiEmbedderService.BuildEmbedText(cv);
            var vector = await _embedder.EmbedAsync(embedText);
            var payload = BuildPayload(cv);

            var point = new PointStruct
            {
                Id = new PointId { Uuid = id.ToString() },
                Vectors = vector,
                Payload = { payload }
            };

            await _qdrant.UpsertAsync(QdrantConfig.CollectionName, [point]);

            Console.WriteLine($"  [✓] Upserted: {cv.Name} ({cv.Location})");
        }

        // ── Batch upsert ──────────────────────────────────────────────────────────

        public async Task UpsertBatchAsync(List<EmbedCvDataModel> cvs)
        {
            var points = new List<PointStruct>();

            var normalizer = new HebrewTextNormalizer();

            //// Before storing in Qdrant
            //string normalized = normalizer.Normalize(AnalyzedCv.Summary);

            // Before searching in Qdrant (keeps stopwords for query context)
            //string normalizedQuery = normalizer.NormalizeQuery(queryText);

            if (cvs.Count > 0)
            {
                foreach (var cv in cvs)
                {
                    if (string.IsNullOrEmpty(cv.CurrentTitleHe) && string.IsNullOrEmpty(cv.SummaryHe))
                    {
                        continue;
                    }

                    var textToNormalize = string.Join(" ", cv.CurrentTitleHe, cv.SummaryHe);

                   string normalized = normalizer.Normalize(textToNormalize);
                    cv.NormelizedHe = normalized;

                    var embedText = OpenAiEmbedderService.BuildEmbedText(cv);
                    var vector = await _embedder.EmbedAsync(embedText);

                    points.Add(new PointStruct
                    {
                        Id = new PointId { Num = (ulong)cv.CandidateId },
                        Vectors = vector,
                        Payload = { BuildPayload(cv) }
                    });

                    Console.WriteLine($"  [embed] {cv.Name}");
                }

                await _qdrant.UpsertAsync(QdrantConfig.CollectionName, points);
            }
        }

        // ── Build Qdrant payload from CvAnalysisResult ────────────────────────────

        private static Dictionary<string, Qdrant.Client.Grpc.Value> BuildPayload(EmbedCvDataModel cv)
        {
            var skillValues = cv.Skills != null? cv.Skills
                .Select(s => new Qdrant.Client.Grpc.Value { StringValue = s })
                .ToList() : [];

            var skillsList = new Qdrant.Client.Grpc.ListValue();

            skillsList.Values.AddRange(skillValues);

            return new Dictionary<string, Qdrant.Client.Grpc.Value>
            {
                ["candidate_id"] = new() { StringValue = cv.CandidateId.ToString() },
                ["cv_id"] = new() { StringValue = cv.CvId.ToString() },
                ["key_id"] = new() { StringValue = cv.KeyId.ToString() },
                ["name"] = new() { StringValue = cv.Name ?? "" },
                ["email"] = new() { StringValue = cv.Email ?? "" },
                ["phone"] = new() { StringValue = cv.Phone ?? "" },
                ["location"] = new() { StringValue = cv.Location ?? "" },
                ["Region"] = new() { StringValue = cv.Region ?? "" },
                ["Area"] = new() { StringValue = cv.Area ?? "" },
                ["skills"] = new() { ListValue = skillsList },
                ["years_experience"] = new() { IntegerValue = cv.YearsExperience ?? 0 },
                ["current_title_en"] = new() { StringValue = cv.CurrentTitleEn ?? "" },
                ["current_title_he"] = new() { StringValue = cv.CurrentTitleHe ?? "" },
                ["languages"] = new() { StringValue = cv.Languages ?? "" },
                ["companies"] = new() { StringValue = cv.Companies ?? "" },
                ["summary_en"] = new() { StringValue = cv.SummaryEn ?? "" },
                ["summary_he"] = new() { StringValue = cv.SummaryHe ?? "" },
                ["normelized_he"] = new() { StringValue = cv.NormelizedHe ?? "" },
                ["indexed_at"] = new() { StringValue = DateTime.UtcNow.ToString("o") },
            };
        }
    }
}
