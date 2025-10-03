using CustomersContactsLibrary;
using Database.models;
using DataModelsLibrary.Models;
using FuturesStatisticsLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuturesStatisticsController : ControllerBase
    {
        private IConfiguration _configuration;
        private IFuturesStatisticsService _futuresStatisticsService;

        public FuturesStatisticsController(IConfiguration config, IFuturesStatisticsService futuresStatisticsService)
        {
            _configuration = config;
            _futuresStatisticsService = futuresStatisticsService;
        }


        [HttpGet]
        [Route("GetDayOhlcList")]
        public async Task<IActionResult> GetDayOhlcList()
        {
            List<FuturesOhlcModel> folders = await _futuresStatisticsService.GetOhlcList();
            return Ok(folders);
        }

        [HttpPost]
        [Route("AddDayOhlc")]
        public async Task<IActionResult> AddDayOhlc(FuturesOhlcModel data)
        {

            FuturesOhlcModel? ohlcDayData = await _futuresStatisticsService.GetDayOhlc(data.statisticDate);

            if (ohlcDayData != null )
            {
                  return BadRequest("duplicate date");
            }

            futures_ohlc addedData = await _futuresStatisticsService.AddOhlc(data);
            return Ok(addedData);
        }

        [HttpPut]
        [Route("UpdateDayOhlc")]
        public async Task<IActionResult> UpdateDayOhlc(FuturesOhlcModel data)
        {
            futures_ohlc updateData = await _futuresStatisticsService.UpdateOhlc(data);
            return Ok(updateData);
        }

        [HttpDelete]
        [Route("DeleteDayOhlc")]
        public async Task<IActionResult> DeleteDayOhlc(int id)
        {
            await _futuresStatisticsService.DeleteOhlc(id);
            return Ok();
        }
    }
}
