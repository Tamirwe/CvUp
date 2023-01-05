using AuthLibrary;
using CvsPositionsLibrary;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CvsController : ControllerBase
    {
        private IConfiguration _configuration;
        private ICvsPositionsServise _cvsPosService;

        public CvsController(IConfiguration config, ICvsPositionsServise cvsPosService)
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

        [HttpGet]
        [Route("getCv")]
        public CvModel? getCv(int cvId)
        {
            return _cvsPosService.GetCv(cvId, Globals.CompanyId);

        }
    }
}
