using DataModelsLibrary.Models;

namespace OpenAiLibrary.AnalyzeCv
{
    public interface IOpenAiAnalyzeCvService
    {
        Task<AnalyzedCvModel?> AiAnalyzeCv(int candId, int cvId, string? cvText);
    }
}
