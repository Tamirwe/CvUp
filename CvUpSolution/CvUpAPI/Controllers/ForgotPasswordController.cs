using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServicesLibrary.Authentication;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        public IConfiguration _configuration;
        private IAuthServise _authServise;

        public ForgotPasswordController(IConfiguration config, IAuthServise authServise)
        {
            _configuration = config;
            _authServise = authServise;
        }

        [HttpPost]
        public IActionResult Post(ForgotPasswordModel data)
        {
            string origin = Request.Headers["Origin"].First();

            user? authenticateUser = _authServise.ForgotPassword(origin,data.email, data.companyId, out UserAuthStatus status);

            if (authenticateUser != null)
            {
                return Ok("emailSent");
                //return Ok(new { data="emailSent" });
            }
            else if (status == UserAuthStatus.more_then_one_company_per_email)
            {
                var userCompanies = _authServise.UserCompanies(data.email);
                return Ok(userCompanies);
            }

            return Ok("userNotFound");
            //return Ok(new { data = "userNotFound" } );

        }

      
    }
}
