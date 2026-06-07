using DataModelsLibrary.Models;

namespace PgVectorLibrary
{
    public interface ISearchCvsService
    {
        Task<List<AiCandidateSearchModel>> SearchCvs(string query, int limit = 20);
    }
}