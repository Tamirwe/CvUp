using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModuleGeneratorLibrary;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleGeneratorController : ControllerBase
    {

        private IConfiguration _configuration;
        private IModuleGeneratorService _moduleGeneratorService;

        public ModuleGeneratorController(IConfiguration config, IModuleGeneratorService moduleGeneratorService)
        {
            _configuration = config;
            _moduleGeneratorService = moduleGeneratorService;
        }



        [HttpPost]
        [Route("GenerateModule")]
        public async Task<IActionResult> GenerateModule(ModuleGenerateRequestModel requestParams)
        {

            ModuleGenerateResponseModel response = await _moduleGeneratorService.GenerateModule(requestParams);

          

            return Ok(response);
        }

    }
}
