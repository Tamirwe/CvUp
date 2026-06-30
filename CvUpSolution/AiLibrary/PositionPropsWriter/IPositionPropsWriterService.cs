using DataModelsLibrary.Models;

namespace AiLibrary.PositionPropsWriter
{
    public interface IPositionPropsWriterService
    {
        Task<PositionContentModel?> PositionRewriteDescrRequirements(PositionModel position);
        Task<string?> PositionAdWriterAsync(PositionModel position);
    }
}
