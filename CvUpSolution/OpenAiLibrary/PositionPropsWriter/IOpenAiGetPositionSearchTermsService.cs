using DataModelsLibrary.Models;

namespace OpenAiLibrary.PositionPropsWriter
{
    public interface IOpenAiGetPositionSearchTermsService
    {
        Task<PositionSearchTermsModel?> GetPositionSearchTerms(string title, string descr, string requirements);
    }
}
