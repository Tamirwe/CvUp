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
            if (data.id == 0)
            {
                _cvsPosService.AddDepartment(data, Globals.CompanyId);
            }
            else
            {
                _cvsPosService.UpdateDepartment(data, Globals.CompanyId);
            }

            return Ok();
        }

        [HttpGet]
        [Route("GetDepartmentsList")]
        public IActionResult GetDepartmentsList()
        {
           List<IdNameModel> departments = _cvsPosService.GetDepartmentsList(Globals.CompanyId);
            return Ok(departments);
        }

        [HttpDelete]
        [Route("DeleteDepartment")]
        public IActionResult DeleteDepartment(int id)
        {
            _cvsPosService.DeleteDepartment(Globals.CompanyId, id);
            return Ok();
        }

        [HttpPost]
        [Route("AddUpdateHrCompany")]
        public IActionResult AddUpdateHrCompany(IdNameModel data)
        {
            if (data.id == 0)
            {
                _cvsPosService.AddHrCompany(data, Globals.CompanyId);
            }
            else
            {
                _cvsPosService.UpdateHrCompany(data, Globals.CompanyId);
            }

            return Ok();
        }

        [HttpGet]
        [Route("GetHrCompaniesList")]
        public IActionResult GetHrCompaniesList()
        {
            List<IdNameModel> departments = _cvsPosService.GetHrCompaniesList(Globals.CompanyId);
            return Ok(departments);
        }

        [HttpDelete]
        [Route("DeleteHrCompany")]
        public IActionResult DeleteHrCompany(int id)
        {
            _cvsPosService.DeleteHrCompany(Globals.CompanyId, id);
            return Ok();
        }
    }
}
