using AuthLibrary;
using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IConfiguration _configuration;
        private IAuthServise _authServise;

        public AuthController(IConfiguration config, IAuthServise authServise)
        {
            _configuration = config;
            _authServise = authServise;
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(UserLoginModel data)
        {
            try
            {
                user? authenticateUser = _authServise.Login(data, out UserAuthStatus status);

                if (authenticateUser != null)
                {
                    TokenModel tokens = _authServise.GenerateAccessToken(authenticateUser);
                    return Ok(tokens);
                }
                else if (status == UserAuthStatus.more_then_one_company_per_email)
                {
                    return Ok(_authServise.UserCompanies(data.email));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "An error occurred in generating the token", ExMessage = ex.Message });
            }

            return Unauthorized();

        }

        [HttpPost]
        [Route("Registration")]
        public IActionResult Registration(CompanyAndUserRegisetModel data)
        {
            try
            {
                string origin = Request.Headers["Origin"].First();
                _authServise.Register(origin, data);
                return Ok();
            }
            catch (Exception ex)
            {
                string msg = "";
                string? innerEx = ex.InnerException?.Message;

                if (innerEx != null && innerEx.Contains("Duplicate entry"))
                {
                    msg = "duplicateUserPass";
                }

                return BadRequest(new { ErrorMessage = msg, Ex = ex.Message, ExInner = ex.InnerException?.Message, Stack = ex.StackTrace });
            }
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public IActionResult ForgotPassword(ForgotPasswordModel data)
        {
            string origin = Request.Headers["Origin"].First();

            user? authenticateUser = _authServise.ForgotPassword(origin, data.email, data.companyId, out UserAuthStatus status);

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

        [HttpPost]
        [Route("Refresh")]
        public IActionResult Refresh(TokenModel tokens)
        {
            if (tokens.refreshToken is null || tokens.token is null)
                return BadRequest("Invalid client request");

            TokenModel? newToken = _authServise.RefreshToken(tokens.token, tokens.refreshToken);

            if (newToken is null)
            {
                return Unauthorized();
            }

            return Ok(newToken);

        }

    }
}
