using Database.models;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DataModelsLibrary.Queries
{
    public class FuturesQueries : IFuturesQueries
    {
        public async Task<List<FuturesOhlcModel>> GetOhlcList()
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from d in dbContext.futures_ohlcs

                            orderby d.statistic_date descending
                            select new FuturesOhlcModel
                            {
                                id = d.id,
                                statisticDate = d.statistic_date,
                                open = d.open,
                                high = d.high,
                                low = d.low,
                                close = d.close,
                            };

                return await query.ToListAsync();
            }
        }

        public async Task<futures_ohlc> AddOhlc(FuturesOhlcModel data)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = new futures_ohlc
                {
                    statistic_date = data.statisticDate,
                    open = data.open,
                    high = data.high,
                    low = data.low,
                    close = data.close,
                };

                dbContext.futures_ohlcs.Add(query);
                await dbContext.SaveChangesAsync();
                return query;
            }
        }
        public async Task<futures_ohlc> UpdateOhlc(FuturesOhlcModel data)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = new futures_ohlc
                {
                    id = data.id,
                    statistic_date = data.statisticDate,
                    open = data.open,
                    high = data.high,
                    low = data.low,
                    close = data.close,
                };

                var result = dbContext.futures_ohlcs.Update(query);
                await dbContext.SaveChangesAsync();
                return result.Entity;
            }
        }

        public async Task DeleteOhlc(int id)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = await (from d in dbContext.futures_ohlcs
                                   where d.id == id
                                   select d).FirstOrDefaultAsync();

                if (query != null)
                {
                    var result = dbContext.futures_ohlcs.Remove(query);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<FuturesOhlcModel?> GetDayOhlc(DateTime statisticDate)
        {
            using (var dbContext = new cvup00001Context())
            {
                var query = from d in dbContext.futures_ohlcs
                            where d.statistic_date == statisticDate
                            select new FuturesOhlcModel
                            {
                                id = d.id,
                                statisticDate = d.statistic_date,
                                open = d.open,
                                high = d.high,
                                low = d.low,
                                close = d.close,
                            };

                return await query.FirstOrDefaultAsync();
            }
        }

     

    }
}
