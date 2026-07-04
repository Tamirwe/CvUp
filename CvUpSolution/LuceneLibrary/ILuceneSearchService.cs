using DataModelsLibrary.Models;

namespace LuceneLibrary
{
    public interface ILuceneSearchService
    {
        Task<List<SearchEntry>> Search(int companyId, searchCandCvModel searchVals);
        Task<List<SearchEntry>> SearchCandidatesByPosition(AnalyzedPositionModel analyzed, int maxResults = 500);
        Task<List<SearchEntry>> SearchWithin(IEnumerable<int> previousResultIds, searchCandCvModel searchVals);
        Task<List<SearchEntry>> ComplexSearch(List<ComplexSearchTerm> firstSearch, List<ComplexSearchTerm>? searchWithin = null);
    }

    public class SearchEntry
    {
        public int Id { get; set; }
        public string CV { get; set; } = string.Empty;
        public long UpdatedTs { get; set; }
        public DateTime Updated { get; set; }
        public int Score { get; set; }
    }

    
}
