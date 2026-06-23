using DataModelsLibrary.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Spans;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Extensions.Configuration;

namespace LuceneLibrary
{
    public class LuceneSearchService : ILuceneSearchService, IDisposable
    {
        private readonly string _indexFolder;
        private readonly Analyzer _analyzer;

        FSDirectory? mIndexDirectory;
        IndexReader? mIndexReader;

        public LuceneSearchService(IConfiguration configuration, int companyId = 154)
        {
            var root = configuration["APP_LOCAL_ROOT_FOLDER"];
            _indexFolder = $"{root}\\_{companyId}\\luceneIndex";
            _analyzer = new WhitespaceAnalyzer(LuceneVersion.LUCENE_48);
        }

        public async Task<List<SearchEntry>> SearchCandidatesByPosition(AnalyzedPositionModel analyzed, int maxResults = 500)
        {
            var keywords = analyzed.LuceneKeywords.En
                .Concat(analyzed.LuceneKeywords.He)
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .Distinct()
                .ToList();

            if (keywords.Count == 0)
                return [];

            using var indexDirectory = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            using var indexReader = DirectoryReader.Open(indexDirectory);
            var indexSearcher = new IndexSearcher(indexReader);

            var tokens = keywords.Select(k => k.Trim().ToLowerInvariant()).ToArray();

            var cvQuery = new BooleanQuery { MinimumNumberShouldMatch = 1 };
            foreach (var token in tokens)
            {
                cvQuery.Add(new FuzzyQuery(new Term("CV", token), 1), Occur.SHOULD);
                cvQuery.Add(new WildcardQuery(new Term("CV", token + "*")), Occur.SHOULD);
                cvQuery.Add(new FuzzyQuery(new Term("Review", token), 1), Occur.SHOULD);
            }

            var topDocs = await Task.Run(() => indexSearcher.Search(cvQuery, null, maxResults));
            var maxScore = topDocs.MaxScore;

            return topDocs.ScoreDocs
                .Select(hit =>
                {
                    var doc = indexSearcher.Doc(hit.Doc);
                    var normalised = maxScore > 0 ? hit.Score / maxScore : 1f;
                    return new SearchEntry
                    {
                        Id        = Convert.ToInt32(doc.Get("Id")),
                        UpdatedTs = Convert.ToInt64(doc.Get("Updated")),
                        CV        = doc.Get("CV"),
                        Score     = (int)Math.Round(normalised * 100),
                    };
                })
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.UpdatedTs)
                .ToList();
        }

        public async Task<List<SearchEntry>> Search(int companyId, searchCandCvModel searchVals)
        {
            if (!searchVals.exact)
                return await FuzzySearch(companyId, searchVals);

            var results = await ExactSearch(companyId, searchVals);

            if (results.Count == 0)
                results = await FuzzySearch(companyId, searchVals);

            return results;
        }

