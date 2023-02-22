using CandsPositionsLibrary;
using ContactsLibrary;
using Database.models;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private IConfiguration _configuration;
        private IContactsService _contactsService;

        public ContactsController(IConfiguration config, IContactsService contactsService)
        {
            _configuration = config;
            _contactsService = contactsService;
        }

        [HttpGet]
        [Route("GetContacts")]
        public async Task<IActionResult> GetContacts(int id)
        {
            List<FolderModel> folders = await _contactsService.GetContacts(Globals.CompanyId, id);
            return Ok(folders);
        }

        [HttpPost]
        [Route("AddContact")]
        public async Task<IActionResult> AddContact(FolderModel data)
        {
            folder newFolder = await _contactsService.AddContact( Globals.CompanyId, data);
            return Ok(newFolder);
        }

        [HttpDelete]
        [Route("Deletecontact")]
        public async Task<IActionResult> Deletecontact(int id)
        {
            await _contactsService.DeleteContact(Globals.CompanyId, id);
            return Ok();
        }
    }
}
