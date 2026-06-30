using DataModelsLibrary.Models;

namespace PgVectorLibrary.PositionPropsWriter
{
    public interface IPositionPropsWriterService
    {
        Task<PositionContentModel?> PositionPostingAsync(string title, string? requirements = null, string? description = null);
        Task<PositionContentModel?> PositionPostingByIdAsync(int positionId, int companyId = 154);
    }
}
