using DataModelsLibrary.Models;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using LuceneLibrary;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Directory = System.IO.Directory;

/// <summary>
/// Integration tests for <see cref="LuceneSearchService"/>.
/// <para>
/// Each test run creates a temporary Lucene index under <c>Path.GetTempPath()</c>,
/// seeds it with four synthetic candidates, executes searches, and deletes the
/// index on teardown — no external dependencies or configuration required.
/// </para>
/// <para>
/// Candidates seeded:
/// <list type="bullet">
///   <item><description>1 — Alice Cohen   | skills: csharp, dotnet      | work: fintech, banking</description></item>
///   <item><description>2 — Bob Levi      | skills: csharp, dotnet      | work: ecommerce, retail</description></item>
///   <item><description>3 — Carol Mizrahi | skills: java, python        | work: fintech, insurance</description></item>
///   <item><description>4 — Dan Shapira   | skills: photoshop, illustrator | work: marketing, design</description></item>
/// </list>
/// </para>
/// <para>
/// Tests covered:
/// <list type="bullet">
///   <item><description><see cref="SearchWithin_NarrowsResultsToIdSet"/> — core refinement behavior</description></item>
///   <item><description><see cref="SearchWithin_ExcludesCandidatesOutsideIdSet"/> — filter boundary</description></item>
///   <item><description><see cref="SearchWithin_ReturnsEmpty_WhenNoMatchInIdSet"/> — no match in restricted set</description></item>
///   <item><description><see cref="SearchWithin_ReturnsEmpty_WhenIdSetIsEmpty"/> — empty input guard</description></item>
///   <item><description><see cref="SearchWithin_CanChain_ThreeLevels"/> — chained search-within-search</description></item>
/// </list>
/// </para>
/// </summary>
public class LuceneSearchServiceTests : IDisposable
{
    private const LuceneVersion LUCENE_VERSION = LuceneVersion.LUCENE_48;
    private readonly string _tempIndexPath;
    private readonly LuceneSearchService _service;

