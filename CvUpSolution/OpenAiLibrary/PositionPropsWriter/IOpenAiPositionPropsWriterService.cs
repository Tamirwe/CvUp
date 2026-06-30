using DataModelsLibrary.Models;

namespace OpenAiLibrary.PositionPropsWriter
{
    public interface IOpenAiPositionPropsWriterService
    {
        Task<PositionContentModel?> OpenAiRewritePositionProps(string title, string? requirements, string? description);
        Task<string?> OpenAiPositionAdWriter(string title, string? requirements, string? description);
    }
}
