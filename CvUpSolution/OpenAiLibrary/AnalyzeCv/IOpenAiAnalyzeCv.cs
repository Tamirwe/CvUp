using DataModelsLibrary.Models;

namespace OpenAiLibrary.AnalyzeCv
{
    public interface IOpenAiAnalyzeCv
    {
        Task<AnalyzedCvModel?> AiAnalyzeCv(int candId, int cvId, string? cvText);
    }
}
