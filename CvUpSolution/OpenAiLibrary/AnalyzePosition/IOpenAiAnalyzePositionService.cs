using DataModelsLibrary.Models;

namespace OpenAiLibrary.AnalyzePosition
{
    public interface IOpenAiAnalyzePositionService
    {
        Task<AnalyzedPositionModel?> AiAnalyzePosition(string positionText);
    }
}
