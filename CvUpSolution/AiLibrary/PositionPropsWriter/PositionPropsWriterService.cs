using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using OpenAiLibrary.PositionPropsWriter;

namespace AiLibrary.PositionPropsWriter
{
    public class PositionPropsWriterService : IPositionPropsWriterService
    {
        private readonly IOpenAiPositionPropsWriterService _positionWriterOpenAi;
        private readonly IPositionsQueries _positionsQueries;

        public PositionPropsWriterService(IOpenAiPositionPropsWriterService positionWriterOpenAi, IPositionsQueries positionsQueries)
        {
            _positionWriterOpenAi = positionWriterOpenAi;
            _positionsQueries = positionsQueries;
        }

        public async Task<PositionContentModel?> PositionPropsRewriteAsync(PositionModel position)
        {
            return await _positionWriterOpenAi.GenerateAllAsync(position.name, position.requirements, position.descr);
        }

        public async Task<PositionContentModel?> PositionPropsRewriteByIdAsync(int positionId, int companyId = 154)
        {
            var position = await _positionsQueries.GetPosition(positionId, companyId);
            return await PositionPropsRewriteAsync(position);
        }
    }
}
