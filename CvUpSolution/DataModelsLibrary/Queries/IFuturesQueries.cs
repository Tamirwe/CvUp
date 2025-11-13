using Database.models;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface IFuturesQueries
    {
        Task<List<FuturesOhlcModel>> GetOhlcList();
        Task<futures_ohlc> AddOhlc(FuturesOhlcModel data);
        Task<futures_ohlc> UpdateOhlc(FuturesOhlcModel data);
        Task DeleteOhlc(int id);
        Task<FuturesOhlcModel> GetDayOhlc(DateTime statisticDate);
    }
}
