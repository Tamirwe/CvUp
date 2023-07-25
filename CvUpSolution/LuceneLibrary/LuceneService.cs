using DataModelsLibrary.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lucene.Net.Util.Packed.PackedInt32s;

namespace LuceneLibrary
{
    public class LuceneService : ILuceneService, IDisposable
    {
        //string _luceneIndexesRootFolder;
        string _filesRootFolder;

        Analyzer? mAnalyzer;
        IndexSearcher? mIndexSearcher;
        FSDirectory? mIndexDirectory;
        IndexReader? mIndexReader;
        QueryParser? mQueryParser;

        public LuceneService(IConfiguration config)
        {
            _filesRootFolder = $"{config["GlobalSettings:CvUpFilesRootFolder"]}";
            System.IO.Directory.CreateDirectory($"{_filesRootFolder}//luceneIndex");
            mAnalyzer = new WhitespaceAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_48);
            //_luceneIndexesRootFolder = config["GlobalSettings:LuceneIndexesRootFolder"];
            //_indexFolder = @"C:\KB\CvUp\CvUpSolution\LuceneLibrary\Index";
        }

        public async Task<List<SearchEntry>> Search(int companyId, string searchQuery)
        {
            string _indexFolder = $"{_filesRootFolder}\\_{companyId}//luceneIndex";
            //string _indexFolder = $"{_luceneIndexesRootFolder}\\_{companyId}index";
            mIndexDirectory = FSDirectory.Open(new System.IO.DirectoryInfo(_indexFolder));
            mIndexReader = DirectoryReader.Open(mIndexDirectory);
            mIndexSearcher = new IndexSearcher(mIndexReader);
            mQueryParser = new QueryParser(Lucene.Net.Util.LuceneVersion.LUCENE_48, "CV", mAnalyzer);

            var result = new List<SearchEntry>();

            //FuzzyQuery query = new FuzzyQuery(new Term("CV", searchQuery));

            //query.setRewriteMethod(FuzzyQuery.SCORING_BOOLEAN_QUERY_REWRITE);
            //ScoreDoc[] hits = searcher.Search(query, null, 1000).ScoreDocs;
            if (mQueryParser != null)
            {
                var query = mQueryParser.Parse(searchQuery.ToLower());
                //var query = new WildcardQuery(new Term("CV", searchQuery));

                if (mIndexSearcher != null)
                {
                    ScoreDoc[] hitIdxs = await Task.Run(() => mIndexSearcher.Search(query, null, 5000).ScoreDocs);

                    for (int i = 0; i < hitIdxs.Length; i++)
                    {
                        var doc = mIndexSearcher.Doc(hitIdxs[i].Doc);

                        result.Add(new SearchEntry
                        {
                            Id = Convert.ToInt32(doc.Get("Id")),
                            CandId = Convert.ToInt32(doc.Get("CAND_Id")),
                            CV = doc.Get("CV")
                        });
                    }
                }
            };

            return result;
        }

