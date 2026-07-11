using DataModelsLibrary.Models;

namespace AiLibrary.SearchCvs
{
    public interface ISearchCvsService
    {
        Task<List<AiCandidateSearchModel>> SearchCvs(string AiSearchPhrase, List<int>? candidateIds = null, int limit = 20);
    }
}
