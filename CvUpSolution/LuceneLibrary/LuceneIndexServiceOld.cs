using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using GeneralLibrary;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Extensions.Configuration;
using QueueLibrary;

namespace LuceneLibrary
{
    public class LuceneIndexServiceOld : ILuceneIndexServiceOld
    {
        private readonly string _indexFolder;
        private readonly Analyzer _analyzer;
        private readonly ILuceneQueries _luceneQueries;
        private readonly IDbQueueService _queueService;
        private readonly int _companyId;

        public LuceneIndexServiceOld(IConfiguration configuration, ILuceneQueries luceneQueries, IDbQueueService queueService, int companyId = 154)
        {
            var root = configuration["APP_LOCAL_ROOT_FOLDER"];
            _companyId = companyId;
            _indexFolder = $"{root}\\_{companyId}\\luceneIndex";
            _analyzer = new WhitespaceAnalyzer(LuceneVersion.LUCENE_48);
            _luceneQueries = luceneQueries;
            _queueService = queueService;
        }

        public async Task IndexAllCandidates()
        {
            List<CandLastCvModel> allCandsTextToIndexList = await _luceneQueries.AllCandidatesLastCv();

            using var indexDir = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, _analyzer);
            using var indexWriter = new IndexWriter(indexDir, config);

            indexWriter.DeleteAll();

            foreach (var cnd in allCandsTextToIndexList)
                await Task.Run(() => indexWriter.AddDocument(CandTextToDocument(cnd)));
        }

        public async Task<bool> IndexNewCvFromQueue()
        {
            var job = await _queueService.DequeueAsync("index cv", "LuceneIndexService");

            if (job == null) return false;

            try
            {
                int candidateId = int.Parse(job.payload);
                await AddUpdateCandidateDataToIndex(candidateId);
                await _queueService.CompleteAsync(job.id);
                Console.WriteLine($"Queue indexed candidate {candidateId}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Queue index failed: {ex.Message}");
                await _queueService.FailAsync(job.id);
                return true;
            }
        }
        
        private async Task AddUpdateCandidateDataToIndex(int candidateId)
        {
            CandLastCvModel? cvPropsToIndex = await _luceneQueries.CandidateLastCv(candidateId);

            if (cvPropsToIndex == null) return;

            await DocumentDelete(cvPropsToIndex.candidateId);

            using var indexDir = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, _analyzer);
            using var indexWriter = new IndexWriter(indexDir, config);
            await Task.Run(() => indexWriter.AddDocument(CandTextToDocument(cvPropsToIndex)));
        }


        private async Task DocumentDelete(int id)
        {
            using var indexDir = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, _analyzer);
            using var indexWriter = new IndexWriter(indexDir, config);
            await Task.Run(() => indexWriter.DeleteDocuments(new TermQuery(new Term("Id", id.ToString()))));
        }

        //        private Document documentToIndex(CvsToIndexModel cvCand)
        //        {
        //            var phoneNumbersOnly = string.Concat(nn(cvCand.phone).Where(char.IsNumber));

        //            string txtToIndex = $"{nn(cvCand.email)} {phoneNumbersOnly} {nn(cvCand.reviewText)} {nn(cvCand.firstName)} {nn(cvCand.lastName)} {nn(cvCand.cvsTxt)}";
        //            txtToIndex = txtIndexMange(txtToIndex);

        //            var updatedDate = cvCand.lastCvSent != null ? ((DateTimeOffset)cvCand.lastCvSent).ToUnixTimeSeconds() : 0;

        //            var doc = new Document() {{ new TextField("Id", cvCand.candidateId.ToString(), Lucene.Net.Documents.Field.Store.YES) },
        //                { new StoredField("Updated",updatedDate)},
        //                {new TextField("CV", txtToIndex, Lucene.Net.Documents.Field.Store.YES) }};

        //            return doc;
        //        }
        private Document CandTextToDocument(CandLastCvModel cvCand)
        {
            var name = $"{cvCand.firstName} {cvCand.lastName}".ToLowerInvariant();
            var meta = $"{cvCand.candidateId} {cvCand.email} {cvCand.phone}".ToLowerInvariant();
            var plainText = StringMethods.ExtractPlainText(cvCand.cvTxt ?? "").ToLowerInvariant();
            var reviewText = (cvCand.reviewText ?? "").ToLowerInvariant();

            return new Document
            {
                new TextField("Id", cvCand.candidateId.ToString(), Field.Store.YES),
                new StoredField("Updated", DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                new TextField("Name", name, Field.Store.YES),
                new TextField("Review", reviewText, Field.Store.YES),
                new TextField("CV", $"{meta} {plainText}", Field.Store.YES)
            };
        }
    }
}
