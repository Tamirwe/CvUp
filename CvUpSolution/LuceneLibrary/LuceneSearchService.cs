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
        private const LuceneVersion LUCENE_VERSION = LuceneVersion.LUCENE_48;

        // Must match field constants in LuceneIndexService
        private const string F_CANDIDATE_ID = "Id";
        private const string F_FULL_NAME = "Name";
        private const string F_CV_TEXT = "CV";
        private const string F_REVIEW = "Review";
        private const string F_AI_SUMMARY = "AiSummary";
        private const string F_AI_WORK = "AiWork";
        private const string F_AI_EDUCATION = "AiEducation";
        private const string F_AI_SKILLS = "AiSkills";

        // All content fields searched in CV/keyword queries
        private static readonly (string Field, float Boost)[] ContentFields =
        [
            (F_REVIEW,       3.0f),
            (F_AI_SUMMARY,   1.5f),
            (F_AI_WORK,      1.5f),
            (F_AI_SKILLS,    1.2f),
            (F_AI_EDUCATION, 1.0f),
            (F_CV_TEXT,      1.0f),
        ];

        private readonly string _indexFolder;
        private readonly Analyzer _analyzer;

        FSDirectory? mIndexDirectory;
        IndexReader? mIndexReader;

        public LuceneSearchService(IConfiguration configuration, int companyId = 154)
        {
            var root = configuration["APP_LOCAL_ROOT_FOLDER"];
            _indexFolder = $"{root}\\_{companyId}\\luceneIndex";
            _analyzer = new WhitespaceAnalyzer(LUCENE_VERSION);
        }

        // ─────────────────────────────────────────────
        // Position-based search (keywords from AI analysis)
        // ─────────────────────────────────────────────

        public async Task<List<SearchEntry>> SearchCandidatesByPosition(AnalyzedPositionModel analyzed, int maxResults = 1000)
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

            var query = new BooleanQuery { MinimumNumberShouldMatch = 1 };

            foreach (var token in tokens)
            {
                foreach (var (field, boost) in ContentFields)
                {
                    query.Add(new FuzzyQuery(new Term(field, token), 1) { Boost = boost }, Occur.SHOULD);
                    query.Add(new WildcardQuery(new Term(field, token + "*")) { Boost = boost * 0.8f }, Occur.SHOULD);
                }
            }

            var topDocs = await Task.Run(() => indexSearcher.Search(query, null, maxResults));
            var maxScore = topDocs.MaxScore;

            return topDocs.ScoreDocs
                .Select(hit =>
                {
                    var doc = indexSearcher.Doc(hit.Doc);
                    var normalised = maxScore > 0 ? hit.Score / maxScore : 1f;
                    return new SearchEntry
                    {
                        Id = Convert.ToInt32(doc.Get(F_CANDIDATE_ID)),
                        Score = (int)Math.Round(normalised * 100),
                    };
                })
                .OrderByDescending(x => x.Score)
                .ToList();
        }

        public async Task<List<SearchEntry>> Search(int companyId, searchCandCvModel searchVals)
        {
            var segments = searchVals.value
                .Split("||", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            // No || operator — normal single search
            if (segments.Count <= 1)
                return await RunSingleSearch(searchVals);

            // First segment — normal search
            var currentSearch = new searchCandCvModel { value = segments[0], exact = searchVals.exact };
            var results = await RunSingleSearch(currentSearch);

            if (results.Count == 0)
                return [];

            // Each subsequent segment — search within previous results
            foreach (var segment in segments.Skip(1))
            {
                var segmentSearch = new searchCandCvModel { value = segment, exact = searchVals.exact };
                results = await SearchWithin(results.Select(r => r.Id), segmentSearch);

                if (results.Count == 0)
                    return [];
            }

            return results;
        }

        public async Task<List<SearchEntry>> SearchForAiFilter(searchCandCvModel searchVals)
        {
            // luceneFilter terms are MUST
            var mustTerms = (searchVals.luceneFilter ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => new ComplexSearchTerm
                {
                    Value = s,
                    Occur = TermOccur.Must,
                    MatchType = s.Contains(' ') ? TermMatchType.ExactPhrase : TermMatchType.Keyword,
                })
                .ToList();

            // value terms are SHOULD
            var shouldTerms = (searchVals.value ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => new ComplexSearchTerm
                {
                    Value = s,
                    Occur = TermOccur.Should,
                    MatchType = TermMatchType.Keyword,
                })
                .ToList();

            // If no must terms — run should-only as first search with no filter
            if (mustTerms.Count == 0)
                return shouldTerms.Count > 0
                    ? await RunGroupSearch(shouldTerms, restrictToIds: null)
                    : [];

            // Must terms first, then narrow with should terms if any
            var results = await RunGroupSearch(mustTerms, restrictToIds: null);

            if (results.Count == 0 || shouldTerms.Count == 0)
                return results;

            return await RunGroupSearch(shouldTerms, results.Select(r => r.Id).ToHashSet());
        }
        // ─────────────────────────────────────────────
        // General search (exact or fuzzy)
        // ─────────────────────────────────────────────
        private async Task<List<SearchEntry>> RunSingleSearch(searchCandCvModel searchVals)
        {
            if (!searchVals.exact)
                return await FuzzySearch(searchVals);

            var results = await ExactSearch(searchVals);

            if (results.Count == 0)
                results = await FuzzySearch(searchVals);

            return results;
        }

        // ─────────────────────────────────────────────
        // Fuzzy search
        // ─────────────────────────────────────────────

        private async Task<List<SearchEntry>> FuzzySearch(searchCandCvModel searchVals, int maxEdits = 2)
        {
            using var indexDirectory = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            using var indexReader = DirectoryReader.Open(indexDirectory);
            var indexSearcher = new IndexSearcher(indexReader);

            var tokens = searchVals.value.Trim().ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToArray();

            Query BuildNameQuery(string token)
            {
                var q = new BooleanQuery();
                q.Add(new FuzzyQuery(new Term(F_FULL_NAME, token), maxEdits), Occur.SHOULD);
                q.Add(new WildcardQuery(new Term(F_FULL_NAME, token + "*")), Occur.SHOULD);
                return q;
            }

            Query BuildContentQuery(string token)
            {
                var q = new BooleanQuery();
                foreach (var (field, boost) in ContentFields)
                {
                    q.Add(new FuzzyQuery(new Term(field, token), maxEdits) { Boost = boost }, Occur.SHOULD);
                    q.Add(new WildcardQuery(new Term(field, token + "*")) { Boost = boost * 0.8f }, Occur.SHOULD);
                }
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
            var contentTopDocs = await Task.Run(() => indexSearcher.Search(BuildMultiToken(BuildContentQuery), null, 100));

            var nameMaxScore = nameTopDocs.MaxScore;
            var contentMaxScore = contentTopDocs.MaxScore;

            // Name matches → 51–90, content-only matches → 1–50
            var results = new Dictionary<int, SearchEntry>();

            foreach (var hit in nameTopDocs.ScoreDocs)
            {
                var doc = indexSearcher.Doc(hit.Doc);
                var id = Convert.ToInt32(doc.Get(F_CANDIDATE_ID));
                var normalised = nameMaxScore > 0 ? hit.Score / nameMaxScore : 1f;
                results[id] = new SearchEntry
                {
                    Id = id,
                    Score = 51 + (int)Math.Round(normalised * 39) // 51–90
                };
            }

            foreach (var hit in contentTopDocs.ScoreDocs)
            {
                var doc = indexSearcher.Doc(hit.Doc);
                var id = Convert.ToInt32(doc.Get(F_CANDIDATE_ID));
                if (results.ContainsKey(id)) continue;

                var normalised = contentMaxScore > 0 ? hit.Score / contentMaxScore : 1f;
                results[id] = new SearchEntry
                {
                    Id = id,
                    Score = 1 + (int)Math.Round(normalised * 49) // 1–50
                };
            }

            return results.Values
                .OrderByDescending(x => x.Score)
                .ToList();
        }

        // ─────────────────────────────────────────────
        // Exact search
        // ─────────────────────────────────────────────

        private async Task<List<SearchEntry>> ExactSearch(searchCandCvModel searchVals)
        {
            mIndexDirectory = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            mIndexReader = DirectoryReader.Open(mIndexDirectory);
            var indexSearcher = new IndexSearcher(mIndexReader);

            var tokens = searchVals.value.Trim().ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToArray();

            // Name: fuzzy-1 to tolerate small typos
            Query nameQuery;
            if (tokens.Length == 1)
            {
                nameQuery = new FuzzyQuery(new Term(F_FULL_NAME, tokens[0]), 1);
            }
            else
            {
                var nq = new BooleanQuery();
                foreach (var t in tokens)
                    nq.Add(new FuzzyQuery(new Term(F_FULL_NAME, t), 1), Occur.MUST);
                nameQuery = nq;
            }

            // Content: span proximity on each field + plain MUST across all fields
            Query contentQuery;
            if (tokens.Length == 1)
            {
                var bq = new BooleanQuery { MinimumNumberShouldMatch = 1 };
                foreach (var (field, boost) in ContentFields)
                    bq.Add(new TermQuery(new Term(field, tokens[0])) { Boost = boost }, Occur.SHOULD);
                contentQuery = bq;
            }
            else
            {
                var combined = new BooleanQuery { MinimumNumberShouldMatch = 1 };

                foreach (var (field, boost) in ContentFields)
                {
                    var spanTerms = tokens
                        .Select(t => new SpanTermQuery(new Term(field, t)))
                        .ToArray<SpanQuery>();

                    combined.Add(new SpanNearQuery(spanTerms, slop: 0, inOrder: false) { Boost = boost * 3f }, Occur.SHOULD);
                    combined.Add(new SpanNearQuery(spanTerms, slop: 5, inOrder: false) { Boost = boost * 2f }, Occur.SHOULD);

                    var allPresent = new BooleanQuery { Boost = boost };
                    foreach (var t in tokens)
                        allPresent.Add(new TermQuery(new Term(field, t)), Occur.MUST);
                    combined.Add(allPresent, Occur.SHOULD);
                }

                contentQuery = combined;
            }

            var nameTopDocs = await Task.Run(() => indexSearcher.Search(nameQuery, null, 100));
            var contentTopDocs = await Task.Run(() => indexSearcher.Search(contentQuery, null, 100));

            var results = new Dictionary<int, SearchEntry>();

            foreach (var hit in nameTopDocs.ScoreDocs)
            {
                var doc = indexSearcher.Doc(hit.Doc);
                var id = Convert.ToInt32(doc.Get(F_CANDIDATE_ID));
                results[id] = new SearchEntry { Id = id, Score = 90 };
            }

            foreach (var hit in contentTopDocs.ScoreDocs)
            {
                var doc = indexSearcher.Doc(hit.Doc);
                var id = Convert.ToInt32(doc.Get(F_CANDIDATE_ID));
                if (results.ContainsKey(id)) continue;
                results[id] = new SearchEntry { Id = id, Score = 50 };
            }

            return results.Values
                .OrderByDescending(x => x.Score)
                .ToList();
        }

        #region Search Within Search
        // ─────────────────────────────────────────────
        // Search Within Search
        // ─────────────────────────────────────────────

        public async Task<List<SearchEntry>> SearchWithin(
            IEnumerable<int> previousResultIds,
            searchCandCvModel searchVals)
        {
            var idSet = previousResultIds.ToHashSet();
            if (idSet.Count == 0) return [];

            using var indexDirectory = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            using var indexReader = DirectoryReader.Open(indexDirectory);
            var indexSearcher = new IndexSearcher(indexReader);

            // ── Build ID filter from previous results ──────────────────────────────
            // Collect internal Lucene doc numbers that match the previous result IDs
            var bits = new OpenBitSet(indexReader.MaxDoc);
            foreach (var leafCtx in indexReader.Leaves)
            {
                var leafReader = leafCtx.AtomicReader;
                var liveDocs = leafReader.LiveDocs; // null means all docs are live
                for (int i = 0; i < leafReader.MaxDoc; i++)
                {
                    if (liveDocs != null && !liveDocs.Get(i)) continue; // skip deleted
                    var doc = leafReader.Document(i);
                    var rawId = doc.Get(F_CANDIDATE_ID);
                    if (rawId != null && idSet.Contains(Convert.ToInt32(rawId)))
                        bits.Set(leafCtx.DocBase + i); // global doc number
                }
            }

            var idFilter = new BitSetFilter(bits);

            // ── Build the new search query (reuse your existing logic) ────────────
            bool isExact = searchVals.exact;
            var tokens = searchVals.value.Trim().ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToArray();

            if (tokens.Length == 0) return [];

            Query contentQuery = isExact
                ? BuildExactContentQuery(tokens)
                : BuildFuzzyContentQuery(tokens);

            // ── Execute search restricted to previous IDs ─────────────────────────
            var topDocs = await Task.Run(() =>
                indexSearcher.Search(contentQuery, idFilter, idSet.Count));

            var maxScore = topDocs.MaxScore;

            return topDocs.ScoreDocs
                .Select(hit =>
                {
                    var doc = indexSearcher.Doc(hit.Doc);
                    var id = Convert.ToInt32(doc.Get(F_CANDIDATE_ID));
                    var normalised = maxScore > 0 ? hit.Score / maxScore : 1f;
                    return new SearchEntry
                    {
                        Id = id,
                        Score = 1 + (int)Math.Round(normalised * 99)
                    };
                })
                .OrderByDescending(x => x.Score)
                .ToList();
        }

        // ─────────────────────────────────────────────
        // Shared query builders (extracted from existing methods)
        // ─────────────────────────────────────────────

        private Query BuildFuzzyContentQuery(string[] tokens, int maxEdits = 2)
        {
            Query BuildToken(string token)
            {
                var q = new BooleanQuery();
                foreach (var (field, boost) in ContentFields)
                {
                    q.Add(new FuzzyQuery(new Term(field, token), maxEdits) { Boost = boost }, Occur.SHOULD);
                    q.Add(new WildcardQuery(new Term(field, token + "*")) { Boost = boost * 0.8f }, Occur.SHOULD);
                }
                return q;
            }

            if (tokens.Length == 1) return BuildToken(tokens[0]);

            var bq = new BooleanQuery();
            foreach (var t in tokens)
                bq.Add(BuildToken(t), Occur.MUST);
            return bq;
        }

        private Query BuildExactContentQuery(string[] tokens)
        {
            if (tokens.Length == 1)
            {
                var bq = new BooleanQuery { MinimumNumberShouldMatch = 1 };
                foreach (var (field, boost) in ContentFields)
                    bq.Add(new TermQuery(new Term(field, tokens[0])) { Boost = boost }, Occur.SHOULD);
                return bq;
            }

            var combined = new BooleanQuery { MinimumNumberShouldMatch = 1 };
            foreach (var (field, boost) in ContentFields)
            {
                var spanTerms = tokens
                    .Select(t => new SpanTermQuery(new Term(field, t)))
                    .ToArray<SpanQuery>();

                combined.Add(new SpanNearQuery(spanTerms, slop: 0, inOrder: false) { Boost = boost * 3f }, Occur.SHOULD);
                combined.Add(new SpanNearQuery(spanTerms, slop: 5, inOrder: false) { Boost = boost * 2f }, Occur.SHOULD);

                var allPresent = new BooleanQuery { Boost = boost };
                foreach (var t in tokens)
                    allPresent.Add(new TermQuery(new Term(field, t)), Occur.MUST);
                combined.Add(allPresent, Occur.SHOULD);
            }
            return combined;
        }

        // ─────────────────────────────────────────────
        // BitSetFilter — restricts search to a set of Lucene doc numbers
        // ─────────────────────────────────────────────

        private sealed class BitSetFilter : Filter
        {
            private readonly OpenBitSet _bits;
            public BitSetFilter(OpenBitSet bits) => _bits = bits;

            public override DocIdSet GetDocIdSet(AtomicReaderContext context, IBits acceptDocs)
            {
                // Offset bits into the segment's docBase
                var segmentBits = new OpenBitSet(context.Reader.MaxDoc);
                int docBase = context.DocBase;
                for (int i = 0; i < context.Reader.MaxDoc; i++)
                {
                    if (_bits.Get(docBase + i))
                        segmentBits.Set(i);
                }
                return segmentBits;
            }
        }
        #endregion

        #region Complex Search

        // ─────────────────────────────────────────────
        // Complex search: multiple groups AND'd via search-within
        // ─────────────────────────────────────────────

        public async Task<List<SearchEntry>> ComplexSearch(
     List<ComplexSearchTerm> firstSearch,
     List<ComplexSearchTerm>? searchWithin = null)
        {
            if (firstSearch.Count == 0) return [];

            var results = await RunGroupSearch(firstSearch, restrictToIds: null);
            if (results.Count == 0 || searchWithin is not { Count: > 0 })
                return results;

            return await RunGroupSearch(searchWithin, results.Select(r => r.Id).ToHashSet());
        }

        private async Task<List<SearchEntry>> RunGroupSearch(List<ComplexSearchTerm> terms, HashSet<int>? restrictToIds)
        {
            using var indexDirectory = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            using var indexReader = DirectoryReader.Open(indexDirectory);
            var indexSearcher = new IndexSearcher(indexReader);

            var groupQuery = BuildComplexGroupQuery(terms);

            Filter? filter = null;
            if (restrictToIds is { Count: > 0 })
            {
                var bits = new OpenBitSet(indexReader.MaxDoc);
                foreach (var leafCtx in indexReader.Leaves)
                {
                    var leafReader = leafCtx.AtomicReader;
                    var liveDocs = leafReader.LiveDocs;
                    for (int i = 0; i < leafReader.MaxDoc; i++)
                    {
                        if (liveDocs != null && !liveDocs.Get(i)) continue;
                        var rawId = leafReader.Document(i).Get(F_CANDIDATE_ID);
                        if (rawId != null && restrictToIds.Contains(Convert.ToInt32(rawId)))
                            bits.Set(leafCtx.DocBase + i);
                    }
                }
                filter = new BitSetFilter(bits);
            }

            int limit = restrictToIds?.Count ?? 1000;
            var topDocs = await Task.Run(() => indexSearcher.Search(groupQuery, filter, limit));
            var maxScore = topDocs.MaxScore;

            return topDocs.ScoreDocs
                .Select(hit =>
                {
                    var doc = indexSearcher.Doc(hit.Doc);
                    var id = Convert.ToInt32(doc.Get(F_CANDIDATE_ID));
                    var normalised = maxScore > 0 ? hit.Score / maxScore : 1f;
                    return new SearchEntry
                    {
                        Id = id,
                        Score = 1 + (int)Math.Round(normalised * 99)
                    };
                })
                .OrderByDescending(x => x.Score)
                .ToList();
        }

        // ─────────────────────────────────────────────
        // Group query builder
        // ─────────────────────────────────────────────

        private Query BuildComplexGroupQuery(List<ComplexSearchTerm> terms)
        {
            var bq = new BooleanQuery();

            foreach (var term in terms)
            {
                var occur = term.Occur == TermOccur.Must ? Occur.MUST : Occur.SHOULD;
                var query = term.MatchType == TermMatchType.ExactPhrase
                    ? BuildExactPhraseQuery(term.Value)
                    : BuildFuzzyTermQuery(term.Value);
                bq.Add(query, occur);
            }

            return bq;
        }

        // ─────────────────────────────────────────────
        // Exact phrase: SpanNear slop=0 across all content fields
        // ─────────────────────────────────────────────

        private Query BuildExactPhraseQuery(string phrase)
        {
            var tokens = phrase.Trim().ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            var bq = new BooleanQuery { MinimumNumberShouldMatch = 1 };

            foreach (var (field, boost) in ContentFields)
            {
                if (tokens.Length == 1)
                {
                    bq.Add(new TermQuery(new Term(field, tokens[0])) { Boost = boost }, Occur.SHOULD);
                }
                else
                {
                    var spanTerms = tokens
                        .Select(t => new SpanTermQuery(new Term(field, t)))
                        .ToArray<SpanQuery>();

                    // slop=0, inOrder=true → true exact phrase
                    bq.Add(new SpanNearQuery(spanTerms, slop: 0, inOrder: true) { Boost = boost * 3f }, Occur.SHOULD);
                }
            }

            return bq;
        }

        // ─────────────────────────────────────────────
        // Keyword (fuzzy): reuses existing BuildFuzzyContentQuery
        // ─────────────────────────────────────────────

        private Query BuildFuzzyTermQuery(string keyword)
        {
            var tokens = keyword.Trim().ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToArray();

            return BuildFuzzyContentQuery(tokens); // already exists in your class
        }
        #endregion


        // ─────────────────────────────────────────────
        // Dispose
        // ─────────────────────────────────────────────

        public void Dispose()
        {
            mIndexReader?.Dispose();
            mIndexDirectory?.Dispose();
        }
    }
}