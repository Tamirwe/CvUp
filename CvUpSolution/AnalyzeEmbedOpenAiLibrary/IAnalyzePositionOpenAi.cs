using DataModelsLibrary.Models;

namespace AnalyzeEmbedOpenAiLibrary
{
    public interface IAnalyzePositionOpenAi
    {
        Task<AnalyzedPositionModel?> AiAnalyzePosition(string positionText);
    }
}
