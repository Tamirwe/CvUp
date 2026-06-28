using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using GeneralLibrary;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Extensions.Configuration;
using QueueLibrary;

namespace LuceneLibrary
{
    public class LuceneIndexService : ILuceneIndexService
    {
        private const LuceneVersion LUCENE_VERSION = LuceneVersion.LUCENE_48;

        private const string F_CANDIDATE_ID = "Id";
        private const string F_CV_ID = "CvId";
        private const string F_FULL_NAME = "Name";
        private const string F_EMAIL = "Email";
        private const string F_CV_TEXT = "CV";
        private const string F_REVIEW = "Review";
        private const string F_AI_SUMMARY = "AiSummary";
        private const string F_AI_WORK = "AiWork";
        private const string F_AI_EDUCATION = "AiEducation";
        private const string F_AI_SKILLS = "AiSkills";

        private readonly string _indexFolder;
        private readonly PerFieldAnalyzerWrapper _analyzer;
        private readonly ILuceneQueries _luceneQueries;
        private readonly IDbQueueService _queueService;
        private readonly int _companyId;

        public LuceneIndexService(IConfiguration configuration, ILuceneQueries luceneQueries, IDbQueueService queueService, int companyId = 154)
        {
            var root = configuration["APP_LOCAL_ROOT_FOLDER"];
            _companyId = companyId;
            _indexFolder = $"{root}\\_{companyId}\\luceneIndex";
            _analyzer = BuildAnalyzer();
            _luceneQueries = luceneQueries;
            _queueService = queueService;
        }

        // ─────────────────────────────────────────────
        // Analyzer
        // ─────────────────────────────────────────────

        private static PerFieldAnalyzerWrapper BuildAnalyzer()
        {
            var hebrewAnalyzer = new SimpleAnalyzer(LUCENE_VERSION);
            var standardAnalyzer = new StandardAnalyzer(LUCENE_VERSION);
            var whitespaceAnalyzer = new WhitespaceAnalyzer(LUCENE_VERSION);

            var fieldAnalyzers = new Dictionary<string, Analyzer>
            {
                { F_CV_TEXT,      hebrewAnalyzer },
                { F_REVIEW,       hebrewAnalyzer },
                { F_AI_SUMMARY,   hebrewAnalyzer },
                { F_AI_WORK,      hebrewAnalyzer },
                { F_AI_EDUCATION, hebrewAnalyzer },
                { F_AI_SKILLS,    hebrewAnalyzer },
                { F_FULL_NAME,    whitespaceAnalyzer },
                { F_EMAIL,        whitespaceAnalyzer },
            };

            return new PerFieldAnalyzerWrapper(standardAnalyzer, fieldAnalyzers);
        }

        // ─────────────────────────────────────────────
        // Public: index all candidates
        // ─────────────────────────────────────────────

        public async Task IndexAllCandidates()
        {
            List<CandLastCvModel> allCands = await _luceneQueries.AllCandidatesLastCv();
            Console.WriteLine($"Rows count: {allCands.Count}");

            Console.WriteLine("Indexing candidates...");
            using var indexDir = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            var config = new IndexWriterConfig(LUCENE_VERSION, _analyzer);
            using var indexWriter = new IndexWriter(indexDir, config);

            Console.WriteLine("Deleting all old documents...");
            indexWriter.DeleteAll();

            Console.WriteLine("Adding new documents...");
            foreach (var cand in allCands)
                indexWriter.AddDocument(CandToDocument(cand));
        }

        // ─────────────────────────────────────────────
        // Public: index single candidate by id
        // ─────────────────────────────────────────────

        public async Task IndexCandidate(int candidateId)
        {
            CandLastCvModel? cand = await _luceneQueries.CandidateLastCv(candidateId);

            if (cand == null) return;

            await DeleteDocument(cand.candidateId);

            using var indexDir = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            var config = new IndexWriterConfig(LUCENE_VERSION, _analyzer);
            using var indexWriter = new IndexWriter(indexDir, config);
            indexWriter.AddDocument(CandToDocument(cand));
        }

