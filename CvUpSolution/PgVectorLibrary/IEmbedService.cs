namespace PgVectorLibrary
{
    public interface IEmbedService
    {
        Task EmbedAnalyzeCvs(int candidateId = 0);
    }
}