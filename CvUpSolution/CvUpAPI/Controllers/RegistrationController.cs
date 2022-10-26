using CvUpAPI.Services;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServicesLibrary.Authentication;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private IAuthServise _authServise;
        public RegistrationController(IAuthServise authServise)
        {
            _authServise = authServise;
        }

        [HttpPost]
        public IActionResult Post(CompanyAndUserRegisetModel data)
        {
            try
            {
                _authServise.Register(data);


                return Ok();
     
            }
            catch(Exception ex)
            {
                string msg = "";
                string? innerEx = ex.InnerException?.Message;

                if (innerEx != null && innerEx.Contains("Duplicate entry"))
                {
                    msg = "duplicateUserPass";
                }

                return BadRequest(new { ErrorMessage = msg, Ex = ex.Message,ExInner=ex.InnerException?.Message,Stack= ex.StackTrace });
            }
        }
    }
}
