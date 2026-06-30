using DataModelsLibrary.Models;

namespace AiLibrary.PositionPropsWriter
{
    public interface IPositionPropsWriterService
    {
        Task<PositionContentModel?> PositionPropsRewriteAsync(PositionModel position);
        Task<string?> PositionAdWriterAsync(PositionModel position);
    }
}
