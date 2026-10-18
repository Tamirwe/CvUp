using AuthLibrary;
using CustomersContactsLibrary;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class CustomerContractsController : ControllerBase
    {
        // What the admin dashboard widget shows.
        private const int LatestContractsCount = 10;

        private ICustomerContractsService _customerContractsService;
        private IAuthServise _authServise;

        public CustomerContractsController(
            ICustomerContractsService customerContractsService,
            IAuthServise authServise)
        {
            _customerContractsService = customerContractsService;
            _authServise = authServise;
        }

        [HttpGet]
        [Route("GetContractTemplates")]
        public async Task<IActionResult> GetContractTemplates()
        {
            List<ContractTemplateModel> templates =
                await _customerContractsService.GetContractTemplates(Globals.CompanyId);
            return Ok(templates);
        }

        [HttpPost]
        [Route("AddContractTemplate")]
        public async Task<IActionResult> AddContractTemplate(
            [FromForm] IFormFile file, [FromForm] string name, [FromForm] string? descr)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file.");
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    ContractTemplateModel template = await _customerContractsService.AddContractTemplate(
                        Globals.CompanyId, name, descr, file.FileName, stream);

                    return Ok(template);
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("UpdateContractTemplate")]
        public async Task<IActionResult> UpdateContractTemplate(ContractTemplateModel data)
        {
            ContractTemplateModel? template =
                await _customerContractsService.UpdateContractTemplate(Globals.CompanyId, data);

            if (template == null)
            {
                return BadRequest();
            }

            return Ok(template);
        }

        [HttpDelete]
        [Route("DeleteContractTemplate")]
        public async Task<IActionResult> DeleteContractTemplate(int id)
        {
            await _customerContractsService.DeleteContractTemplate(Globals.CompanyId, id);
            return Ok();
        }

        [HttpGet]
        [Route("GetCustomerLatestContract")]
        public async Task<IActionResult> GetCustomerLatestContract(int customerId)
        {
            CustomerContractModel? contract =
                await _customerContractsService.GetCustomerLatestContract(Globals.CompanyId, customerId);
            return Ok(contract);
        }

        [HttpGet]
        [Route("GetLatestContracts")]
        public async Task<IActionResult> GetLatestContracts()
        {
            List<CustomerContractModel> contracts =
                await _customerContractsService.GetLatestContracts(Globals.CompanyId, LatestContractsCount);
            return Ok(contracts);
        }

        [HttpPost]
        [Route("SendContract")]
        public async Task<IActionResult> SendContract(SendContractModel data)
        {
            UserModel? user = await _authServise.GetUser(Globals.CompanyId, Globals.UserId);

            try
            {
                string fileName = await _customerContractsService.SendContract(Globals.CompanyId, data, user);
                return Ok(new { fileName });
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is FileNotFoundException)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
