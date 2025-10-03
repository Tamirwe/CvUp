using Database.models;
using DataModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuturesStatisticsLibrary
{
    public interface IFuturesStatisticsService
    {
        Task<List<FuturesOhlcModel>> GetOhlcList();
        Task<futures_ohlc> AddOhlc(FuturesOhlcModel data);
        Task<futures_ohlc> UpdateOhlc(FuturesOhlcModel data);
        Task DeleteOhlc( int id);
        Task<FuturesOhlcModel?> GetDayOhlc(DateTime statisticDate);
    }
}
