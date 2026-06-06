using DataModelsLibrary.Models;

namespace LuceneLibrary
{
    public interface ILuceneSearchService
    {
        Task<List<SearchEntry>> Search(int companyId, searchCandCvModel searchVals);
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
