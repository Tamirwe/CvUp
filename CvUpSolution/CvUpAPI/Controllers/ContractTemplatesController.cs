using CvFilesLibrary;
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

        public ContractTemplatesController(IContractTemplatesService contractTemplatesService)
        {
            _contractTemplatesService = contractTemplatesService;
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

        [HttpDelete]
        [Route("DeleteContractTemplate")]
        public async Task<IActionResult> DeleteContractTemplate(int id)
        {
            await _contractTemplatesService.DeleteContractTemplate(id);

            return Ok();
        }
    }
}
