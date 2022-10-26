using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServicesLibrary.UserLogin;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        public IConfiguration _configuration;
        private IUserLoginServise _userLoginServise;

        public ForgotPasswordController(IConfiguration config, IUserLoginServise userLoginServise)
        {
            _configuration = config;
            _userLoginServise = userLoginServise;
        }

        [HttpPost]
        public IActionResult Post(ForgotPasswordModel data)
        {
            user? authenticateUser = _userLoginServise.ForgotPassword(data.email, data.companyId, out UserAuthStatus status);

            if (authenticateUser != null)
            {
                return Ok("emailSent");
                //return Ok(new { data="emailSent" });
            }
            else if (status == UserAuthStatus.more_then_one_company_per_email)
            {
                var userCompanies = _userLoginServise.UserCompanies(data.email);
                return Ok(userCompanies);
            }

            return Ok("userNotFound");
            //return Ok(new { data = "userNotFound" } );

        }
    }
}
