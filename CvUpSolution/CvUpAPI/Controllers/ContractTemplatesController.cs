using AuthLibrary;
using CvFilesLibrary;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ContractTemplatesController : ControllerBase
    {
        private IContractTemplatesService _contractTemplatesService;
        private IAuthServise _authServise;

        public ContractTemplatesController(IContractTemplatesService contractTemplatesService, IAuthServise authServise)
        {
            _contractTemplatesService = contractTemplatesService;
            _authServise = authServise;
        }

        [HttpPost]
        [Route("SendContract")]
        public async Task<IActionResult> SendContract(SendContractModel contract)
        {
            if (string.IsNullOrWhiteSpace(contract.companyName) || string.IsNullOrWhiteSpace(contract.emailAddress))
            {
                return BadRequest("Company name and email address are required");
            }

            UserModel? user = await _authServise.GetUser(Globals.CompanyId, Globals.UserId);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            var isSent = await _contractTemplatesService.SendContract(contract, user);

            return isSent ? Ok() : NotFound();
        }

        [HttpGet]
        [Route("GetContractSends")]
        public async Task<IActionResult> GetContractSends()
        {
            return Ok(await _contractTemplatesService.GetContractSends());
        }

        [HttpGet]
        [Route("GetContractTemplates")]
        public async Task<IActionResult> GetContractTemplates()
        {
            return Ok(await _contractTemplatesService.GetContractTemplates());
        }

        [HttpPost]
        [Route("AddContractTemplate")]
        public async Task<IActionResult> AddContractTemplate([FromForm] string name, [FromForm] string? descr, IFormFile file)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 300 || descr?.Length > 1000)
            {
                return BadRequest("Invalid contract name or description");
            }

            if (file.Length == 0 || !_contractTemplatesService.IsAllowedFile(file.FileName))
            {
                return BadRequest("Only pdf, doc and docx files are allowed");
            }

            using var fileContent = file.OpenReadStream();

            return Ok(await _contractTemplatesService.AddContractTemplate(name.Trim(), descr?.Trim(), file.FileName, fileContent));
        }

        [HttpGet]
        [Route("GetContractTemplateFile")]
        public async Task<IActionResult> GetContractTemplateFile(int id)
        {
            var pdf = await _contractTemplatesService.GetContractTemplatePdf(id);

            return pdf == null ? NotFound() : File(pdf, "application/pdf");
        }

        [HttpGet]
        [Route("DownloadContractTemplate")]
        public IActionResult DownloadContractTemplate(int id)
        {
            var file = _contractTemplatesService.GetContractTemplateOriginalFile(id);

            return file == null ? NotFound() : File(file.content, file.contentType);
        }

        [HttpDelete]
        [Route("DeleteContractTemplate")]
        public async Task<IActionResult> DeleteContractTemplate(int id)
        {
            await _contractTemplatesService.DeleteContractTemplate(id);

            return Ok();
        }
    }
}
