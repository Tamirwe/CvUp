using DataModelsLibrary.Models;
using OpenAiLibrary.PositionPropsWriter;

namespace AiLibrary.PositionPropsWriter
{
    public class PositionPropsWriterService : IPositionPropsWriterService
    {
        private readonly IOpenAiPositionPropsWriterService _positionWriterOpenAi;

        public PositionPropsWriterService(IOpenAiPositionPropsWriterService positionWriterOpenAi)
        {
            _positionWriterOpenAi = positionWriterOpenAi;
        }

        public Task<PositionContentModel?> PositionRewriteDescrRequirements(PositionModel position) =>
            _positionWriterOpenAi.OpenAiRewritePositionDescrRequirements(position.name, position.requirements, position.descr);

        public Task<string?> PositionAdWriterAsync(PositionModel position) =>
            _positionWriterOpenAi.OpenAiPositionAdWriter(position.name, position.requirements, position.descr);

    }
}
