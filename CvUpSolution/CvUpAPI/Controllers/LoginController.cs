using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ServicesLibrary.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public IConfiguration _configuration;
        private IAuthServise _authServise;
        public LoginController(IConfiguration config, IAuthServise authServise)
        {
            _configuration = config;
            _authServise = authServise;
        }

        [HttpPost]
        public IActionResult Post(UserLoginModel data)
        {
            try
            {
                user? authenticateUser = _authServise.Login(data, out UserAuthStatus status);

                if (authenticateUser != null)
                {
                    var claims = new[] {
                                //new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                                //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                //new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                                new Claim("UserId", authenticateUser.id.ToString()),
                                new Claim("CompanyId", authenticateUser.company_id.ToString()),
                                new Claim("DisplayName", string.Format("{0} {1}",authenticateUser.first_name,authenticateUser.last_name)),
                                new Claim("email", authenticateUser.email),
                                new Claim("role",Enum.GetName(typeof(UsersRole), authenticateUser.role)?? ""),
                            };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signinCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: signinCredentials
                    );

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else if(status == UserAuthStatus.more_then_one_company_per_email)
                {
                    return Ok(_authServise.UserCompanies(data.email));

                }
            }
            catch(Exception ex)
            {
                return BadRequest(new { Message= "An error occurred in generating the token", ExMessage = ex.Message }); 
            }

            return Unauthorized();

        }

    }
}
