using DataModelsLibrary.Models;

namespace OpenAiLibrary.PositionPropsWriter
{
    public interface IOpenAiPositionPropsWriterService
    {
        Task<string?> GenerateRequirementsAsync(string title, string? description);
        Task<string?> GenerateDescriptionAsync(string title, string? requirements);
        Task<string?> GenerateJobAdAsync(string title, string? requirements, string? description);
        Task<PositionContentModel?> GenerateAllAsync(string title, string? requirements, string? description);
    }
}
