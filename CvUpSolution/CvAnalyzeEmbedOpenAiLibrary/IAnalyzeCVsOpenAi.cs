

using CvAnalyzeEmbedOpenAiLibrary.Models;

namespace CvAnalyzeEmbedOpenAiLibrary
{
    internal interface IAnalyzeCVOpenAi
    {
        Task<AnalyzedCvModel?> AiAnalyzeCv(int candId, int cvId, string? cvText);
    }
}
