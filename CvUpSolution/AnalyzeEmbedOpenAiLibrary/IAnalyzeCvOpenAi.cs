

using DataModelsLibrary.Models;

namespace AnalyzeEmbedOpenAiLibrary
{
    public interface IAnalyzeCvOpenAi
    {
        Task<AnalyzedCvModel?> AiAnalyzeCv(int candId, int cvId, string? cvText);
    }
}
