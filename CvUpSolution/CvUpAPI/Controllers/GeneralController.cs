using CandsPositionsLibrary;
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
        private ICandsPositionsServise _cvsPosService;

        public GeneralController(IConfiguration config, ICandsPositionsServise cvsPosService)
        {
            _configuration = config;
            _cvsPosService = cvsPosService;
        }

        [HttpPost]
        [Route("AddUpdateDepartment")]
        public async Task<IActionResult> AddUpdateDepartment(IdNameModel data)
        {
            if (data.id == 0)
            {
                await _cvsPosService.AddDepartment(data, Globals.CompanyId);
            }
            else
            {
                await _cvsPosService.UpdateDepartment(data, Globals.CompanyId);
            }

            return Ok();
        }

        [HttpGet]
        [Route("GetDepartmentsList")]
        public async Task<IActionResult> GetDepartmentsList()
        {
            List<IdNameModel> departments = await _cvsPosService.GetDepartmentsList(Globals.CompanyId);
            return Ok(departments);
        }

        [HttpDelete]
        [Route("DeleteDepartment")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            await _cvsPosService.DeleteDepartment(Globals.CompanyId, id);
            return Ok();
        }

        [HttpPost]
        [Route("AddUpdateHrCompany")]
        public async Task<IActionResult> AddUpdateHrCompany(IdNameModel data)
        {
            if (data.id == 0)
            {
                await _cvsPosService.AddHrCompany(data, Globals.CompanyId);
            }
            else
            {
                await _cvsPosService.UpdateHrCompany(data, Globals.CompanyId);
            }

            return Ok();
        }

        [HttpGet]
        [Route("GetHrCompaniesList")]
        public async Task<IActionResult> GetHrCompaniesList()
        {
            List<IdNameModel> departments = await _cvsPosService.GetHrCompaniesList(Globals.CompanyId);
            return Ok(departments);
        }

        [HttpDelete]
        [Route("DeleteHrCompany")]
        public async Task<IActionResult> DeleteHrCompany(int id)
        {
            await _cvsPosService.DeleteHrCompany(Globals.CompanyId, id);
            return Ok();
        }
    }
}