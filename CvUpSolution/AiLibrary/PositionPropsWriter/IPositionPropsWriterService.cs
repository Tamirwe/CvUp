using DataModelsLibrary.Models;

namespace AiLibrary.PositionPropsWriter
{
    public interface IPositionPropsWriterService
    {
        Task<PositionContentModel?> PositionPropsRewriteAsync(string title, string? requirements = null, string? description = null);
        Task<PositionContentModel?> PositionPropsRewriteByIdAsync(int positionId, int companyId = 154);
    }
}
