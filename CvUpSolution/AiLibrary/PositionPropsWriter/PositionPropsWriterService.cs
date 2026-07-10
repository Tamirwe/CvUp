using DataModelsLibrary.Models;
using OpenAiLibrary.PositionPropsWriter;

namespace AiLibrary.PositionPropsWriter
{
    public class PositionPropsWriterService : IPositionPropsWriterService
    {
        private readonly IOpenAiPositionPropsWriterService _positionWriterOpenAi;
        private readonly IOpenAiGetPositionSearchTermsService _positionSearchTermsOpenAi;

        public PositionPropsWriterService(IOpenAiPositionPropsWriterService positionWriterOpenAi, IOpenAiGetPositionSearchTermsService positionSearchTermsOpenAi)
        {
            _positionWriterOpenAi = positionWriterOpenAi;
            _positionSearchTermsOpenAi = positionSearchTermsOpenAi;
        }

        public Task<PositionContentModel?> PositionRewriteDescrRequirements(PositionModel position) =>
            _positionWriterOpenAi.OpenAiRewritePositionDescrRequirements(position.name, position.requirements, position.descr);

        public Task<string?> PositionAdWriterAsync(PositionModel position) =>
            _positionWriterOpenAi.OpenAiPositionAdWriter(position.name, position.requirements, position.descr);

        public Task<PositionSearchTermsModel?> GetAnalyzedPositionSearchTerms(string title, string descr, string requirements) =>
            _positionSearchTermsOpenAi.GetAnalyzedPositionSearchTerms(title, descr, requirements);

    }
}
