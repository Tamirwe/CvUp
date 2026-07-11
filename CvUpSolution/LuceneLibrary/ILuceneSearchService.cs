using DataModelsLibrary.Models;

namespace LuceneLibrary
{
    public interface ILuceneSearchService
    {
        Task<List<SearchEntry>> Search(int companyId, searchCandCvModel searchVals);
        Task<List<SearchEntry>> SearchCandidatesByTerms(List<string> terms, int maxResults = 1000);
        Task<List<SearchEntry>> SearchForAiFilter(searchCandCvModel searchVals);
        Task<List<SearchEntry>> SearchCandidatesByPosition(AnalyzedPositionModel analyzed, int maxResults = 500);
        Task<List<SearchEntry>> SearchWithin(IEnumerable<int> previousResultIds, searchCandCvModel searchVals);
        Task<List<SearchEntry>> ComplexSearch(SearchTermsModel searchTerms);
    }

}
