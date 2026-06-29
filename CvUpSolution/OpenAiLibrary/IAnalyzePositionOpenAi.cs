using DataModelsLibrary.Models;

namespace OpenAiLibrary
{
    public interface IAnalyzePositionOpenAi
    {
        Task<AnalyzedPositionModel?> AiAnalyzePosition(string positionText);
    }
}
