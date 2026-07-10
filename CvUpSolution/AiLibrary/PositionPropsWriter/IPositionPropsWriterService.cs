using DataModelsLibrary.Models;

namespace AiLibrary.PositionPropsWriter
{
    public interface IPositionPropsWriterService
    {
        Task<PositionContentModel?> PositionRewriteDescrRequirements(PositionModel position);
        Task<string?> PositionAdWriterAsync(PositionModel position);
        Task<PositionSearchTermsModel?> GetAnalyzedPositionSearchTerms(string title, string descr, string requirements);
    }
}
