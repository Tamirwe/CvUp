using CandsPositionsLibrary;
using Database.models;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionsController : ControllerBase
    {
        private IConfiguration _configuration;
        private ICandsPositionsServise _cvsPosService;

        public PositionsController(IConfiguration config, ICandsPositionsServise cvsPosService)
        {
            _configuration = config;
            _cvsPosService = cvsPosService;
        }

        [HttpGet]
        [Route("GetPosition")]
        public async Task<IActionResult> GetPosition(int id)
        {
            PositionClientModel position = await _cvsPosService.GetPosition(Globals.CompanyId, id);
            return Ok(position);
        }

        [HttpPost]
        [Route("AddUpdatePosition")]
        public async Task<IActionResult> AddUpdatePosition(PositionClientModel data)
        {
            position? pos;

            if (data.id == 0)
            {
                pos = await _cvsPosService.AddPosition(data, Globals.CompanyId, Globals.UserId);
            }
            else
            {
                pos = await _cvsPosService.UpdatePosition(data, Globals.CompanyId, Globals.UserId);
            }

            return Ok(pos != null ? pos.id : 0);
        }

        [HttpGet]
        [Route("GetPositionsList")]
        public async Task<IActionResult> GetPositionsList()
        {
            List<PositionModel> positions = await _cvsPosService.GetPositionsList(Globals.CompanyId);
            return Ok(positions);
        }

        [HttpDelete]
        [Route("DeletePosition")]
        public async Task<IActionResult> DeletePosition(int id)
        {
            await _cvsPosService.DeletePosition(Globals.CompanyId, id);
            return Ok();
        }
    }
}