        private async Task DeleteDocument(int candidateId)
        {
            using var indexDir = FSDirectory.Open(new DirectoryInfo(_indexFolder));
            var config = new IndexWriterConfig(LUCENE_VERSION, _analyzer);
            using var indexWriter = new IndexWriter(indexDir, config);
            await Task.Run(() => indexWriter.DeleteDocuments(new Term(F_CANDIDATE_ID, candidateId.ToString())));
        }

        // ─────────────────────────────────────────────
        // Document builder
        // ─────────────────────────────────────────────

        private Document CandToDocument(CandLastCvModel cand)
        {
            var doc = new Document();

            // Identity
            doc.Add(new TextField(F_CANDIDATE_ID, cand.candidateId.ToString(), Field.Store.YES));
            doc.Add(new TextField(F_CV_ID, cand.cvId.ToString(), Field.Store.YES));

            // Full name
            var fullName = System.Text.RegularExpressions.Regex.Replace(
                $"{cand.firstName} {cand.lastName}".Trim(), @"[-\p{P}]", " ").Trim();
            if (!string.IsNullOrWhiteSpace(fullName))
                doc.Add(new TextField(F_FULL_NAME, fullName.ToLowerInvariant(), Field.Store.YES));

            // Email
            if (!string.IsNullOrWhiteSpace(cand.email))
                doc.Add(new TextField(F_EMAIL, cand.email.ToLowerInvariant(), Field.Store.YES));

            // Raw CV text
            var cvText = StringMethods.ExtractPlainText(cand.cvTxt ?? "");
            if (!string.IsNullOrWhiteSpace(cvText))
                doc.Add(new TextField(F_CV_TEXT, cvText.ToLowerInvariant(), Field.Store.NO));

            // Review
            if (!string.IsNullOrWhiteSpace(cand.reviewText))
                doc.Add(new TextField(F_REVIEW, cand.reviewText.ToLowerInvariant(), Field.Store.YES));

            // AI: summary (he + en combined)
            var summary = CombineText(cand.summaryHe, cand.summaryEn);
            if (summary != null)
                doc.Add(new TextField(F_AI_SUMMARY, summary, Field.Store.NO));

            // AI: work experience (company + title_he + title_en)
            var work = FlattenWorkExperience(cand.workExperienceItems);
            if (work != null)
                doc.Add(new TextField(F_AI_WORK, work, Field.Store.NO));

            // AI: education (degree + field_he + field_en)
            var education = FlattenEducation(cand.educationItems);
            if (education != null)
                doc.Add(new TextField(F_AI_EDUCATION, education, Field.Store.NO));

            // AI: skills + profession words (he + en)
            var skills = CombineText(
                 cand.skills != null ? string.Join(" ", cand.skills) : null,
                 FlattenProfessionWords(cand.professionWordsItems)
             );
            if (skills != null)
                doc.Add(new TextField(F_AI_SKILLS, skills, Field.Store.NO));

            return doc;
        }

        // ─────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────

        private static string? CombineText(params object?[] parts)
        {
            var tokens = new List<string>();

            foreach (var part in parts)
            {
                switch (part)
                {
                    case string s when !string.IsNullOrWhiteSpace(s):
                        tokens.Add(s);
                        break;
                    case string[] arr:
                        tokens.AddRange(arr.Where(s => !string.IsNullOrWhiteSpace(s)));
                        break;
                }
            }

            return tokens.Count == 0 ? null : string.Join(" ", tokens);
        }

        private static string? FlattenWorkExperience(WorkExperienceItemModel[]? items) =>
            items == null || items.Length == 0 ? null :
            string.Join(" ", items.Select(w =>
                CombineText(w.company, w.title_he, w.title_en)));

        private static string? FlattenEducation(EducationItemModel[]? items) =>
            items == null || items.Length == 0 ? null :
            string.Join(" ", items.Select(e =>
                CombineText(e.degree, e.field_he, e.field_en)));

        private static string? FlattenProfessionWords(ProfessionWordModel[]? items) =>
            items == null || items.Length == 0 ? null :
            string.Join(" ", items.Select(p =>
                CombineText(p.hebrew, p.english)));
    }
}