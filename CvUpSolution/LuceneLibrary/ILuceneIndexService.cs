namespace LuceneLibrary
{
    public interface ILuceneIndexService
    {
        Task IndexAllCandidates();
        Task<bool> IndexNewCvFromQueue();
    }
}
