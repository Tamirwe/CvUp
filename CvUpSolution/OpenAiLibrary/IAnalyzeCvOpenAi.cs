

using DataModelsLibrary.Models;

namespace OpenAiLibrary
{
    public interface IAnalyzeCvOpenAi
    {
        Task<AnalyzedCvModel?> AiAnalyzeCv(int candId, int cvId, string? cvText);
    }
}
