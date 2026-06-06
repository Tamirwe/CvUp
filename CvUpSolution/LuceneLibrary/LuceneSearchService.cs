using DataModelsLibrary.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Search.Spans;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.RegularExpressions;

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

            Query BuildTokenQuery(string token)
            {
                var q = new BooleanQuery();
                q.Add(new FuzzyQuery(new Term("CV", token), maxEdits), Occur.SHOULD);
                q.Add(new WildcardQuery(new Term("CV", token + "*")), Occur.SHOULD);
                return q;
            }

            Query query;
            if (tokens.Length == 1)
            {
                query = BuildTokenQuery(tokens[0]);
            }
            else
            {
                var boolQuery = new BooleanQuery();
                foreach (var token in tokens)
                    boolQuery.Add(BuildTokenQuery(token), Occur.MUST);
                query = boolQuery;
            }

            var topDocs = await Task.Run(() => indexSearcher.Search(query, null, 10000));
            var maxScore = topDocs.MaxScore;

            var result = new List<SearchEntry>();
            foreach (var hit in topDocs.ScoreDocs)
            {
                var doc = indexSearcher.Doc(hit.Doc);
                result.Add(new SearchEntry
                {
                    Id = Convert.ToInt32(doc.Get("Id")),
                    UpdatedTs = Convert.ToInt64(doc.Get("Updated")),
                    CV = doc.Get("CV"),
                    Score = maxScore > 0 ? Math.Min((int)Math.Round(hit.Score / maxScore * 100, 0), 90) : 0
                });
            }

            return result.OrderByDescending(x => x.Score).ThenByDescending(x => x.UpdatedTs).ToList();
        }

        private async Task<List<SearchEntry>> ExactSearch(int companyId, searchCandCvModel searchVals)
        {
            mIndexDirectory = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            mIndexReader = DirectoryReader.Open(mIndexDirectory);
            var indexSearcher = new IndexSearcher(mIndexReader);

            var nameQuery = searchVals.value.Trim().ToLowerInvariant();
            var tokens = nameQuery
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToArray();

            Query query;
            if (tokens.Length == 1)
            {
                query = new TermQuery(new Term("CV", tokens[0]));
            }
            else
            {
                var spanTerms = tokens
                    .Select(t => new SpanTermQuery(new Term("CV", t)))
                    .ToArray<SpanQuery>();
                query = new SpanNearQuery(spanTerms, slop: 0, inOrder: false);
            }

            var topDocs = await Task.Run(() => indexSearcher.Search(query, null, 10000));
            var maxScore = topDocs.MaxScore;

            var result = new List<SearchEntry>();
            foreach (var hit in topDocs.ScoreDocs)
            {
                var doc = indexSearcher.Doc(hit.Doc);
                result.Add(new SearchEntry
                {
                    Id = Convert.ToInt32(doc.Get("Id")),
                    UpdatedTs = Convert.ToInt64(doc.Get("Updated")),
                    CV = doc.Get("CV"),
                    Score = 100
                });
            }

            return result.OrderByDescending(x => x.UpdatedTs).ToList();
        }

        public void Dispose()
        {
            mIndexReader?.Dispose();
            mIndexDirectory?.Dispose();
        }
    }
}
