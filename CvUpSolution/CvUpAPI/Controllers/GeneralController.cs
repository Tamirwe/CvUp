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
        [Route("AddUpdateDepartment")]
        public IActionResult AddUpdateDepartment(IdNameModel data)
        {
            department? department;

            if (data.id == 0)
            {
                department = _cvsPosService.AddDepartment(data, Globals.CompanyId);
            }
            else
            {
                department = _cvsPosService.UpdateDepartment(data, Globals.CompanyId);
            }

            return Ok(department);
        }

        [HttpGet]
        [Route("GetDepartments")]
        public IActionResult GetDepartments()
        {
           List<IdNameModel> departments = _cvsPosService.GetDepartments(Globals.CompanyId);
            return Ok(departments);
        }

        [HttpDelete]
        [Route("DeleteDepartment")]
        public IActionResult DeleteDepartment(int id)
        {
            department? result = _cvsPosService.DeleteDepartment(Globals.CompanyId, id);
            return Ok(result);
        }

        [HttpPost]
        [Route("AddUpdateHrCompany")]
        public IActionResult AddUpdateHrCompany(IdNameModel data)
        {
            hr_company? hrCompany;

            if (data.id == 0)
            {
                hrCompany = _cvsPosService.AddHrCompany(data, Globals.CompanyId);
            }
            else
            {
                hrCompany = _cvsPosService.UpdateHrCompany(data, Globals.CompanyId);
            }

            return Ok(hrCompany);
        }

        [HttpGet]
        [Route("GetHrCompanies")]
        public IActionResult GetHrCompanies()
        {
            List<IdNameModel> departments = _cvsPosService.GetHrCompanies(Globals.CompanyId);
            return Ok(departments);
        }

        [HttpDelete]
        [Route("DeleteHrCompany")]
        public IActionResult DeleteHrCompany(int id)
        {
            hr_company? result = _cvsPosService.DeleteHrCompany(Globals.CompanyId, id);
            return Ok(result);
        }
    }
}
