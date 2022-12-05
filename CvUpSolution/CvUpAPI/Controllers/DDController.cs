using CvsPositionsLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DDController : ControllerBase
    {
        private IConfiguration _configuration;

        public DDController(IConfiguration config, ICvsPositionsServise cvsPosService)
        {
            _configuration = config;
        }

        [HttpGet]
        public IActionResult? Get(string id)
        {
            string decripted = GeneralLibrary.Encriptor.Encrypt(id, _configuration["GlobalSettings:cvsEncryptorKey"]);
            string[] parts = decripted.Split("~");

            if (Convert.ToDateTime(parts[1]).Date == DateTime.Now.Date)
            {
                return PhysicalFile(parts[0], "application/msword", true);
            }

            return null;

        }
    }
}
