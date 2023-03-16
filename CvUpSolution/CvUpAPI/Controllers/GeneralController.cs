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
        [Route("AddUpdateCustomer")]
        public async Task<IActionResult> AddUpdateCustomer(IdNameModel data)
        {
            if (data.id == 0)
            {
                await _cvsPosService.AddCustomer(data, Globals.CompanyId);
            }
            else
            {
                await _cvsPosService.UpdateCustomer(data, Globals.CompanyId);
            }

            return Ok();
        }

        [HttpGet]
        [Route("GetCustomersList")]
        public async Task<IActionResult> GetCustomersList()
        {
            List<IdNameModel> customers = await _cvsPosService.GetCustomersList(Globals.CompanyId);
            return Ok(customers);
        }

        [HttpDelete]
        [Route("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            await _cvsPosService.DeleteCustomer(Globals.CompanyId, id);
            return Ok();
        }

    }
}