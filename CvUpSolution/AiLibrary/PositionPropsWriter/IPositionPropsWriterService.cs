using DataModelsLibrary.Models;

namespace AiLibrary.PositionPropsWriter
{
    public interface IPositionPropsWriterService
    {
        Task<PositionContentModel?> PositionPropsRewriteAsync(PositionModel position);
        Task<PositionContentModel?> PositionPropsRewriteByIdAsync(int positionId, int companyId = 154);
    }
}
