namespace AiLibrary.AnalyzeCvs
{
    public interface IAnalyzeCvsService
    {
        Task AnalyzeCandidates(int candidateId = 0);
        Task EmbedAnalyzeCvs(int candidateId = 0);
    }
}
