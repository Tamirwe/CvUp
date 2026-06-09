namespace LuceneLibrary
{
    public interface ILuceneIndexService
    {
        Task AddUpdateCandidateDataToIndex(int companyId, int candidateId);
        Task IndexAllCandidates();
    }
}
