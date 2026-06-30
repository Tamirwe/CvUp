using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using PgVectorLibrary;
using PgVectorLibrary.AnalyzePositions;
using QueueLibrary;

namespace CandsPositionsLibrary
{
    public class PositionsServise : IPositionsServise
    {
        private IPositionsQueries _cvsPositionsQueries;
        private ICandsListsQueries _candsListsQueries;
        private IDbQueueService _queueService;
        private IAnalyzePositionsService _analyzePositionsService;

        public PositionsServise(IPositionsQueries cvsPositionsQueries, ICandsListsQueries candsListsQueries, IDbQueueService queueService, IAnalyzePositionsService analyzePositionsService)
        {
            _cvsPositionsQueries = cvsPositionsQueries;
            _candsListsQueries = candsListsQueries;
            _queueService = queueService;
            _analyzePositionsService = analyzePositionsService;
        }

        public async Task<PositionModel> GetPosition(int companyId, int positionId)
        {
            return await _cvsPositionsQueries.GetPosition(positionId ,companyId );
        }

        public async Task<int> AddPosition(PositionModel data, int companyId, int userId)
        {
            position newRec = await _cvsPositionsQueries.AddPosition(data, companyId, userId);
            await _cvsPositionsQueries.AddUpdateInterviewers(companyId, newRec.id, data.interviewersIds);
            await _cvsPositionsQueries.AddUpdatePositionContacts(companyId, newRec.id, data.contactsIds);
            await _queueService.EnqueueAsync("analyze position", newRec.id.ToString());
            return newRec.id;
        }

        public async Task<int> UpdatePosition(PositionModel data, int companyId, int userId)
        {
            await _cvsPositionsQueries.UpdatePosition(data, companyId, userId);
            await _cvsPositionsQueries.AddUpdatePositionContacts(companyId, data.id, data.contactsIds);
            await _queueService.EnqueueAsync("analyze position", data.id.ToString());
            return data.id;
        }

        public async Task<List<PositionModel>> GetPositionsList(int companyId)
        {
            return await _cvsPositionsQueries.GetPositionsList(companyId);
        }

        public async Task DeletePosition(int companyId, int id)
        {
            await _cvsPositionsQueries.DeletePosition(companyId, id);
        }

        public async Task<List<int>> getPositionContactsIds(int companyId, int positionId)
        {
            return await _cvsPositionsQueries.getPositionContactsIds(companyId, positionId);
        }

        public async Task<List<ParserRulesModel>> GetParsersRules()
        {
            return await _cvsPositionsQueries.GetParsersRules();
        }

        public async Task<position?> GetPositionByMatchStr(int companyId, string matchStr)
        {
            return await _cvsPositionsQueries.GetPositionByMatchStr(companyId, matchStr);
        }

        public async Task<int?> GetPositionTypeId(int companyId, string positionRelated)
        {
            return await _cvsPositionsQueries.GetPositionTypeId(companyId, positionRelated);
        }

        public async Task<int> AddPositionTypeName(int companyId, string positionRelated)
        {
            return await _cvsPositionsQueries.AddPositionTypeName(companyId, positionRelated);
        }

        public async Task<List<PositionTypeModel>> GetPositionsTypes(int companyId)
        {
            return await _cvsPositionsQueries.GetPositionsTypes(companyId);
        }

        public async Task CalculatePositionTypesCount(int companyId)
        {
            await _cvsPositionsQueries.CalculatePositionTypesCount(companyId);
        }

        public async Task<List<PositionTypeCountModel>> PositionsTypesCvsCount(int companyId)
        {
            return await _cvsPositionsQueries.PositionsTypesCvsCount(companyId);
        }

        public async Task UpdatePositionDate(int companyId, int positionId, bool isUpdateCount)
        {
            await _cvsPositionsQueries.UpdatePositionDate(companyId, positionId, isUpdateCount);
        }

        public async Task<AnalyzedPositionModel?> GetPositionAnalyzedData(int positionId)
        {
            var analyzed = await _cvsPositionsQueries.GetAnalyzedPosition(positionId);

            if (analyzed == null)
            {
                var companyId = await _cvsPositionsQueries.GetPositionCompanyId(positionId);
                await _analyzePositionsService.AnalyzePosition(positionId, companyId);
                analyzed = await _cvsPositionsQueries.GetAnalyzedPosition(positionId);
            }

            return analyzed;
        }
    }
}
