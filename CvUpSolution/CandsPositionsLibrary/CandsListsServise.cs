using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using LuceneLibrary;
using PgVectorLibrary;
using PgVectorLibrary.AnalyzePositions;
using PgVectorLibrary.SearchCvs;

namespace CandsPositionsLibrary
{
    public class CandsListsServise : ICandsListsServise
    {
        private ICandsListsQueries _candsListsQueries;
        private IPositionsQueries _cvsPositionsQueries;
        private ILuceneSearchService _luceneSearchService;
        private ISearchCvsService _searchCvsService;
        private IAnalyzePositionsService _analyzePositionsService;

        public CandsListsServise(ICandsListsQueries candsListsQueries, IPositionsQueries cvsPositionsQueries, ILuceneSearchService luceneSearchService, ISearchCvsService searchCvsService, IAnalyzePositionsService analyzePositionsService)
        {
            _candsListsQueries = candsListsQueries;
            _cvsPositionsQueries = cvsPositionsQueries;
            _luceneSearchService = luceneSearchService;
            _searchCvsService = searchCvsService;
            _analyzePositionsService = analyzePositionsService;
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

        public async Task<List<AiCandidateSearchModel>> FindPositionMatchCvs(int positionId)
        {
            var analyzed = await _cvsPositionsQueries.GetAnalyzedPosition(positionId);

            if (analyzed == null)
            {
                var companyId = await _cvsPositionsQueries.GetPositionCompanyId(positionId);
                await _analyzePositionsService.AnalyzePosition(positionId, companyId);
                analyzed = await _cvsPositionsQueries.GetAnalyzedPosition(positionId);
            }

            if (analyzed == null) return [];

            var luceneResults = await _luceneSearchService.SearchCandidatesByPosition(analyzed, maxResults: 500);
            var luceneCandidateIds = luceneResults.Select(r => r.Id).ToList();

            return await _searchCvsService.SearchCvsByPositionFiltered(positionId, luceneCandidateIds, limit: 100);
        }

        public async Task<List<SearchEntry>> GetLuceneCandidatesForPosition(int positionId)
        {
            var analyzed = await _cvsPositionsQueries.GetAnalyzedPosition(positionId);
            if (analyzed == null) return [];

            return await _luceneSearchService.SearchCandidatesByPosition(analyzed, maxResults: 500);
        }
    }
}
