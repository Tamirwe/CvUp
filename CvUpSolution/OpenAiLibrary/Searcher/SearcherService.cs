using DataModelsLibrary.Models;
using OpenAiLibrary.EmbeddingAndStore;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace OpenAiLibrary.Searcher
{
    public class SearcherService : ISearcherService
    {
        private readonly QdrantClient _qdrant;
        private readonly IOpenAiEmbedderService _embedder;

        public SearcherService(IOpenAiEmbedderService embedder,string host = "localhost",int port = 6334)
        {
            _qdrant = new QdrantClient(host, port);
            _embedder = embedder;
        }


        public async Task DemoSearch()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //var openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;

         //   // ── Example 1: semantic only — no filters ─────────────────────────────
         //   var results11 = await SearchAsync(
         //    query: "מנתח מערכות בכיר",
         //    limit: 5
         //);
         //   ResultPrinter.Print(results11);

            var results12 = await SearchAsync(
          //query: "אבטחת מידע התמחות ב checkpoint or fortinet FW",
          //query: "אנשי רשת ואבטחת מידע סייבר",
          //query: "אנשי אבטחת מידע סייבר",
          query: "חקלאות הידרופוניה",

          //query: "רואה חשבון רוא\"ח",
          //query: "מנהל כספים מנוסה בחברות טכנולוגיות ציבוריות",

          limit: 5
      );
            ResultPrinter.Print(results12);

            //var results1 = await SearchAsync(
            //    query: "מפתח בכיר עם ניסיון ב-javascript ו-TypeScript",
            //    limit: 5
            //);
            //ResultPrinter.Print(results1);

            //// ── Example 2: semantic + seniority + location filter ─────────────────
            //var results2 = await SearchAsync(
            //    query: "מפתח Full Stack עם ניסיון ב-AWS",
            //    filter: new SearchFilterModel
            //    {
            //        Seniority = "Senior",
            //        Location = "תל אביב"
            //    },
            //    limit: 5
            //);
            //ResultPrinter.Print(results2);

            //// ── Example 3: must have specific skills + min experience ─────────────
            //var results3 = await SearchAsync(
            //    query: "מפתח backend עם ניסיון ב-microservices",
            //    filter: new SearchFilterModel
            //    {
            //        RequiredSkills = ["Docker", "Kubernetes"],
            //        MinYearsExperience = 5
            //    },
            //    limit: 10
            //);
            //ResultPrinter.Print(results3);

            //// ── Example 4: all filters combined ───────────────────────────────────
            //var results4 = await SearchAsync(
            //    query: "מפתח C# עם ניסיון ב-Azure",
            //    filter: new SearchFilterModel
            //    {
            //        Seniority = "Senior",
            //        Location = "תל אביב",
            //        RequiredSkills = ["C#", "Docker"],
            //        MinYearsExperience = 5,
            //        MaxYearsExperience = 15
            //    },
            //    limit: 5
            //);
            //ResultPrinter.Print(results4);
        }


        public async Task<List<SearchResultModel>> SearchAsync(
            string query,
            SearchFilterModel? filter = null,
            int limit = 10)
        {
            Console.WriteLine(ResultPrinter.reverseStr($"\n\n חיפוש:\"{query}\" \n\n"));

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
     //scoreThreshold: 0.55f    // drop candidates below this similarity score
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
                Name = GetString(p, "name"),
                CandidateId = GetString(p, "candidate_id"),
                CurrentTitle = GetString(p, "current_title"),
                Location = GetString(p, "location"),
                Email = GetString(p, "email"),
                Phone = GetString(p, "phone"),
                Summary = GetString(p, "summary"),
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

    public static class ResultPrinter
    {
        public static void Print(List<SearchResultModel> results)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (results.Count == 0)
            {
                Console.WriteLine(reverseStr("לא נמצאו מועמדים מתאימים."));
                return;
            }

            for (int i = 0; i < results.Count; i++)
            {
                var r = results[i];
                Console.WriteLine(reverseStr($"── {i + 1}. {r.Name}  ניקוד: {r.Score:F2} \n\n"));
                Console.WriteLine(reverseStr($"   תפקיד    : {r.CurrentTitle}"));
                Console.WriteLine(reverseStr($"   ניסיון   : {r.YearsExperience} שנים"));
                Console.WriteLine(reverseStr($"   מיקום    : {r.Location}"));
                Console.WriteLine(reverseStr($"   כישורים  : {string.Join(", ", r.Skills)}"));
                Console.WriteLine(reverseStr($"   סיכום    : {r.Summary}"));
                Console.WriteLine(reverseStr($"   אימייל   : {r.Email}"));
                Console.WriteLine(reverseStr($"   טלפון    : {r.Phone}"));
                Console.WriteLine($"   טלפון    : {r.CandidateId}");
            }
        }

        public static string reverseStr(string str = "")
        {
            char[] charArray = str.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}