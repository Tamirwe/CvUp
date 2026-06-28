namespace PgVectorLibrary
{
    public interface IAnalyzeCvsService
    {
        Task AnalyzeCandidates(int candidateId = 0);
        Task<bool> AnalyzeCvFromQueue();
    }
}