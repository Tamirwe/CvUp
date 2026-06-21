using AnalyzeEmbedOpenAiLibrary;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;

namespace CandsPositionsLibrary
{
    public class CandsListsServise : ICandsListsServise
    {
        private ICandsListsQueries _candsListsQueries;
        private ICandsPositionsQueries _cvsPositionsQueries;
        private IAnalyzePositionOpenAi _analyzePositionOpenAi;

        public CandsListsServise(ICandsListsQueries candsListsQueries, ICandsPositionsQueries cvsPositionsQueries, IAnalyzePositionOpenAi analyzePositionOpenAi)
        {
            _candsListsQueries = candsListsQueries;
            _cvsPositionsQueries = cvsPositionsQueries;
            _analyzePositionOpenAi = analyzePositionOpenAi;
        }

        public async Task<CandModel?> GetPositionCandidate(int companyId, int candId, int positionId)
        {
            return await _candsListsQueries.GetPositionCandidate(companyId, candId, positionId);
        }

        public async Task<List<CandModel?>> GetCandsList(int companyId, List<int>? candsIds)
        {
            return await _candsListsQueries.GetCandsList(companyId, candsIds);
        }

        public async Task<List<CandModel?>> GetPosCandsList(int companyId, int positionId)
        {
            return await _candsListsQueries.GetPosCandsList(companyId, positionId);
        }

        public async Task<List<CandModel?>> GetPosTypeCandsList(int companyId, int positionTypeId)
        {
            return await _candsListsQueries.GetPosTypeCandsList(companyId, positionTypeId);
        }

        public async Task<List<CandModel?>> GetFolderCandsList(int companyId, int folderId)
        {
            return await _candsListsQueries.GetFolderCandsList(companyId, folderId);
        }

        public async Task<List<CandCvModel>> FindPositionMatchCvs(int companyId, int positionId)
        {
            var position = await _cvsPositionsQueries.GetPosition(companyId, positionId);
            var positionText = string.Join(" ", new[] { position.name, position.descr, position.requirements }
                .Where(s => !string.IsNullOrWhiteSpace(s)));
            var analyzedPosition = await _analyzePositionOpenAi.AiAnalyzePosition(positionText);
            return await _candsListsQueries.FindPositionMatchCvs(companyId, positionId);
        }
    }
}
