namespace LuceneLibrary
{
    public interface ILuceneIndexService
    {
        Task IndexAllCandidates();
        Task IndexCandidate(int candidateId);
    }
}
