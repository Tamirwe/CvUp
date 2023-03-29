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
        public async Task<IActionResult> Login(UserLoginModel data)
        {
            user? authenticateUser = await _authServise.Login(data);

            if (authenticateUser != null)
            {
                TokenModel tokens = await _authServise.GenerateAccessToken(authenticateUser, data.rememberMe);
                return Ok(tokens);
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("CompleteRegistration")]
        public async Task<IActionResult> CompleteRegistration(UserLoginModel data)
        {
            user? authenticateUser = await _authServise.CompleteRegistration(data);

            if (authenticateUser != null)
            {
                TokenModel tokens = await _authServise.GenerateAccessToken(authenticateUser, data.rememberMe);
                return Ok(tokens);
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("PasswordReset")]
        public async Task<IActionResult> PasswordReset(UserLoginModel data)
        {
            user? authenticateUser = await _authServise.PasswordReset(data);

            if (authenticateUser != null)
            {
                TokenModel tokens = await _authServise.GenerateAccessToken(authenticateUser, data.rememberMe);
                return Ok(tokens);
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("Registration")]
        public async Task<IActionResult> Registration(CompanyAndUserRegisetModel data)
        {
            bool isUserDuplicate = await _authServise.CheckUserDuplicate(data);

            if (isUserDuplicate)
            {
                return Ok("duplicateUserPass");
            }

            string? origin = Request.Headers["Origin"].First();
            await _authServise.AddCompanyAndFirstUser(origin, data);
            return Ok();
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel data)
        {
            string? origin = Request.Headers["Origin"].FirstOrDefault();

            if (origin == null)
            {
                return BadRequest();
            }

            UserStatusModel authenticateUser = await _authServise.ForgotPassword(origin, data.email, data.companyId);

            if (authenticateUser.user != null)
            {
                return Ok("emailSent");
                //return Ok(new { data="emailSent" });
            }
            else if (authenticateUser.status == UserAuthStatus.more_then_one_company_per_email)
            {
                var userCompanies = await _authServise.UserCompanies(data.email);
                return Ok(userCompanies);
            }

            return Ok("userNotFound");
            //return Ok(new { data = "userNotFound" } );

        }

        [HttpPost]
        [Route("Refresh")]
        public async Task<IActionResult> Refresh(TokenModel tokens)
        {
            if (tokens.refreshToken is null || tokens.token is null)
                return BadRequest("Invalid client request");

            TokenModel? newToken = await _authServise.RefreshToken(tokens.token, tokens.refreshToken);

            if (newToken is null)
            {
                return Ok();
            }

            return Ok(newToken);

        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public async Task<IActionResult> Revoke()
        {
            await _authServise.RevokeToken(Globals.UserId);
            return NoContent();
        }

        [HttpPost]
        [Route("AddUpdateInterviewer")]
        public async Task<IActionResult> AddUpdateInterviewer(InterviewerModel data)
        {

            if (data.id == 0)
            {
                await _authServise.AddInterviewer(data, Globals.CompanyId);
            }
            else
            {
                await _authServise.UpdateInterviewer(data, Globals.CompanyId);
            }

            return Ok();
        }

        [HttpGet]
        [Route("GetInterviewersList")]
        public async Task<IActionResult> GetInterviewersList()
        {
            List<InterviewerModel> users = await _authServise.GetInterviewersList(Globals.CompanyId);
            return Ok(users);
        }

        [HttpDelete]
        [Route("DeleteInterviewer")]
        public async Task<IActionResult> DeleteInterviewer(int id)
        {
            await _authServise.DeleteUser(Globals.CompanyId, id);
            return Ok();
        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            List<UserModel> users = await _authServise.GetUsers(Globals.CompanyId);
            return Ok(users);
        }

        [HttpPost]
        [Route("AddCompanyUser")]
        public async Task<IActionResult> AddCompanyUser(UserModel data)
        {
            bool isUserDuplicate = await _authServise.CheckCompanyUserDuplicate(data, Globals.CompanyId);

            if (isUserDuplicate)
            {
                return BadRequest("duplicateUserPass");
            }

            string? origin = Request.Headers["Origin"].First();
            await _authServise.AddCompanyUser(origin,data, Globals.CompanyId);
            return Ok();
        }

        [HttpPut]
        [Route("UpdateCompanyUser")]
        public async Task<IActionResult> UpdateCompanyUser(UserModel data)
        {
            await _authServise.UpdateCompanyUser(data, Globals.CompanyId);
            return Ok();
        }

        [HttpDelete]
        [Route("DeleteCompanyUser")]
        public async Task<IActionResult> DeleteCompanyUser(int id)
        {
            await _authServise.DeleteUser(Globals.CompanyId, id);
            return Ok();
        }

        [HttpPut]
        [Route("ActivateCompanyUser")]
        public async Task<IActionResult> ActivateCompanyUser(UserModel data)
        {
            string? origin = Request.Headers["Origin"].First();
            await _authServise.ActivateCompanyUser(origin,Globals.CompanyId, data);
            return Ok();
        }

        [HttpPut]
        [Route("DactivateCompanyUser")]
        public async Task<IActionResult> DactivateCompanyUser(UserModel data)
        {
            await _authServise.DactivateCompanyUser(Globals.CompanyId, data);
            return Ok();
        }

        [HttpPut]
        [Route("ResendRegistrationEmail")]
        public async Task<IActionResult> ResendRegistrationEmail(UserModel data)
        {
            string? origin = Request.Headers["Origin"].First();
            await _authServise.ResendRegistrationEmail(origin,data, Globals.CompanyId);
            return Ok();
        }
    }
}
