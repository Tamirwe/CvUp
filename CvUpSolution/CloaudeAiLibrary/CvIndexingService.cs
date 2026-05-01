using CloaudeAiLibrary.Models;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace CloaudeAiLibrary
{


    public class CvIndexingService
    {
        private readonly QdrantClient _qdrant;
        private readonly OpenAIEmbedder _embedder;
        private const string Collection = "cvs";
        private const int VectorSize = 1536; // text-embedding-3-small dimension

        public CvIndexingService(QdrantClient qdrant, OpenAIEmbedder embedder)
        {
            _qdrant = qdrant;
            _embedder = embedder;
        }

        public async Task EnsureCollectionExistsAsync()
        {
            var collections = await _qdrant.ListCollectionsAsync();
            if (!collections.Contains(Collection))
            {
                await _qdrant.CreateCollectionAsync(Collection,
                    new VectorParams
                    {
                        Size = VectorSize,
                        Distance = Distance.Cosine
                    });
            }
        }

        public async Task IndexCvAsync(ParsedCvModel cv)
        {
            var embedding = await _embedder.EmbedAsync(cv.EnrichedText);
            await UpsertPointAsync(cv, embedding);
        }

        public async Task IndexBatchAsync(IEnumerable<ParsedCvModel> cvs)
        {
            var cvList = cvs.ToList();

            // Single OpenAI batch call for all CVs
            var embeddings = (await _embedder
                .EmbedBatchAsync(cvList.Select(cv => cv.EnrichedText)))
                .ToList();

            var points = cvList.Zip(embeddings).Select(pair =>
            {
                var (cv, embedding) = pair;
                return BuildPoint(cv, embedding);
            }).ToList();

            await _qdrant.UpsertAsync(Collection, points);
        }

        private async Task UpsertPointAsync(ParsedCvModel cv, float[] embedding)
        {
            await _qdrant.UpsertAsync(Collection, new[] { BuildPoint(cv, embedding) });
        }

        private static PointStruct BuildPoint(ParsedCvModel cv, float[] embedding)
        {
            return new PointStruct
            {
                Id = new PointId { Uuid = cv.Id },
                Vectors = new Vectors
                {
                    Vector = new Vector { Data = { embedding } }
                },
                Payload =
            {
                ["full_name"]        = cv.FullName,
                ["email"]            = cv.Email,
                ["phone"]            = cv.Phone,
                ["city"]             = cv.City,
                ["profession"]       = cv.Profession,
                ["summary"]          = cv.Summary,
                ["job_titles"]       = string.Join(", ", cv.JobTitles),
                ["skills"]           = string.Join(", ", cv.Skills),
                ["experience_years"] = cv.ExperienceYears?.ToString() ?? "",
                ["education"]        = cv.Education,
                ["military_service"] = cv.MilitaryService,
                ["languages"]        = cv.Languages,
                ["enriched_text"]    = cv.EnrichedText
            }
            };
        }
    }
}
