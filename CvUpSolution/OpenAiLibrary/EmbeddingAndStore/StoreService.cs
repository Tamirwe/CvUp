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
                    if (string.IsNullOrEmpty(cv.CurrentJobTitleHe) && string.IsNullOrEmpty(cv.SummaryHe))
                    {
                        continue;
                    }

                    // var textToNormalize = string.Join(" ", cv.CurrentTitleHe, cv.SummaryHe);

                    //string normalized = normalizer.Normalize(textToNormalize);
                    // cv.NormelizedHe = normalized;

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

            var skillsList = new Qdrant.Client.Grpc.ListValue();
            var professionWordsEnList = new Qdrant.Client.Grpc.ListValue();
            var professionWordsHeList = new Qdrant.Client.Grpc.ListValue();
            var professionSkillsEnList = new Qdrant.Client.Grpc.ListValue();
            var professionSkillsHeList = new Qdrant.Client.Grpc.ListValue();

            var skillValues = cv.Skills != null ? cv.Skills
               .Select(s => new Qdrant.Client.Grpc.Value { StringValue = s.Trim() })
               .ToList() : [];
            skillsList.Values.AddRange(skillValues);

            var professionWordsEnValues = cv.ProfessionWordsEn != null ? cv.ProfessionWordsEn
               .Select(s => new Qdrant.Client.Grpc.Value { StringValue = s.Trim() })
               .ToList() : [];
            professionWordsEnList.Values.AddRange(skillValues);

            var professionWordsHeValues = cv.ProfessionWordsHe != null ? cv.ProfessionWordsHe
               .Select(s => new Qdrant.Client.Grpc.Value { StringValue = s.Trim() })
               .ToList() : [];
            professionWordsHeList.Values.AddRange(professionWordsHeValues);

            var professionSkillsEnValues = cv.ProfessionSkillsEn != null ? cv.ProfessionSkillsEn
               .Select(s => new Qdrant.Client.Grpc.Value { StringValue = s.Trim() })
               .ToList() : [];
            professionSkillsEnList.Values.AddRange(professionSkillsEnValues);

            var professionSkillsHeValues = cv.ProfessionSkillsHe != null ? cv.ProfessionSkillsHe
               .Select(s => new Qdrant.Client.Grpc.Value { StringValue = s.Trim() })
               .ToList() : [];
            professionSkillsHeList.Values.AddRange(professionSkillsHeValues);


            return new Dictionary<string, Qdrant.Client.Grpc.Value>
            {
                ["candidate_id"] = new() { StringValue = cv.CandidateId.ToString() },
                ["cv_id"] = new() { StringValue = cv.CvId.ToString() },
                ["name"] = new() { StringValue = (cv.Name ?? "").Trim() },
                ["email"] = new() { StringValue = (cv.Email ?? "").Trim() },
                ["phone"] = new() { StringValue = (cv.Phone ?? "").Trim() },
                ["location"] = new() { StringValue = (cv.Location ?? "").Trim() },
                ["region"] = new() { StringValue = (cv.Region ?? "").Trim() },
                ["area"] = new() { StringValue = (cv.Area ?? "").Trim() },
                ["languages"] = new() { StringValue = (cv.Languages ?? "").Trim() },
                ["current_job_title_en"] = new() { StringValue = (cv.CurrentJobTitleEn ?? "").Trim() },
                ["current_job_title_he"] = new() { StringValue = (cv.CurrentJobTitleHe ?? "").Trim() },
                ["profession_words_en"] = new() { ListValue = professionWordsEnList },
                ["profession_words_he"] = new() { ListValue = professionWordsHeList },
                ["profession_skills_en"] = new() { ListValue = professionSkillsEnList },
                ["profession_skills_he"] = new() { ListValue = professionSkillsHeList },
                ["seniority"] = new() { StringValue = (cv.Seniority ?? "").Trim() },
                ["education"] = new() { StringValue = (cv.Education ?? "").Trim() },
                ["companies"] = new() { StringValue = (cv.Companies ?? "").Trim() },
                ["skills"] = new() { ListValue = skillsList },
                ["military_service"] = new() { StringValue = (cv.MilitaryService ?? "").Trim() },
                ["summary_en"] = new() { StringValue = (cv.SummaryEn ?? "").Trim() },
                ["summary_he"] = new() { StringValue = (cv.SummaryHe ?? "").Trim() },
                ["years_experience"] = new() { IntegerValue = cv.YearsExperience ?? 0 },
                ["indexed_at"] = new() { StringValue = DateTime.UtcNow.ToString("o") },
            };
        }
    }
}
