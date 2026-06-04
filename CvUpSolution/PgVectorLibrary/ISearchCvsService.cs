using DataModelsLibrary.Models;

namespace PgVectorLibrary
{
    public interface ISearchCvsService
    {
        Task<List<CandidateSearchResultModel>> SearchCvs(string query, int limit = 20);
    }
}