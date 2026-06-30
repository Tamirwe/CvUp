namespace PgVectorLibrary.AnalyzePositions
{
    public interface IAnalyzePositionsService
    {
        Task AnalyzePosition(int positionId, int companyId = 154);
        Task<bool> AnalyzePositionFromQueue();
    }
}
