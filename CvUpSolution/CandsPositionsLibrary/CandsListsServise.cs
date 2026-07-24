using AiLibrary;
using AiLibrary.PositionPropsWriter;
using AiLibrary.SearchCvs;
using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using LuceneLibrary;

namespace CandsPositionsLibrary
{
    public class CandsListsServise : ICandsListsServise
    {
        private ICandsListsQueries _candsListsQueries;
        private IPositionsQueries _cvsPositionsQueries;
        private ILuceneSearchService _luceneSearchService;
        private IPositionPropsWriterService _positionPropsWriterService;

        public CandsListsServise(ICandsListsQueries candsListsQueries, IPositionsQueries cvsPositionsQueries, ILuceneSearchService luceneSearchService, IPositionPropsWriterService positionPropsWriterService)
        {
            _candsListsQueries = candsListsQueries;
            _cvsPositionsQueries = cvsPositionsQueries;
            _luceneSearchService = luceneSearchService;
            _positionPropsWriterService = positionPropsWriterService;
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

        public async Task<List<SearchEntry>> ComplexSearchCands(int companyId, SearchTermsModel searchTerms)
        {
            return await _luceneSearchService.ComplexSearch(searchTerms);
        }

        public async Task SaveSearchTerms(SearchTermsModel searchTerms)
        {
            await _cvsPositionsQueries.SaveSearchTerms(searchTerms);
        }

        public async Task<List<SearchTermsListItemModel>> GetSearchTermsList()
        {
            return await _cvsPositionsQueries.GetSearchTermsList();
        }

        public async Task<SearchTermsModel?> GetSearchTermsById(int id)
        {
            return await _cvsPositionsQueries.GetExistPositionSearchTerms(0, id);
        }

        public async Task DeleteSearchTerms(int id)
        {
            await _cvsPositionsQueries.DeleteSearchTerms(id);
        }

        public async Task CleanupOldSearchTerms(int keepCount = 100)
        {
            await _cvsPositionsQueries.CleanupOldSearchTerms(keepCount);
        }

        public async Task<SearchTermsModel?> GetPositionSearchTerms(int positionId, bool isReAnalyze = false, int companyId = 154)
        {
            var searchTerms = isReAnalyze ? null : await _cvsPositionsQueries.GetExistPositionSearchTerms(positionId, 0);

            if (searchTerms == null)
            {
                var position = await _cvsPositionsQueries.GetPosition(positionId, companyId);
                var generated = await _positionPropsWriterService.GetAnalyzedPositionSearchTerms(position.name, position.descr, position.requirements);

                if (generated != null)
                {
                    searchTerms = new SearchTermsModel
                    {
                        PositionId = positionId,
                        MustHave = generated.MustHaveInIndexSearch,
                        ShouldHave = generated.ShouldHaveInIndexSearch,
                        AiSearchPhrase = generated.AiSearchPrompt,
                    };
                }
            }

            return searchTerms;
        }
    }
}