        public async Task BuildCompanyIndex(int companyId, List<CvsToIndexModel> CompanyTextToIndexList)
        {
            try
            {

            
            //string _indexFolder = $"{_luceneIndexesRootFolder}\\_{companyId}index";
                string _indexFolder = $"{_filesRootFolder}\\_{companyId}//luceneIndex";

                using (var indexDir = FSDirectory.Open(new System.IO.DirectoryInfo(_indexFolder)))
            {
                var config = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, mAnalyzer);

                using (var indexWriter = new IndexWriter(indexDir, config))
                {
                    indexWriter.DeleteAll();

                    foreach (var item in CompanyTextToIndexList)
                    {
                        //string txtToIndex = $"{item.email}~~~{item.phone}~~~{item.emailSubject}~~~{item.cvTxt}".ToLower();
                        //var doc = new Document();
                        //doc.Add(new TextField("Id", item.cvId.ToString(), Field.Store.YES));
                        //doc.Add(new TextField("CAND_Id", item.candidateId.ToString(), Field.Store.YES));
                        //doc.Add(new TextField("CV", txtToIndex, Field.Store.YES));
                        await Task.Run(() => indexWriter.AddDocument(documentToIndex(item)));
                    }
                }
            }

            }
            catch (Exception ex)
            {

                throw ;
            }
        }

        public async Task DocumentAdd(int companyId, CvsToIndexModel cvPropsToIndex)
        {
            //string _indexFolder = $"{_luceneIndexesRootFolder}\\_{companyId}index";
            string _indexFolder = $"{_filesRootFolder}\\_{companyId}//luceneIndex";

            using (var indexDir = FSDirectory.Open(new System.IO.DirectoryInfo(_indexFolder)))
            {
                var config = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, mAnalyzer);

                using (var indexWriter = new IndexWriter(indexDir, config))
                {

                    //string txtToIndex = $"{cvPropsToIndex.email}~~~{cvPropsToIndex.phone}~~~{cvPropsToIndex.emailSubject}~~~{cvPropsToIndex.cvTxt}";
                    //var doc = new Document();
                    //doc.Add(new TextField("Id", cvPropsToIndex.cvId.ToString(), Field.Store.YES));
                    //doc.Add(new TextField("CAND_Id", cvPropsToIndex.candidateId.ToString(), Field.Store.YES));
                    //doc.Add(new TextField("CV", txtToIndex, Field.Store.YES));
                    await Task.Run(() => indexWriter.AddDocument(documentToIndex(cvPropsToIndex)));
                }
            }
        }

        private Document documentToIndex(CvsToIndexModel cvCand)
        {
            string txtToIndex = $"{cvCand.email}~~~{cvCand.phone}~~~{cvCand.reviewText}~~~{cvCand.firstName}~~~{cvCand.lastName}~~~{cvCand.emailSubject}~~~{cvCand.cvTxt}".ToLower();
            var doc = new Document();
            doc.Add(new TextField("Id", cvCand.cvId.ToString(), Field.Store.YES));
            doc.Add(new TextField("CAND_Id", cvCand.candidateId.ToString(), Field.Store.YES));
            doc.Add(new TextField("CV", txtToIndex, Field.Store.YES));

            return doc;
        }

        public async Task DocumentUpdate(int companyId, CvsToIndexModel cvPropsToIndex)
        {
            await DocumentDelete(companyId, cvPropsToIndex.cvId);
            await DocumentAdd(companyId, cvPropsToIndex);
        }

        private async Task DocumentDelete(int companyId, int cvId)
        {
            //string _indexFolder = $"{_luceneIndexesRootFolder}\\_{companyId}index";
            string _indexFolder = $"{_filesRootFolder}\\_{companyId}//luceneIndex";

            using (var indexDir = FSDirectory.Open(new System.IO.DirectoryInfo(_indexFolder)))
            {
                var config = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, mAnalyzer);

                using (var indexWriter = new IndexWriter(indexDir, config))
                {
                    var DocIdToDelete = new TermQuery(new Term("Id", cvId.ToString()));
                    await Task.Run(() => indexWriter.DeleteDocuments(DocIdToDelete));
                }
            }
        }

        public void SearchBoolean(int companyId, long collectionId, string text)
        {
            //string _indexFolder = $"{_luceneIndexesRootFolder}\\_{companyId}index";
            string _indexFolder = $"{_filesRootFolder}\\_{companyId}//luceneIndex";
            Console.WriteLine();
            Console.WriteLine("SEARCH EXAMPLE");
            Console.WriteLine("SEARCHING FOR: \"" + text + "\" IN COLLECTION " + collectionId);

            using (FSDirectory directory = FSDirectory.Open(_indexFolder))

            using (Analyzer analyzer = new WhitespaceAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_48))

            using (IndexReader reader = DirectoryReader.Open(directory))
            {
                IndexSearcher searcher = new IndexSearcher(reader);

                MultiFieldQueryParser queryParser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, new[] { "title", "body" }, analyzer);

                Query searchTermQuery = queryParser.Parse(text);

                Query collectionIdQuery = NumericRangeQuery.NewInt64Range("collectionId", collectionId, collectionId, true, true);

                BooleanQuery aggregateQuery = new BooleanQuery() { { searchTermQuery, Occur.MUST }, { collectionIdQuery, Occur.MUST } };

                // perform search
                TopDocs topDocs = searcher.Search(aggregateQuery, 10);

                Console.WriteLine("\nSearch returned {0} results", topDocs.ScoreDocs.Length);

                // display results
                foreach (ScoreDoc scoreDoc in topDocs.ScoreDocs)
                {
                    float score = scoreDoc.Score;
                    int docId = scoreDoc.Doc;
                    Document doc = searcher.Doc(docId);

                    // fields of search hit
                    string id = doc.Get("id");
                    string cId = doc.Get("collectionId");
                    string title = doc.Get("title");
                    string body = doc.Get("body");

                    Console.WriteLine("  score:{0}  id:{1}  collectionId:{2}  " + "title:\"{3}\"  body:\"{4}\"", score, id, cId, title, body);
                }
            }

            Console.WriteLine();
        }

        public void Dispose()
        {
            if (mIndexReader != null) mIndexReader.Dispose();
            if (mIndexDirectory != null) mIndexDirectory.Dispose();
        }

    }

    public class SearchEntry
    {
        public int Id { get; set; }
        public int CandId { get; set; }
        public string CV { get; set; } = String.Empty;
    }
}