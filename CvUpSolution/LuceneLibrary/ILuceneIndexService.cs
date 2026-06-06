using DataModelsLibrary.Models;

namespace LuceneLibrary
{
    public interface ILuceneIndexService
    {
        Task AddUpdateCandidateDataToIndex(CvsToIndexModel candidateDataToIndex);
        Task IndexAllCandidates(int companyId, List<CvsToIndexModel> allCandsTextToIndexList);
    }
}
