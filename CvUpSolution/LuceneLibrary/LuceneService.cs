using DataModelsLibrary.Models;
using GeneralLibrary;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Documents;
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
    public class LuceneService : ILuceneService, IDisposable
    {
        private readonly string _indexFolder;
        private readonly int _companyId;

        string _cvupNotBackedUpRootFolder;

        Analyzer? mAnalyzer;
        IndexSearcher? mIndexSearcher;
        FSDirectory? mIndexDirectory;
        IndexReader? mIndexReader;
        QueryParser? mQueryParser;

        public LuceneService(IConfiguration configuration, int companyId=154)
        {
            _cvupNotBackedUpRootFolder = configuration["APP_LOCAL_ROOT_FOLDER"];

            _indexFolder = $"{_cvupNotBackedUpRootFolder}\\_{companyId}\\luceneIndex";
            _companyId = companyId;

            //System.IO.Directory.CreateDirectory($"{_filesRootFolder}\\luceneIndex");
            mAnalyzer = new WhitespaceAnalyzer(LuceneVersion.LUCENE_48);



            //mAnalyzer = new ClassicAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_48);
            //_luceneIndexesRootFolder = config["GlobalSettings:LuceneIndexesRootFolder"];
        }

        public async Task<List<SearchEntry>> Search(int companyId, searchCandCvModel searchVals)
        {
          

            mIndexDirectory = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            mIndexReader = DirectoryReader.Open(mIndexDirectory);
            mIndexSearcher = new IndexSearcher(mIndexReader);
            mQueryParser = new QueryParser(LuceneVersion.LUCENE_48, "CV", mAnalyzer);

            
            //var debug = await DebugEmailSearch(mIndexSearcher, "Leonardopalinsky@gmail.com");
            //Console.WriteLine(debug); // or log it

            ScoreDoc[] hitIdxs =    await SearchCandidateByName(mIndexSearcher, searchVals.value);

            var result = new List<SearchEntry>();

            for (int i = 0; i < hitIdxs.Length; i++)
            {
                var doc = mIndexSearcher.Doc(hitIdxs[i].Doc);

                result.Add(new SearchEntry
                {
                    Id = Convert.ToInt32(doc.Get("Id")),
                    UpdatedTs = Convert.ToInt64(doc.Get("Updated")),
                    //Updated = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(doc.Get("Updated"))).UtcDateTime,
                    CV = doc.Get("CV"),
                    Score = (int)Math.Round(hitIdxs[i].Score * 100, 0)
                });
            }

            result = result.OrderByDescending(x => x.UpdatedTs).ToList();
            return result;
        }

        public async Task<ScoreDoc[]> SearchCandidateByName(IndexSearcher mIndexSearcher, string nameQuery)
        {
            nameQuery = nameQuery.Trim().ToLowerInvariant();

            //// Email → single TermQuery
            //if (nameQuery.Contains('@'))
            //{
            //    return await Task.Run(() =>
            //        mIndexSearcher.Search(new TermQuery(new Term("CV", nameQuery)), null, 10000).ScoreDocs);
            //}

            var tokens = nameQuery
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToArray();

            // Single token → TermQuery
            if (tokens.Length == 1)
            {
                return await Task.Run(() =>
                    mIndexSearcher.Search(new TermQuery(new Term("CV", tokens[0])), null, 10000).ScoreDocs);
            }

            // Multi-word → SpanNearQuery, any order, adjacent
            var spanTerms = tokens
                .Select(t => new SpanTermQuery(new Term("CV", t)))
                .ToArray<SpanQuery>();

            var query = new SpanNearQuery(spanTerms, slop: 0, inOrder: false);

            return await Task.Run(() =>
                mIndexSearcher.Search(query, null, 10000).ScoreDocs);
        }


        public async Task<string> DebugEmailSearch(IndexSearcher mIndexSearcher, string email)
        {
            var sb = new StringBuilder();
            email = email.Trim().ToLowerInvariant();

            // Step 1: check what the index actually contains
            var reader = mIndexSearcher.IndexReader;
            sb.AppendLine($"Total docs in index: {reader.NumDocs}");

            // Step 2: scan all docs and look for the email manually
            for (int i = 0; i < reader.MaxDoc; i++)
            {
                var doc = reader.Document(i);
                var cv = doc.Get("CV");
                if (cv != null && cv.Contains(email))
                {
                    sb.AppendLine($"✓ Doc {i} CONTAINS email in stored text");
                }
            }

            // Step 3: try TermQuery and report
            var termQuery = new TermQuery(new Term("CV", email));
            var hits = mIndexSearcher.Search(termQuery, null, 10).ScoreDocs;
            sb.AppendLine($"TermQuery hits: {hits.Length}");

            // Step 4: check the actual tokens stored in the index for that field
            var terms = MultiFields.GetTerms(reader, "CV");
            if (terms != null)
            {
                var termsEnum = terms.GetIterator(null);
                var emailParts = email.Split('@', '.');
                BytesRef term;
                while ((term = termsEnum.Next()) != null)
                {
                    var termText = term.Utf8ToString();
                    // Print any token that looks like part of the email
                    if (emailParts.Any(p => termText.Contains(p)))
                        sb.AppendLine($"  Index token: '{termText}'");
                }
            }

            return sb.ToString();
        }


        public async Task<List<SearchEntry>> Search2(int companyId, searchCandCvModel searchVals)
        {
            mIndexDirectory = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            mIndexReader = DirectoryReader.Open(mIndexDirectory);
            mIndexSearcher = new IndexSearcher(mIndexReader);
            mQueryParser = new QueryParser(LuceneVersion.LUCENE_48, "CV", mAnalyzer);

            var result = new List<SearchEntry>();

            //FuzzyQuery query = new FuzzyQuery(new Term("CV", searchQuery));

            //query.setRewriteMethod(FuzzyQuery.SCORING_BOOLEAN_QUERY_REWRITE);
            //ScoreDoc[] hits = searcher.Search(query, null, 1000).ScoreDocs;
            if (mQueryParser != null)
            {
                string managedKeyWords = searchVals.value;
                //string managedKeyWords = txtIndexMange(searchVals.value);

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

                string[] words = managedKeyWords.Split(" ", StringSplitOptions.RemoveEmptyEntries);

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

                    //var sort = new Sort(new SortField("Updated", SortFieldType.INT64,true));

                    ScoreDoc[] hitIdxs = await Task.Run(() => mIndexSearcher.Search(aggregateQuery, null, 10000).ScoreDocs);
                    //ScoreDoc[] hitIdxs = await Task.Run(() => mIndexSearcher.Search(query, null, 5000).ScoreDocs);

                    for ( int i = 0; i < hitIdxs.Length; i++)
                    {
                        var doc = mIndexSearcher.Doc(hitIdxs[i].Doc);

                        result.Add(new SearchEntry
                        {
                            Id = Convert.ToInt32(doc.Get("Id")),
                            UpdatedTs = Convert.ToInt64(doc.Get("Updated")),
                            //Updated = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(doc.Get("Updated"))).UtcDateTime,
                            CV = doc.Get("CV"),
                            Score= (int)Math.Round(hitIdxs[i].Score*100,0)
                        });
                    }

                    //find by id
                    //result.Where(x => x.Id == 348337)


                    if (searchVals.exact)
                    {

                        //string managedKeyWords = txtIndexMange(searchVals.value);
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
                            keyWordsToSearch.Add(phrase);
                        }

                        foreach (var word in keyWordsToSearch)
                        {
                            result = result.Where(x => x.CV.Contains(word)).ToList();
                        }

                        if (matches.Count == 0 && keyWordsToSearch.Count > 1)
                        {
                            var resultMatch = result.Where(x => x.CV.Contains(managedKeyWords));

                            foreach (var item in resultMatch)
                            {
                                item.Score = 1000;
                            }
                        }

                        result = result.OrderByDescending(x => x.Score).ToList();
                    }


                    result = result.OrderByDescending(x => x.UpdatedTs).ToList();





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

        public async Task AddUpdateCandidateDataToIndex(CvsToIndexModel candidateDataToIndex)
        {
            await DocumentDelete(candidateDataToIndex.candidateId);

            try
            {
                using (var indexDir = FSDirectory.Open(new DirectoryInfo(_indexFolder)))
                {
                    var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, mAnalyzer);

                    using (var indexWriter = new IndexWriter(indexDir, config))
                    {
                        await Task.Run(() => indexWriter.AddDocument(CandTextToDocument(candidateDataToIndex)));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ;
            }
        }

        public async Task IndexAllCandidates(int companyId, List<CvsToIndexModel> allCandsTextToIndexList)
        {
            try
            {
                using (var indexDir = FSDirectory.Open(new DirectoryInfo(_indexFolder)))
                {
                    var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, mAnalyzer);

                    using (var indexWriter = new IndexWriter(indexDir, config))
                    {
                        indexWriter.DeleteAll();

                        foreach (var cnd in allCandsTextToIndexList)
                        {
                            await Task.Run(() => indexWriter.AddDocument(CandTextToDocument(cnd)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public async Task CompanyIndexAddDocuments(int companyId, List<CvsToIndexModel> CompanyTextToIndexList, bool isDeleteAllDocuments)
        //{
        //    try
        //    {
        //        using (var indexDir = FSDirectory.Open(new DirectoryInfo(_indexFolder)))
        //        {
        //            var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, mAnalyzer);

        //            using (var indexWriter = new IndexWriter(indexDir, config))
        //            {
        //                if (isDeleteAllDocuments)
        //                {
        //                    indexWriter.DeleteAll();
        //                }

        //                foreach (var item in CompanyTextToIndexList)
        //                {
        //                    await Task.Run(() => indexWriter.AddDocument(documentToIndex(item)));
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        private Document CandTextToDocument(CvsToIndexModel cvCand)
        {
           var plainText =  CleanString.ExtractPlainText(cvCand.cvsTxt ?? "").ToLowerInvariant();

            var doc = new Document() {{ new TextField("Id", cvCand.candidateId.ToString(), Field.Store.YES) },
                { new StoredField("Updated",DateTimeOffset.UtcNow.ToUnixTimeSeconds())},
                {new TextField("CV", plainText, Field.Store.YES) }};

            return doc;
        }


        private Document documentToIndex(CvsToIndexModel cvCand)
        {
            var phoneNumbersOnly = string.Concat(nn(cvCand.phone).Where(char.IsNumber));

            string txtToIndex = $"{nn(cvCand.email)} {phoneNumbersOnly} {nn(cvCand.reviewText)} {nn(cvCand.firstName)} {nn(cvCand.lastName)} {nn(cvCand.cvsTxt)}";
            txtToIndex = txtIndexMange(txtToIndex);

            var updatedDate = cvCand.lastCvSent != null ? ((DateTimeOffset)cvCand.lastCvSent).ToUnixTimeSeconds() : 0;

            var doc = new Document() {{ new TextField("Id", cvCand.candidateId.ToString(), Lucene.Net.Documents.Field.Store.YES) },
                { new StoredField("Updated",updatedDate)},
                {new TextField("CV", txtToIndex, Lucene.Net.Documents.Field.Store.YES) }};

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

            //string[] result = words.Distinct().ToArray();

            var cvWords = string.Join(" ", words);

            return cvWords;
        }

        private string nn(string? str)
        {
            return str == null ? "" : str;
        }

       

        private async Task DocumentDelete( int id)
        {
            using (var indexDir = FSDirectory.Open(new DirectoryInfo(_indexFolder)))
            {
                var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, mAnalyzer);

                using (var indexWriter = new IndexWriter(indexDir, config))
                {
                    var DocIdToDelete = new TermQuery(new Term("Id", id.ToString()));
                    await Task.Run(() => indexWriter.DeleteDocuments(DocIdToDelete));
                }
            }
        }

        public void SearchBoolean(long collectionId, string text)
        {
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
        public long UpdatedTs { get; set; }
        public DateTime Updated { get; set; }
        public int Score { get; set; }
    }
}