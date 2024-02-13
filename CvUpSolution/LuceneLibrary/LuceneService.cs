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
using MailKit.Search;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Lucene.Net.Util.Packed.PackedInt32s;
using static System.Net.Mime.MediaTypeNames;

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
            System.IO.Directory.CreateDirectory($"{_filesRootFolder}\\luceneIndex");
            mAnalyzer = new WhitespaceAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_48);
            //mAnalyzer = new ClassicAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_48);

            //_luceneIndexesRootFolder = config["GlobalSettings:LuceneIndexesRootFolder"];
            //_indexFolder = @"C:\KB\CvUp\CvUpSolution\LuceneLibrary\Index";
        }

        public async Task<List<SearchEntry>> Search(int companyId, searchCandCvModel searchVals)
        {
            string _indexFolder = $"{_filesRootFolder}\\_{companyId}\\luceneIndex";
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
                string keyWords = txtIndexMange(searchVals.value);

                //var query = mQueryParser.Parse(searchQuery.ToLower());


                //string[] words = keyWords.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                //string wildcardphrase = "";

                //foreach (var word in words)
                //{
                //    wildcardphrase += word + "* ";
                //}

                //wildcardphrase.Trim();

                //var query = new WildcardQuery(new Term("CV", wildcardphrase.ToLower()));




                BooleanQuery aggregateQuery = new BooleanQuery();

                string[] words = keyWords.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                foreach (var word in words)
                {
                    //if (searchVals.exact)
                    //{
                    //    var query = mQueryParser.Parse(word.ToLower());
                    //    aggregateQuery.Add(query, Occur.MUST);
                    //}
                    //else
                    //{
                        aggregateQuery.Add(new WildcardQuery(new Term("CV", "*" + word.ToLower() + "*")), Occur.MUST);
                    //}
                }

                if (mIndexSearcher != null)
                {
                    ScoreDoc[] hitIdxs = await Task.Run(() => mIndexSearcher.Search(aggregateQuery, null, 10000).ScoreDocs);
                    //ScoreDoc[] hitIdxs = await Task.Run(() => mIndexSearcher.Search(query, null, 5000).ScoreDocs);

                    for ( int i = 0; i < hitIdxs.Length; i++)
                    {
                        var doc = mIndexSearcher.Doc(hitIdxs[i].Doc);

                        result.Add(new SearchEntry
                        {
                            Id = Convert.ToInt32(doc.Get("Id")),
                            CV = doc.Get("CV"),
                            Score= (int)Math.Round(hitIdxs[i].Score*100,0)
                        });
                    }



                    if (searchVals.exact)
                    {

                        string managedKeyWords = txtIndexMange(searchVals.value);
                        var keyWordsToSearch = managedKeyWords.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();

                        // Define a regular expression pattern to match quoted phrases
                        //מנכל מפעל "שרשרת הספקה"
                        string pattern = "\"([^\"]*)\"";

                        // Match the pattern in the input string
                        MatchCollection matches = Regex.Matches(searchVals.value, pattern);

                        // Output each matched phrase
                        foreach (Match match in matches)
                        {
                            string phrase = match.Groups[1].Value;
                            keyWordsToSearch.Add(txtIndexMange(phrase));
                        }

                        foreach (var word in keyWordsToSearch)
                        {
                            result = result.Where(x => x.CV.Contains(word)).ToList();
                        }

                        if (matches.Count == 0 && keyWordsToSearch.Count > 1)
                        {
                            foreach (var item in result.Where(x => x.CV.Contains(managedKeyWords)))
                            {
                                item.Score = 1000;
                            }
                        }

                        result = result.OrderByDescending(x => x.Score).ToList();
                    }








                    //string advancedKeyWords = txtIndexMange(searchVals.advancedValue);
                    //string[] advancedWords = advancedKeyWords.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                    //foreach (var word in advancedWords)
                    //{
                    //    result = result.Where(x => x.CV.Contains(word)).ToList();
                    //}
                }
            };

            return result;
        }

        public async Task CompanyIndexAddDocuments(int companyId, List<CvsToIndexModel> CompanyTextToIndexList, bool isDeleteAllDocuments)
        {
            try
            {
                string _indexFolder = $"{_filesRootFolder}\\_{companyId}\\luceneIndex";

                using (var indexDir = FSDirectory.Open(new System.IO.DirectoryInfo(_indexFolder)))
                {
                    var config = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, mAnalyzer);

                    using (var indexWriter = new IndexWriter(indexDir, config))
                    {
                        if (isDeleteAllDocuments)
                        {
                            indexWriter.DeleteAll();
                        }

                        foreach (var item in CompanyTextToIndexList)
                        {
                            await Task.Run(() => indexWriter.AddDocument(documentToIndex(item)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Document documentToIndex(CvsToIndexModel cvCand)
        {
            var phoneNumbersOnly = string.Concat(nn(cvCand.phone).Where(char.IsNumber));

            string txtToIndex = $"{nn(cvCand.email)} {phoneNumbersOnly} {nn(cvCand.reviewText)} {nn(cvCand.firstName)} {nn(cvCand.lastName)} {nn(cvCand.cvsTxt)}";
            txtToIndex = txtIndexMange(txtToIndex);

            var doc = new Document() {{ new TextField("Id", cvCand.candidateId.ToString(), Field.Store.YES) },
                {new TextField("CV", txtToIndex, Field.Store.YES) }};

            return doc;
        }

        //private Document documentToIndex(CvsToIndexModel cvCand)
        //{
        //    string txtToIndex = $"{nn(cvCand.email)} {nn(cvCand.phone)} {nn(cvCand.reviewText)} {nn(cvCand.firstName)} {nn(cvCand.lastName)} {nn(cvCand.emailSubject)} {nn(cvCand.cvTxt)}";
        //    txtToIndex = txtIndexMange(txtToIndex);

        //    var doc = new Document() {{ new TextField("Id", cvCand.cvId.ToString(), Field.Store.YES) },
        //        {new TextField("CAND_Id", cvCand.candidateId.ToString(), Field.Store.YES) },
        //        {new TextField("CV", txtToIndex, Field.Store.YES) }};

        //    return doc;
        //}

        private string txtIndexMange(string txt)
        {
            string manageTxt = txt.Replace("'", "").Replace("\"", "").Replace("ך", "כ").Replace("ם", "מ").Replace("ף", "פ").Replace("ץ", "צ").ToLower();
            string pattern = @"\t|\n|\r|\p{P}";
            manageTxt = Regex.Replace(manageTxt, pattern, " ");
            //return manageTxt;

            string[] words = manageTxt.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            //var word_query =(from string word in words orderby word select word).Distinct();

            string[] result = words.Distinct().ToArray();

            var cvWords = string.Join(" ", result);

            return cvWords;
        }

        private string nn(string? str)
        {
            return str == null ? "" : str;
        }

        public async Task DocumentUpdate(int companyId, List<CvsToIndexModel> cvPropsToIndex)
        {
            await DocumentDelete(companyId, cvPropsToIndex.First().candidateId);
            await CompanyIndexAddDocuments(companyId, cvPropsToIndex,false);
        }

        private async Task DocumentDelete(int companyId, int id)
        {
            //string _indexFolder = $"{_luceneIndexesRootFolder}\\_{companyId}index";
            string _indexFolder = $"{_filesRootFolder}\\_{companyId}\\luceneIndex";

            using (var indexDir = FSDirectory.Open(new System.IO.DirectoryInfo(_indexFolder)))
            {
                var config = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, mAnalyzer);

                using (var indexWriter = new IndexWriter(indexDir, config))
                {
                    var DocIdToDelete = new TermQuery(new Term("Id", id.ToString()));
                    await Task.Run(() => indexWriter.DeleteDocuments(DocIdToDelete));
                }
            }
        }

        public void SearchBoolean(int companyId, long collectionId, string text)
        {
            //string _indexFolder = $"{_luceneIndexesRootFolder}\\_{companyId}index";
            string _indexFolder = $"{_filesRootFolder}\\_{companyId}\\luceneIndex";
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
        public string CV { get; set; } = String.Empty;
        public int Score { get; set; }
    }
}