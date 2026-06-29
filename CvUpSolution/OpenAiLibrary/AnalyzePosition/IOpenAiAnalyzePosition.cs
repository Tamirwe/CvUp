using DataModelsLibrary.Models;

namespace OpenAiLibrary.AnalyzePosition
{
    public interface IOpenAiAnalyzePosition
    {
        Task<AnalyzedPositionModel?> AiAnalyzePosition(string positionText);
    }
}
