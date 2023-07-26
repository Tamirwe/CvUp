﻿using CandsPositionsLibrary;
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
            PositionModel position = await _cvsPosService.GetPosition(Globals.CompanyId, id);
            return Ok(position);
        }

        [HttpGet]
        [Route("GetPositionsList")]
        public async Task<IActionResult> GetPositionsList()
        {
            List<PositionModel> positions = await _cvsPosService.GetPositionsList(Globals.CompanyId);
            return Ok(positions);
        }

        [HttpPost]
        [Route("AddPosition")]
        public async Task<IActionResult> AddPosition(PositionModel data)
        {
            var posId = await _cvsPosService.AddPosition(data, Globals.CompanyId, Globals.UserId);
            return Ok(posId);
        }

        [HttpPut]
        [Route("UpdatePosition")]
        public async Task<IActionResult> UpdatePosition(PositionModel data)
        {
            var posId = await _cvsPosService.UpdatePosition(data, Globals.CompanyId, Globals.UserId);
            return Ok(posId);
        }

        [HttpDelete]
        [Route("DeletePosition")]
        public async Task<IActionResult> DeletePosition(int id)
        {
            await _cvsPosService.DeletePosition(Globals.CompanyId, id);
            return Ok();
        }

        [HttpPut]
        [Route("ActivatePosition")]
        public async Task<IActionResult> ActivatePosition(PositionModel data)
        {
            await _cvsPosService.ActivatePosition( Globals.CompanyId, data);
            return Ok();
        }

        [HttpPut]
        [Route("DactivatePosition")]
        public async Task<IActionResult> DactivatePosition(PositionModel data)
        {
            await _cvsPosService.DactivatePosition(Globals.CompanyId, data);
            return Ok();
        }
    }
}
