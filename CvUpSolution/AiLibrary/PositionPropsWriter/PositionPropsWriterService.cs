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

        public Task<string?> PositionPropsRewriteAsync(PositionModel position, PositionPropsRewriteType rewriteType) =>
            _positionWriterOpenAi.OpenAiRewritePositionProps(position.name, position.requirements, position.descr, rewriteType);

    }
}
