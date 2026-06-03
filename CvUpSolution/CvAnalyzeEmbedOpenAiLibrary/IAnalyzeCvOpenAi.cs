

using DataModelsLibrary.Models;

namespace CvAnalyzeEmbedOpenAiLibrary
{
    public interface IAnalyzeCvOpenAi
    {
        Task<AnalyzedCvModel?> AiAnalyzeCv(int candId, int cvId, string? cvText);
    }
}
