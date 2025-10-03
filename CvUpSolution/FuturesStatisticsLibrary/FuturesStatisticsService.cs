using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuturesStatisticsLibrary
{


    public class FuturesStatisticsService : IFuturesStatisticsService
    {
        private IConfiguration _configuration;
        private IFuturesQueries _futuresQueries;

        public FuturesStatisticsService(IConfiguration config, IFuturesQueries futuresQueries)
        {
            _configuration = config;
            _futuresQueries = futuresQueries;
        }

        public async Task<List<FuturesOhlcModel>> GetOhlcList()
        {
            return await _futuresQueries.GetOhlcList();
        }

        public async Task<futures_ohlc> AddOhlc(FuturesOhlcModel data)
        {
            return await _futuresQueries.AddOhlc(data);
        }

        public async Task<futures_ohlc> UpdateOhlc(FuturesOhlcModel data)
        {
            return await _futuresQueries.UpdateOhlc(data);
        }


        public async Task DeleteOhlc(int id)
        {
            await _futuresQueries.DeleteOhlc(id);
        }

        public async Task<FuturesOhlcModel?> GetDayOhlc(DateTime statisticDate)
        {
            return await _futuresQueries.GetDayOhlc(statisticDate);
        }
    }

}
