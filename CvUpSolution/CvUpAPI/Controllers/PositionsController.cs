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

        [HttpGet]
        [Route("GetPosition")]
        public IActionResult GetPosition(int id)
        {
            PositionClientModel position = _cvsPosService.GetPosition(Globals.CompanyId,id);
            return Ok(position);
        }

        [HttpPost]
        [Route("AddUpdatePosition")]
        public IActionResult AddUpdatePosition(PositionClientModel data)
        {
            position? pos;

            if (data.id == 0)
            {
                pos = _cvsPosService.AddPosition(data, Globals.CompanyId, Globals.UserId);
            }
            else
            {
                pos = _cvsPosService.UpdatePosition(data, Globals.CompanyId, Globals.UserId);
            }

            return Ok(pos != null ? pos.id : 0);
        }

        [HttpGet]
        [Route("GetPositionsList")]
        public IActionResult GetPositionsList()
        {
            List<PositionModel> positions = _cvsPosService.GetPositionsList(Globals.CompanyId);
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
