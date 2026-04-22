using OpenAiLibrary.EmbeddingQdrant;
using OpenAiLibrary.Models;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAiLibrary.Searcher
{
    internal class SearcherService
    {
        private readonly QdrantClient _qdrant;
        private readonly Embedder _embedder;

        public SearcherService(Embedder embedder,
                                 string host = "localhost",
                                 int port = 6334)
        {
            _qdrant = new QdrantClient(host, port);
            _embedder = embedder;
        }

        public async Task<List<SearchResultModel>> SearchAsync(
            string query,
            SearchFilterModel? filter = null,
            int limit = 10)
        {
            Console.WriteLine($"[→] Query: \"{query}\"");

            // Embed the Hebrew query using the same model as ingestion
            var queryVector = await _embedder.EmbedAsync(query);

            // Build Qdrant filter from structured filter object
            var qdrantFilter = BuildFilter(filter);

            // Search
            var hits = await _qdrant.SearchAsync(
     collectionName: QdrantConfig.CollectionName,
     vector: queryVector,
     filter: qdrantFilter,
     limit: (ulong)limit
 );

            var results = hits.Select(MapToResult).ToList();

            Console.WriteLine($"[✓] Found {results.Count} candidates.");
            return results;
        }

        // ── Filter builder ────────────────────────────────────────────────────────

        private static Filter? BuildFilter(SearchFilterModel? f)
        {
            if (f is null) return null;

            var conditions = new List<Condition>();

            // Seniority exact match
            if (!string.IsNullOrWhiteSpace(f.Seniority))
            {
                conditions.Add(new Condition
                {
                    Field = new FieldCondition
                    {
                        Key = "seniority",
                        Match = new Match { Keyword = f.Seniority }
                    }
                });
            }

            // Location exact match
            if (!string.IsNullOrWhiteSpace(f.Location))
            {
                conditions.Add(new Condition
                {
                    Field = new FieldCondition
                    {
                        Key = "location",
                        Match = new Match { Keyword = f.Location }
                    }
                });
            }

            // Required skills — all must match (AND)
            if (f.RequiredSkills?.Count > 0)
            {
                foreach (var skill in f.RequiredSkills)
                {
                    conditions.Add(new Condition
                    {
                        Field = new FieldCondition
                        {
                            Key = "skills",
                            Match = new Match { Keyword = skill }
                        }
                    });
                }
            }

            // Min years experience
            if (f.MinYearsExperience.HasValue)
            {
                conditions.Add(new Condition
                {
                    Field = new FieldCondition
                    {
                        Key = "years_experience",
                        Range = new Qdrant.Client.Grpc.Range
                        {
                            Gte = f.MinYearsExperience.Value
                        }
                    }
                });
            }

            // Max years experience
            if (f.MaxYearsExperience.HasValue)
            {
                conditions.Add(new Condition
                {
                    Field = new FieldCondition
                    {
                        Key = "years_experience",
                        Range = new Qdrant.Client.Grpc.Range
                        {
                            Lte = f.MaxYearsExperience.Value
                        }
                    }
                });
            }

            if (conditions.Count == 0) return null;

            var filter = new Filter();
            filter.Must.AddRange(conditions);
            return filter;
        }

        // ── Map Qdrant hit → CandidateSearchResult ────────────────────────────────

        private static SearchResultModel MapToResult(ScoredPoint hit)
        {
            var p = hit.Payload;

            return new SearchResultModel
            {
                Id = hit.Id.Num,
                Score = hit.Score,
                FullName = GetString(p, "full_name"),
                CurrentTitle = GetString(p, "current_title"),
                Seniority = GetString(p, "seniority"),
                Location = GetString(p, "location"),
                Email = GetString(p, "email"),
                Phone = GetString(p, "phone"),
                SummaryHebrew = GetString(p, "summary_hebrew"),
                YearsExperience = (int)GetLong(p, "years_experience"),
                Skills = GetList(p, "skills"),
            };
        }

        // ── Payload helpers ───────────────────────────────────────────────────────

        private static string GetString(
            IDictionary<string, Qdrant.Client.Grpc.Value> payload, string key)
            => payload.TryGetValue(key, out var v) ? v.StringValue ?? "" : "";

        private static long GetLong(
            IDictionary<string, Qdrant.Client.Grpc.Value> payload, string key)
            => payload.TryGetValue(key, out var v) ? v.IntegerValue : 0;

        private static List<string> GetList(
            IDictionary<string, Qdrant.Client.Grpc.Value> payload, string key)
            => payload.TryGetValue(key, out var v) && v.ListValue is not null
                   ? v.ListValue.Values.Select(x => x.StringValue ?? "").ToList()
                   : [];
    }
}