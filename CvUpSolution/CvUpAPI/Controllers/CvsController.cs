using AuthLibrary;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CvsController : ControllerBase
    {
        public IConfiguration _configuration;
        ICvsPositionsQueries _cvsPosService;

        public CvsController(IConfiguration config, ICvsPositionsQueries cvsPosService)
        {
            _configuration = config;
            _cvsPosService = cvsPosService;
        }

        [HttpGet]
        [Route("GetCvsList")]
        public List<CvListItemModel> GetCvsList()
        {
            return _cvsPosService.GetCvsList(Globals.CompanyId);

        }
    }
}
