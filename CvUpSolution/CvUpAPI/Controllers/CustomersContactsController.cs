using CustomersContactsLibrary;
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
    public class CustomersContactsController : ControllerBase
    {
        private IConfiguration _configuration;
        private ICustomersContactsService _customersContactsService;

        public CustomersContactsController(IConfiguration config, ICustomersContactsService customersContactsService)
        {
            _configuration = config;
            _customersContactsService = customersContactsService;
        }

        [HttpGet]
        [Route("GetContacts")]
        public async Task<IActionResult> GetContacts()
        {
            List<ContactModel> folders = await _customersContactsService.GetContacts(Globals.CompanyId);
            return Ok(folders);
        }

        [HttpPost]
        [Route("AddContact")]
        public async Task<IActionResult> AddContact(ContactModel data)
        {
            contact newFolder = await _customersContactsService.AddContact(Globals.CompanyId, data);
            return Ok(newFolder);
        }

        [HttpPut]
        [Route("UpdateContact")]
        public async Task<IActionResult> UpdateContact(ContactModel data)
        {
            contact newFolder = await _customersContactsService.UpdateContact(Globals.CompanyId, data);
            return Ok(newFolder);
        }

        [HttpDelete]
        [Route("Deletecontact")]
        public async Task<IActionResult> Deletecontact(int id)
        {
            await _customersContactsService.DeleteContact(Globals.CompanyId, id);
            return Ok();
        }

        [HttpPost]
        [Route("AddCustomer")]
        public async Task<IActionResult> AddCustomer(IdNameModel data)
        {
            customer newRec =await _customersContactsService.AddCustomer(data, Globals.CompanyId);
            return Ok(new IdNameModel { id= newRec.id, name=newRec.name });
        }

        [HttpPut]
        [Route("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(IdNameModel data)
        {
            customer? newRec = await _customersContactsService.UpdateCustomer(data, Globals.CompanyId);

            if (newRec != null)
            {
                return Ok(new IdNameModel { id = newRec.id, name = newRec.name });

            }

            return BadRequest();
        }

        [HttpGet]
        [Route("GetCustomersList")]
        public async Task<IActionResult> GetCustomersList()
        {
            List<IdNameModel> customers = await _customersContactsService.GetCustomersList(Globals.CompanyId);
            return Ok(customers);
        }

        [HttpDelete]
        [Route("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            await _customersContactsService.DeleteCustomer(Globals.CompanyId, id);
            return Ok();
        }
    }
}
