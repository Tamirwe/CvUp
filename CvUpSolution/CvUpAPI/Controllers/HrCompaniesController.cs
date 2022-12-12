using CvsPositionsLibrary;
using Database.models;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HrCompaniesController : ControllerBase
    {
        private IConfiguration _configuration;
        private ICvsPositionsServise _cvsPosService;

        public HrCompaniesController(IConfiguration config, ICvsPositionsServise cvsPosService)
        {
            _configuration = config;
            _cvsPosService = cvsPosService;
        }

        [HttpPost]
        [Route("AddHrCompany")]
        public IActionResult AddHrCompany(HrCompanyModel data)
        {
            HrCompanyModel hrCompanyModel = _cvsPosService.AddUpdateHrCompany(data);
            return Ok(hrCompanyModel);
        }
    }
}
