using DataModelsLibrary.Models;
using GeneralLibrary;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace LuceneLibrary
{
    public class LuceneIndexService : ILuceneIndexService
    {
        private readonly string _indexFolder;
        private readonly Analyzer _analyzer;
        private readonly ILuceneQueries _luceneQueries;
        private readonly int _companyId;

        public LuceneIndexService(IConfiguration configuration, ILuceneQueries luceneQueries, int companyId = 154)
        {
            var root = configuration["APP_LOCAL_ROOT_FOLDER"];
            _companyId = companyId;
            _indexFolder = $"{root}\\_{companyId}\\luceneIndex";
            _analyzer = new WhitespaceAnalyzer(LuceneVersion.LUCENE_48);
            _luceneQueries = luceneQueries;
        }

        public async Task AddUpdateCandidateDataToIndex(int companyId, int candidateId)
        {
            List<CvsToIndexModel> cvPropsToIndexList = await _luceneQueries.GetCandidatesLastCvsToIndex(companyId, candidateId);

            if (cvPropsToIndexList.Count == 0) return;

            var candidateDataToIndex = cvPropsToIndexList.First();

            await DocumentDelete(candidateDataToIndex.candidateId);

            using var indexDir = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, _analyzer);
            using var indexWriter = new IndexWriter(indexDir, config);
            await Task.Run(() => indexWriter.AddDocument(CandTextToDocument(candidateDataToIndex)));
        }

        public async Task IndexAllCandidates()
        {
            List<CvsToIndexModel> allCandsTextToIndexList = await _luceneQueries.GetCandidatesLastCvsToIndex(_companyId, 0);

            using var indexDir = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, _analyzer);
            using var indexWriter = new IndexWriter(indexDir, config);

            indexWriter.DeleteAll();

            foreach (var cnd in allCandsTextToIndexList)
                await Task.Run(() => indexWriter.AddDocument(CandTextToDocument(cnd)));
        }

        private async Task DocumentDelete(int id)
        {
            using var indexDir = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, _analyzer);
            using var indexWriter = new IndexWriter(indexDir, config);
            await Task.Run(() => indexWriter.DeleteDocuments(new TermQuery(new Term("Id", id.ToString()))));
        }

        private Document CandTextToDocument(CvsToIndexModel cvCand)
        {
            var plainText = CleanString.ExtractPlainText(cvCand.cvsTxt ?? "").ToLowerInvariant();
            return new Document
            {
                new TextField("Id", cvCand.candidateId.ToString(), Field.Store.YES),
                new StoredField("Updated", DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                new TextField("CV", plainText, Field.Store.YES)
            };
        }
    }
}
