namespace LuceneLibrary
{
    public interface ILuceneIndexServiceOld
    {
        Task IndexAllCandidates();
        Task<bool> IndexNewCvFromQueue();
    }
}
