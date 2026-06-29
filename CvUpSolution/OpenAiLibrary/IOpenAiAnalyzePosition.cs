using DataModelsLibrary.Models;

namespace OpenAiLibrary
{
    public interface IOpenAiAnalyzePosition
    {
        Task<AnalyzedPositionModel?> AiAnalyzePosition(string positionText);
    }
}
