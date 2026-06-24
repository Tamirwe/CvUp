using Database.models;
using DataModelsLibrary.Models;

namespace CandsPositionsLibrary
{
    public interface IPositionsServise
    {
        Task<PositionModel> GetPosition(int companyId, int positionId);
        Task<int> AddPosition(PositionModel data, int companyId, int userId);
        Task<int> UpdatePosition(PositionModel data, int companyId, int userId);
        Task<List<PositionModel>> GetPositionsList(int companyId);
        Task DeletePosition(int companyId, int id);
        Task<List<int>> getPositionContactsIds(int companyId, int positionId);
        Task<List<ParserRulesModel>> GetParsersRules();
        Task<position?> GetPositionByMatchStr(int companyId, string matchStr);
        Task<int?> GetPositionTypeId(int companyId, string positionRelated);
        Task<int> AddPositionTypeName(int companyId, string positionRelated);
        Task<List<PositionTypeModel>> GetPositionsTypes(int companyId);
        Task CalculatePositionTypesCount(int companyId);
        Task<List<PositionTypeCountModel>> PositionsTypesCvsCount(int companyId);
        Task UpdatePositionDate(int companyId, int positionId, bool isUpdateCount);
        Task<AnalyzedPositionModel?> GetPositionAnalyzedData(int positionId);
    }
}
