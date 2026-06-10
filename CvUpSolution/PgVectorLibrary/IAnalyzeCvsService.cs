namespace PgVectorLibrary
{
    public interface IAnalyzeCvsService
    {
        Task AnalyzeCvsBatch();
        Task<bool> AnalyzeCvFromQueue();
        Task AnalyzeCandidate(int candidateId);
    }
}