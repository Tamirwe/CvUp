using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using OpenAiLibrary.PositionPropsWriter;

namespace PgVectorLibrary.PositionPropsWriter
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

        public async Task<PositionContentModel?> PositionPostingAsync(string title, string? requirements = null, string? description = null)
        {
            return await _positionWriterOpenAi.GenerateAllAsync(title, requirements, description);
        }

        public async Task<PositionContentModel?> PositionPostingByIdAsync(int positionId, int companyId = 154)
        {
            var position = await _positionsQueries.GetPosition(positionId, companyId);
            return await PositionPostingAsync(position.name, position.requirements, position.descr);
        }
    }
}
