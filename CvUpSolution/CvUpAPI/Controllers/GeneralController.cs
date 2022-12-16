using CvsPositionsLibrary;
using Database.models;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        private IConfiguration _configuration;
        private ICvsPositionsServise _cvsPosService;

        public GeneralController(IConfiguration config, ICvsPositionsServise cvsPosService)
        {
            _configuration = config;
            _cvsPosService = cvsPosService;
        }

        [HttpPost]
        [Route("AddUpdateHrCompany")]
        public IActionResult AddUpdateHrCompany(IdNameModel data)
        {
            hr_company hrCompanyModel = _cvsPosService.AddUpdateHrCompany(data);
            return Ok(hrCompanyModel);
        }

        [HttpPost]
        [Route("AddUpdateDepartment")]
        public IActionResult AddUpdateDepartment(IdNameModel data)
        {
            data.companyId = Globals.CompanyId;
            department department = _cvsPosService.AddUpdateDepartment(data);
            return Ok(department);
        }

        [HttpGet]
        [Route("GetCompanyDepartments")]
        public IActionResult GetCompanyDepartments()
        {
           List<IdNameModel> departments = _cvsPosService.GetCompanyDepartments(Globals.CompanyId);
            return Ok(departments);
        }

        [HttpDelete]
        [Route("DeleteCompanyDepartment")]
        public IActionResult DeleteCompanyDepartment(int id)
        {
            department? result = _cvsPosService.DeleteCompanyDepartment(Globals.CompanyId, id);
            return Ok(result);
        }
    }
}
