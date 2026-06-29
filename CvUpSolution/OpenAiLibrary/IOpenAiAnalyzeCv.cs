

using DataModelsLibrary.Models;

namespace OpenAiLibrary
{
    public interface IOpenAiAnalyzeCv
    {
        Task<AnalyzedCvModel?> AiAnalyzeCv(int candId, int cvId, string? cvText);
    }
}
