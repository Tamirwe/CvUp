namespace PgVectorLibrary
{
    public interface IAnalyzeCvsService
    {
        Task AnalyzeCandidatesLastCv();
        Task<bool> AnalyzeCvFromQueue();
    }
}