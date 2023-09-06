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
    public class GeneralController : ControllerBase
    {
        private IConfiguration _configuration;
        private ICandsPositionsServise _cvsPosService;
        private ITranslateService _translateService;

        public GeneralController(IConfiguration config, ICandsPositionsServise cvsPosService, ITranslateService translateService)
        {
            _configuration = config;
            _cvsPosService = cvsPosService;
            _translateService = translateService;
        }

        [HttpPost]
        [Route("TranslateSingleLine")]
        public async Task<string> TranslateSingleLine(TranslateModel trans)
        {
            return await _translateService.SingleLine(trans.txt, trans.lang);
        }

        [HttpPost]
        [Route("TranslateMultiLines")]
        public async Task<IActionResult> TranslateMultiLines(TranslateModel trans)
        {
            try
            {
                 var aaa = await _translateService.MultiLines(trans.txtList, trans.lang);
                return Ok(aaa);

            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }

    }
}