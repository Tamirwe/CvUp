using DataModelsLibrary.Models;
using OpenAiLibrary.PositionPropsWriter;

namespace AiLibrary.PositionPropsWriter
{
    public interface IPositionPropsWriterService
    {
        Task<string?> PositionPropsRewriteAsync(PositionModel position, PositionPropsRewriteType rewriteType);
    }
}
