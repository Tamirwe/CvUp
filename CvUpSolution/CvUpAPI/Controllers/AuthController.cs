using AuthLibrary;
using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Authorization;
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
            user? authenticateUser = _authServise.Login(data);

            if (authenticateUser != null)
            {
                TokenModel tokens = _authServise.GenerateAccessToken(authenticateUser, data.rememberMe);
                return Ok(tokens);
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("CompleteRegistration")]
        public IActionResult CompleteRegistration(UserLoginModel data)
        {
            user? authenticateUser = _authServise.CompleteRegistration(data);

            if (authenticateUser != null)
            {
                TokenModel tokens = _authServise.GenerateAccessToken(authenticateUser, data.rememberMe);
                return Ok(tokens);
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("PasswordReset")]
        public IActionResult PasswordReset(UserLoginModel data)
        {
            user? authenticateUser = _authServise.PasswordReset(data);

            if (authenticateUser != null)
            {
                TokenModel tokens = _authServise.GenerateAccessToken(authenticateUser, data.rememberMe);
                return Ok(tokens);
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("Registration")]
        public IActionResult Registration(CompanyAndUserRegisetModel data)
        {
            bool isDuplicateUserPassword = _authServise.CheckDuplicateUserPassword(data);

            if (isDuplicateUserPassword)
            {
                return Ok("duplicateUserPass");
            }

            string origin = Request.Headers["Origin"].First();
            _authServise.Register(origin, data);
            return Ok();
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
                return Ok();
            }

            return Ok(newToken);

        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public IActionResult Revoke()
        {
            _authServise.RevokeToken(Globals.UserId);
            return NoContent();
        }

    }
}