        private async Task<List<SearchEntry>> FuzzySearch(int companyId, searchCandCvModel searchVals, int maxEdits = 2)
        {
            using var indexDirectory = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            using var indexReader = DirectoryReader.Open(indexDirectory);
            var indexSearcher = new IndexSearcher(indexReader);

            var tokens = searchVals.value.Trim().ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToArray();

            // Pass 1: name-only query
            Query BuildNameQuery(string token)
            {
                var q = new BooleanQuery();
                q.Add(new FuzzyQuery(new Term("Name", token), maxEdits), Occur.SHOULD);
                q.Add(new WildcardQuery(new Term("Name", token + "*")), Occur.SHOULD);
                return q;
            }

            // Pass 2: CV + Review query
            Query BuildCvQuery(string token)
            {
                var q = new BooleanQuery();
                q.Add(new FuzzyQuery(new Term("CV", token), maxEdits), Occur.SHOULD);
                q.Add(new WildcardQuery(new Term("CV", token + "*")), Occur.SHOULD);
                q.Add(new FuzzyQuery(new Term("Review", token), maxEdits), Occur.SHOULD);
                q.Add(new WildcardQuery(new Term("Review", token + "*")), Occur.SHOULD);
                return q;
            }

            Query BuildMultiToken(Func<string, Query> buildToken)
            {
                if (tokens.Length == 1) return buildToken(tokens[0]);
                var bq = new BooleanQuery();
                foreach (var t in tokens)
                    bq.Add(buildToken(t), Occur.MUST);
                return bq;
            }

            var nameTopDocs = await Task.Run(() => indexSearcher.Search(BuildMultiToken(BuildNameQuery), null, 100));
            var cvTopDocs   = await Task.Run(() => indexSearcher.Search(BuildMultiToken(BuildCvQuery),   null, 100));

            var nameMaxScore = nameTopDocs.MaxScore;
            var cvMaxScore   = cvTopDocs.MaxScore;

            // Name matches → score 51–90, CV-only matches → score 1–50
            var results = new Dictionary<int, SearchEntry>();

            foreach (var hit in nameTopDocs.ScoreDocs)
            {
                var doc = indexSearcher.Doc(hit.Doc);
                var id  = Convert.ToInt32(doc.Get("Id"));
                var normalised = nameMaxScore > 0 ? hit.Score / nameMaxScore : 1f;
                results[id] = new SearchEntry
                {
                    Id        = id,
                    UpdatedTs = Convert.ToInt64(doc.Get("Updated")),
                    CV        = doc.Get("CV"),
                    Score     = 51 + (int)Math.Round(normalised * 39) // 51–90
                };
            }

            foreach (var hit in cvTopDocs.ScoreDocs)
            {
                var doc = indexSearcher.Doc(hit.Doc);
                var id  = Convert.ToInt32(doc.Get("Id"));
                if (results.ContainsKey(id)) continue; // already ranked by name

                var normalised = cvMaxScore > 0 ? hit.Score / cvMaxScore : 1f;
                results[id] = new SearchEntry
                {
                    Id        = id,
                    UpdatedTs = Convert.ToInt64(doc.Get("Updated")),
                    CV        = doc.Get("CV"),
                    Score     = 1 + (int)Math.Round(normalised * 49) // 1–50
                };
            }

            return results.Values
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.UpdatedTs)
                .ToList();
        }

        private async Task<List<SearchEntry>> ExactSearch(int companyId, searchCandCvModel searchVals)
        {
            mIndexDirectory = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            mIndexReader    = DirectoryReader.Open(mIndexDirectory);
            var indexSearcher = new IndexSearcher(mIndexReader);

            var tokens = searchVals.value.Trim().ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToArray();

            // Pass 1: name-only exact (fuzzy-1 to tolerate small inaccuracies)
            Query nameQuery;
            if (tokens.Length == 1)
            {
                nameQuery = new FuzzyQuery(new Term("Name", tokens[0]), 1);
            }
            else
            {
                var nq = new BooleanQuery();
                foreach (var t in tokens)
                    nq.Add(new FuzzyQuery(new Term("Name", t), 1), Occur.MUST);
                nameQuery = nq;
            }

            // Pass 2: CV + Review exact
            Query cvQuery;
            if (tokens.Length == 1)
            {
                var bq = new BooleanQuery { MinimumNumberShouldMatch = 1 };
                bq.Add(new TermQuery(new Term("CV",     tokens[0])), Occur.SHOULD);
                bq.Add(new TermQuery(new Term("Review", tokens[0])), Occur.SHOULD);
                cvQuery = bq;
            }
            else
            {
                var spanTerms = tokens
                    .Select(t => new SpanTermQuery(new Term("CV", t)))
                    .ToArray<SpanQuery>();

                // words adjacent → highest score
                var cvAdjacent = new SpanNearQuery(spanTerms, slop: 0, inOrder: false) { Boost = 3.0f };
                // words within 5 tokens → medium score
                var cvNear     = new SpanNearQuery(spanTerms, slop: 5, inOrder: false) { Boost = 2.0f };
                // all words present anywhere → base score
                var cvAny = new BooleanQuery { Boost = 1.0f };
                foreach (var t in tokens)
                    cvAny.Add(new TermQuery(new Term("CV", t)), Occur.MUST);

                var reviewQ = new BooleanQuery();
                foreach (var t in tokens)
                    reviewQ.Add(new TermQuery(new Term("Review", t)), Occur.MUST);

                var combined = new BooleanQuery { MinimumNumberShouldMatch = 1 };
                combined.Add(cvAdjacent, Occur.SHOULD);
                combined.Add(cvNear,     Occur.SHOULD);
                combined.Add(cvAny,      Occur.SHOULD);
                combined.Add(reviewQ,    Occur.SHOULD);
                cvQuery = combined;
            }

            var nameTopDocs = await Task.Run(() => indexSearcher.Search(nameQuery, null, 100));
            var cvTopDocs   = await Task.Run(() => indexSearcher.Search(cvQuery,   null, 100));

            var results = new Dictionary<int, SearchEntry>();

            foreach (var hit in nameTopDocs.ScoreDocs)
            {
                var doc = indexSearcher.Doc(hit.Doc);
                var id  = Convert.ToInt32(doc.Get("Id"));
                results[id] = new SearchEntry
                {
                    Id        = id,
                    UpdatedTs = Convert.ToInt64(doc.Get("Updated")),
                    CV        = doc.Get("CV"),
                    Score     = 90
                };
            }

            foreach (var hit in cvTopDocs.ScoreDocs)
            {
                var doc = indexSearcher.Doc(hit.Doc);
                var id  = Convert.ToInt32(doc.Get("Id"));
                if (results.ContainsKey(id)) continue;

                results[id] = new SearchEntry
                {
                    Id        = id,
                    UpdatedTs = Convert.ToInt64(doc.Get("Updated")),
                    CV        = doc.Get("CV"),
                    Score     = 50
                };
            }

            return results.Values
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.UpdatedTs)
                .ToList();
        }

        public void Dispose()
        {
            mIndexReader?.Dispose();
            mIndexDirectory?.Dispose();
        }
    }
}
