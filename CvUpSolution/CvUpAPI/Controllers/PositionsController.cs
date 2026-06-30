using AiLibrary.PositionPropsWriter;
using OpenAiLibrary.PositionPropsWriter;
using CandsPositionsLibrary;
using Database.models;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PositionsController : ControllerBase
    {
        private IPositionsServise _positionsService;
        private IPositionPropsWriterService _positionPropsWriterService;

        public PositionsController(IPositionsServise positionsService, IPositionPropsWriterService positionPropsWriterService)
        {
            _positionsService = positionsService;
            _positionPropsWriterService = positionPropsWriterService;
        }

        [HttpGet]

        [Route("GetPosition")]
        public async Task<IActionResult> GetPosition(int id)
        {
            PositionModel position = await _positionsService.GetPosition(Globals.CompanyId, id);
            return Ok(position);
        }

        [HttpGet]
        [Route("getPositionContactsIds")]
        public async Task<IActionResult> getPositionContactsIds(int posId)
        {
            var positionContacts = await _positionsService.getPositionContactsIds(Globals.CompanyId, posId);
            return Ok(positionContacts);
        }

        [HttpGet]
        [Route("GetPositionAnalyzedData")]
        public async Task<AnalyzedPositionModel?> GetPositionAnalyzedData(int positionId)
        {
            return await _positionsService.GetPositionAnalyzedData(positionId);
        }

        [HttpPut]
        [Route("PositionDescrAiRewrite")]
        public async Task<IActionResult> PositionDescrAiRewrite(PositionModel data)
        {
            var result = await _positionPropsWriterService.PositionPropsRewriteAsync(data, PositionPropsRewriteType.Description);
            return Ok(result);
        }

        [HttpPut]
        [Route("PositionRequirementsAiRewrite")]
        public async Task<IActionResult> PositionRequirementsAiRewrite(PositionModel data)
        {
            var result = await _positionPropsWriterService.PositionPropsRewriteAsync(data, PositionPropsRewriteType.Requirements);
            return Ok(result);
        }

        [HttpPut]
        [Route("PositionAdAiRewrite")]
        public async Task<IActionResult> PositionAdAiRewrite(PositionModel data)
        {
            var result = await _positionPropsWriterService.PositionPropsRewriteAsync(data, PositionPropsRewriteType.Ad);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetPositionsList")]
        public async Task<IActionResult> GetPositionsList()
        {
            List<PositionModel> positions = await _positionsService.GetPositionsList(Globals.CompanyId);
            return Ok(positions);
        }

        [HttpPost]
        [Route("AddPosition")]
        public async Task<IActionResult> AddPosition(PositionModel data)
        {
            var posId = await _positionsService.AddPosition(data, Globals.CompanyId, Globals.UserId);
            return Ok(posId);
        }

        [HttpPut]
        [Route("UpdatePosition")]
        public async Task<IActionResult> UpdatePosition(PositionModel data)
        {
            var posId = await _positionsService.UpdatePosition(data, Globals.CompanyId, Globals.UserId);
            return Ok(posId);
        }

        [HttpDelete]
        [Route("DeletePosition")]
        public async Task<IActionResult> DeletePosition(int id)
        {
            await _positionsService.DeletePosition(Globals.CompanyId, id);
            return Ok();
        }

        [HttpGet]
        [Route("GetPositionsTypes")]
        public async Task<IActionResult> GetPositionsTypes()
        {
            List<PositionTypeModel> positionTypes = await _positionsService.GetPositionsTypes(Globals.CompanyId);
            return Ok(positionTypes);
        }

        [HttpGet]
        [Route("PositionsTypesCvsCount")]
        public async Task<IActionResult> PositionsTypesCvsCount()
        {
            List<PositionTypeCountModel> positionTypes = await _positionsService.PositionsTypesCvsCount(Globals.CompanyId);
            return Ok(positionTypes);
        }
    }
}