    public LuceneSearchServiceTests()
    {
        _tempIndexPath = Path.Combine(Path.GetTempPath(), $"lucene_test_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempIndexPath);

        // Mock IConfiguration to point at our temp index
        var config = new Mock<IConfiguration>();
        config.Setup(c => c["APP_LOCAL_ROOT_FOLDER"]).Returns(_tempIndexPath);

        // Use companyId=0 so path becomes _tempIndexPath\_0\luceneIndex
        _service = new LuceneSearchService(config.Object, companyId: 0);

        SeedIndex();
    }

    private void SeedIndex()
    {
        var indexPath = Path.Combine(_tempIndexPath, "_0", "luceneIndex");
        Directory.CreateDirectory(indexPath);

        using var dir = FSDirectory.Open(new DirectoryInfo(indexPath));
        var config = new IndexWriterConfig(LUCENE_VERSION, new WhitespaceAnalyzer(LUCENE_VERSION))
        {
            OpenMode = OpenMode.CREATE
        };
        using var writer = new IndexWriter(dir, config);

        // Helper to add a candidate doc
        void AddDoc(int id, string name, string skills, string work)
        {
            var doc = new Document
            {
                new StringField("Id",   id.ToString(), Field.Store.YES),
                new TextField  ("Name", name,          Field.Store.YES),
                new TextField  ("AiSkills", skills,    Field.Store.YES),
                new TextField  ("AiWork",   work,      Field.Store.YES),
                // fill remaining ContentFields as empty so no null issues
                new TextField("CV",          "", Field.Store.NO),
                new TextField("Review",      "", Field.Store.NO),
                new TextField("AiSummary",   "", Field.Store.NO),
                new TextField("AiEducation", "", Field.Store.NO),
            };
            writer.AddDocument(doc);
        }

        // Candidate 1 — C# developer in fintech
        AddDoc(1, "Alice Cohen", "csharp dotnet", "fintech banking");
        // Candidate 2 — C# developer, NOT fintech
        AddDoc(2, "Bob Levi", "csharp dotnet", "ecommerce retail");
        // Candidate 3 — fintech but NOT C#
        AddDoc(3, "Carol Mizrahi", "java python", "fintech insurance");
        // Candidate 4 — unrelated
        AddDoc(4, "Dan Shapira", "photoshop illustrator", "marketing design");

        writer.Commit();
    }

    // ── Tests ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task SearchWithin_NarrowsResultsToIdSet()
    {
        // First search: find C# developers → should return candidates 1 and 2
        var firstSearch = new searchCandCvModel { value = "csharp", exact = false };
        var firstResults = await _service.Search(0, firstSearch);

        Assert.Contains(firstResults, r => r.Id == 1);
        Assert.Contains(firstResults, r => r.Id == 2);

        // Search within those results for "fintech" → only candidate 1 should survive
        var refineSearch = new searchCandCvModel { value = "fintech", exact = false };
        var refined = await _service.SearchWithin(firstResults.Select(r => r.Id), refineSearch);

        Assert.Single(refined);
        Assert.Equal(1, refined[0].Id);
    }

    [Fact]
    public async Task SearchWithin_ExcludesCandidatesOutsideIdSet()
    {
        // Candidate 3 matches "fintech" but is NOT in the first result set
        var firstResultIds = new[] { 1, 2 }; // only C# devs

        var refineSearch = new searchCandCvModel { value = "fintech", exact = false };
        var refined = await _service.SearchWithin(firstResultIds, refineSearch);

        Assert.DoesNotContain(refined, r => r.Id == 3);
    }

    [Fact]
    public async Task SearchWithin_ReturnsEmpty_WhenNoMatchInIdSet()
    {
        var firstResultIds = new[] { 4 }; // only the unrelated candidate

        var refineSearch = new searchCandCvModel { value = "csharp", exact = false };
        var refined = await _service.SearchWithin(firstResultIds, refineSearch);

        Assert.Empty(refined);
    }

    [Fact]
    public async Task SearchWithin_ReturnsEmpty_WhenIdSetIsEmpty()
    {
        var refineSearch = new searchCandCvModel { value = "csharp", exact = false };
        var refined = await _service.SearchWithin(Array.Empty<int>(), refineSearch);

        Assert.Empty(refined);
    }

    [Fact]
    public async Task SearchWithin_CanChain_ThreeLevels()
    {
        // Level 1: csharp → {1, 2}
        var l1 = await _service.Search(0, new searchCandCvModel { value = "csharp", exact = false });

        // Level 2: within {1,2} search fintech → {1}
        var l2 = await _service.SearchWithin(
            l1.Select(r => r.Id),
            new searchCandCvModel { value = "fintech", exact = false });

        // Level 3: within {1} search banking → still {1}
        var l3 = await _service.SearchWithin(
            l2.Select(r => r.Id),
            new searchCandCvModel { value = "banking", exact = false });

        Assert.Single(l3);
        Assert.Equal(1, l3[0].Id);
    }

    // ── ComplexSearch Tests ──────────────────────────────────────────────────────

    [Fact]
    public async Task ComplexSearch_MustTerms_ReturnsOnlyMatchingBoth()
    {
        // Must: csharp AND fintech → only candidate 1 has both
        var results = await _service.ComplexSearch(new SearchTermsModel
        {
            MustHave = ["csharp", "fintech"],
        });

        Assert.Single(results);
        Assert.Equal(1, results[0].Id);
    }

    [Fact]
    public async Task ComplexSearch_ShouldTerm_BoostsButDoesNotExclude()
    {
        // Must: csharp | Should: fintech → both 1 and 2 return, but 1 scores higher
        var results = await _service.ComplexSearch(new SearchTermsModel
        {
            MustHave = ["csharp"],
            ShouldHave = ["fintech"],
        });

        Assert.Contains(results, r => r.Id == 1);
        Assert.Contains(results, r => r.Id == 2);

        var score1 = results.First(r => r.Id == 1).Score;
        var score2 = results.First(r => r.Id == 2).Score;
        Assert.True(score1 > score2, "Candidate with fintech should score higher");
    }

    [Fact]
    public async Task ComplexSearch_SearchWithin_NarrowsCorrectly()
    {
        // First: csharp → {1, 2}
        // Within: fintech → {1}
        var results = await _service.ComplexSearch(new SearchTermsModel
        {
            MustHave = ["csharp"],
            MustHaveInResult = ["fintech"],
        });

        Assert.Single(results);
        Assert.Equal(1, results[0].Id);
    }

    [Fact]
    public async Task ComplexSearch_SearchWithin_ReturnsEmpty_WhenNoOverlap()
    {
        // First: csharp → {1, 2}
        // Within: java → {} (candidates 1 and 2 have no java)
        var results = await _service.ComplexSearch(new SearchTermsModel
        {
            MustHave = ["csharp"],
            MustHaveInResult = ["java"],
        });

        Assert.Empty(results);
    }

    [Fact]
    public async Task ComplexSearch_ExactPhrase_MatchesOnlyWhenWordsAdjacent()
    {
        // "fintech banking" as exact phrase → only candidate 1 has both words adjacent
        // Candidate 3 has "fintech insurance" — different second word, should not match
        var results = await _service.ComplexSearch(new SearchTermsModel
        {
            MustHave = ["fintech banking"],
        });

        Assert.Single(results);
        Assert.Equal(1, results[0].Id);
    }

    [Fact]
    public async Task ComplexSearch_ReturnsEmpty_WhenFirstSearchIsEmpty()
    {
        var results = await _service.ComplexSearch(new SearchTermsModel());
        Assert.Empty(results);
    }

    [Fact]
    public async Task ComplexSearch_SearchWithin_Ignored_WhenFirstSearchReturnsEmpty()
    {
        // First search matches nothing, searchWithin should never even run
        var results = await _service.ComplexSearch(new SearchTermsModel
        {
            MustHave = ["cobol"],
            MustHaveInResult = ["csharp"],
        });

        Assert.Empty(results);
    }

    // ── Cleanup ──────────────────────────────────────────────────────────────

    public void Dispose()
    {
        _service.Dispose();
        if (Directory.Exists(_tempIndexPath))
            Directory.Delete(_tempIndexPath, recursive: true);
    }
}