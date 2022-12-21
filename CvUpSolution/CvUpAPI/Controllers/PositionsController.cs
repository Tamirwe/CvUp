using CvsPositionsLibrary;
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
        private ICvsPositionsServise _cvsPosService;

        public PositionsController(IConfiguration config, ICvsPositionsServise cvsPosService)
        {
            _configuration = config;
            _cvsPosService = cvsPosService;
        }

        [HttpPost]
        [Route("AddUpdatePosition")]
        public IActionResult AddUpdatePosition(PositionClientModel data)
        {
            position? position;

            if (data.id == 0)
            {
                position = _cvsPosService.AddPosition(data, Globals.CompanyId, Globals.UserId);
            }
            else
            {
                position = _cvsPosService.UpdatePosition(data, Globals.CompanyId, Globals.UserId);
            }

            return Ok(data);
        }

        [HttpGet]
        [Route("GetPositions")]
        public IActionResult GetPositionsList()
        {
            List<PositionListItemModel> positions = _cvsPosService.GetPositionsList(Globals.CompanyId);
            return Ok(positions);
        }

        [HttpDelete]
        [Route("DeletePosition")]
        public IActionResult DeletePosition(int id)
        {
            _cvsPosService.DeletePosition(Globals.CompanyId, id);
            return Ok();
        }
    }
}
