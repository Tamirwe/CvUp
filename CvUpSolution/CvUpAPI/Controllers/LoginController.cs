﻿using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ServicesLibrary.UserLogin;
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
        private IUserLoginServise _userLoginServise;
        public LoginController(IConfiguration config, IUserLoginServise userLoginServise)
        {
            _configuration = config;
            _userLoginServise = userLoginServise;
        }

        [HttpPost]
        public IActionResult Post(UserLoginModel data)
        {
            try
            {
                user? authenticateUser = _userLoginServise.Login(data, out UserAuthStatus status);

                if (authenticateUser != null)
                {
                    var claims = new[] {
                                //new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                                //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                //new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                                new Claim("UserId", authenticateUser.id.ToString()),
                                new Claim("DisplayName", string.Format("{0} {1}",authenticateUser.first_name,authenticateUser.last_name)),
                                new Claim("email", authenticateUser.email)
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
                    return Ok(_userLoginServise.UserCompanies(data.email));

                }
            }
            catch(Exception ex)
            {
                return BadRequest(new { Message= "An error occurred in generating the token", ExMessage = ex.Message }); 
            }

            return Unauthorized();

        }

        [HttpPost]
        public IActionResult Post(string email)
        {
            user? authenticateUser = _userLoginServise.ForgotPassword(email, out UserAuthStatus status);

            if (authenticateUser != null)
            {
                return Ok("emailSent");
            }
            else if (status == UserAuthStatus.more_then_one_company_per_email)
            {
                var userCompanies = _userLoginServise.UserCompanies(email);
                return Ok(userCompanies);
            }

            return Ok("userNotFound");

        }
    }
}
