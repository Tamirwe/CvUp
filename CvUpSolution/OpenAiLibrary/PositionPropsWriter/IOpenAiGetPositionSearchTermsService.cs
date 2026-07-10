using DataModelsLibrary.Models;

namespace OpenAiLibrary.PositionPropsWriter
{
    public interface IOpenAiGetPositionSearchTermsService
    {
        Task<PositionSearchTermsModel?> GetAnalyzedPositionSearchTerms(string title, string descr, string requirements);
    }
}
