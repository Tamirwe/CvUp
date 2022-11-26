using DataModelsLibrary.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
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

namespace LuceneLibrary
{
    public class LuceneService : ILuceneService,IDisposable
    {
        string _luceneIndexesRootFolder;

        Analyzer? mAnalyzer;
        IndexSearcher? mIndexSearcher;
        FSDirectory? mIndexDirectory;
        IndexReader? mIndexReader;
        QueryParser? mQueryParser;

        public LuceneService(IConfiguration config)
        {
            _luceneIndexesRootFolder = config["GlobalSettings:LuceneIndexesRootFolder"];
            System.IO.Directory.CreateDirectory(_luceneIndexesRootFolder);

            //_indexFolder = @"C:\KB\CvUp\CvUpSolution\LuceneLibrary\Index";

            mAnalyzer = new WhitespaceAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_48);
        }

        public void BuildCompanyIndex(int companyId, List<CompanyTextToIndexModel> CompanyTextToIndexList)
        {
            string _indexFolder = $"{_luceneIndexesRootFolder}\\_{companyId}index";

            using (var indexDir = FSDirectory.Open(new System.IO.DirectoryInfo(_indexFolder)))
            {
                var config = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, mAnalyzer);

                using (var indexWriter = new IndexWriter(indexDir, config))
                {
                    foreach (var item in CompanyTextToIndexList)
                    {
                        string txtToIndex = $"{item.email}~~~{item.phone}~~~{item.emailSubject}~~~{item.cvTxt}";
                        var doc = new Document();
                        doc.Add(new TextField("Id", item.cvId, Field.Store.YES));
                        doc.Add(new TextField("CV", txtToIndex, Field.Store.YES));
                        indexWriter.AddDocument(doc);
                    }
                }
            }
        }
    

    //public void BuildIndex()
    //    {
    //        if (System.IO.Directory.Exists(mIndexPath)) System.IO.Directory.Delete(mIndexPath, true);

    //        using (var indexDir = FSDirectory.Open(new System.IO.DirectoryInfo(mIndexPath)))
    //        {
    //            var config = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, mAnalyzer);

    //            using (var indexWriter = new IndexWriter(indexDir, config))
    //            {

    //                //for (int i = 0; i < dataToIndex.Length; ++i)
    //                //{
    //                //    var row = dataToIndex[i];

    //                var doc = new Document();
    //                doc.Add(new Int32Field("Id", 1, Field.Store.YES));
    //                doc.Add(new Int32Field("companyId", 111, Field.Store.YES));
    //                doc.Add(new TextField("Name", "שלבנון העבירה לאמריקנים את ההערות שלה לטיוטת בהסכם", Field.Store.YES));
    //                indexWriter.AddDocument(doc);

    //                doc = new Document();
    //                doc.Add(new Int32Field("Id", 2, Field.Store.YES));
    //                doc.Add(new Int32Field("companyId", 111, Field.Store.YES));
    //                doc.Add(new TextField("Name", "הימים הקרובים יהיו המכריעים ביותר בשאלת החתימה על הסכם הגז של לבנון וישראל", Field.Store.YES));
    //                indexWriter.AddDocument(doc);

    //                doc = new Document();
    //                doc.Add(new Int32Field("Id", 3, Field.Store.YES));
    //                doc.Add(new Int32Field("companyId", 222, Field.Store.YES));
    //                doc.Add(new TextField("Name", "גורם מדיני בכיר אמר שאם לבנון תדרוש להכניס שינויים מהותיים בטיוטת אבהסכם", Field.Store.YES));
    //                indexWriter.AddDocument(doc);


    //                doc = new Document();
    //                doc.Add(new Int32Field("Id", 4, Field.Store.YES));
    //                doc.Add(new Int32Field("companyId", 222, Field.Store.YES));
    //                doc.Add(new TextField("Name", "החשיבות בהסכם בעת הזאת היא לבלום את ההשפעה העצומה של איראן וחיזבאללה", Field.Store.YES));
    //                indexWriter.AddDocument(doc);

    //                doc = new Document();
    //                doc.Add(new Int32Field("Id", 5, Field.Store.YES));
    //                doc.Add(new Int32Field("companyId", 333, Field.Store.YES));
    //                doc.Add(new TextField("Name", "יציגו היום ראש הממשלה יאיר לפיד וצוות המשא ומתן הישראלי את טיוטת בעהסכם", Field.Store.YES));
    //                indexWriter.AddDocument(doc);


    //                doc = new Document();
    //                doc.Add(new Int32Field("Id", 6, Field.Store.YES));
    //                doc.Add(new Int32Field("companyId", 333, Field.Store.YES));
    //                doc.Add(new TextField("Name", "fff", Field.Store.YES));
    //                indexWriter.AddDocument(doc);
    //                //}
    //            }
    //        }
    //    }

        //public void DocumentAdd(int companyId, string cvId, string cvTxt)
        //{
        //    using (var indexDir = FSDirectory.Open(new System.IO.DirectoryInfo(mIndexPath)))
        //    {
        //        var config = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, mAnalyzer);

        //        using (var indexWriter = new IndexWriter(indexDir, config))
        //        {

        //            //for (int i = 0; i < dataToIndex.Length; ++i)
        //            //{
        //            //var row = dataToIndex[i];

        //            //var doc = new Document();
        //            //doc.Add(new Int32Field("Id", row.Id, Field.Store.YES));
        //            //doc.Add(new TextField("Name", row.Name, Field.Store.YES));
        //            //indexWriter.AddDocument(doc);
        //            //}
        //        }
        //    }
        //}




        public void DocumentDelete()
        {
        }


        public void WarmupSearch(int companyId)
        {
            string _indexFolder = $"{_luceneIndexesRootFolder}\\_{companyId}index";
            mIndexDirectory = FSDirectory.Open(new System.IO.DirectoryInfo(_indexFolder));
            mIndexReader = DirectoryReader.Open(mIndexDirectory);
            mIndexSearcher = new IndexSearcher(mIndexReader);
            mQueryParser = new QueryParser(Lucene.Net.Util.LuceneVersion.LUCENE_48, "Name", mAnalyzer);
        }

        public IEnumerable<SearchEntry> Search(int companyId, string searchQuery)
        {
            var result = new List<SearchEntry>();

            FuzzyQuery query = new FuzzyQuery(new Term("Name", "הסכם"));

            //query.setRewriteMethod(FuzzyQuery.SCORING_BOOLEAN_QUERY_REWRITE);
            //ScoreDoc[] hits = searcher.Search(query, null, 1000).ScoreDocs;

            //var query = mQueryParser.Parse("?הסכם");
            //var query = new WildcardQuery(new Term("name", "הסכם?"));


            ScoreDoc[] hitIdxs = mIndexSearcher.Search(query, null, 40).ScoreDocs;

            for (int i = 0; i < hitIdxs.Length; i++)
            {
                var doc = mIndexSearcher.Doc(hitIdxs[i].Doc);

                result.Add(new SearchEntry
                {
                    Id = (int)doc.GetField("Id").NumericType,
                    Name = doc.Get("Name")
                });
            }

            return result;
        }

        public void SearchBoolean(int companyId,long collectionId, string text)
        {
            string _indexFolder = $"{_luceneIndexesRootFolder}\\_{companyId}index";
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
        public string Name { get; set; } = String.Empty;
    }

    

}