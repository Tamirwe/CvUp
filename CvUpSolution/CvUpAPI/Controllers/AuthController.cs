using AuthLibrary;
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

        //[HttpPost]
        //[Route("refresh")]
        //public IActionResult Refresh(RefreshTokenModel data)
        //{
        //    if (data is null)
        //        return BadRequest("Invalid client request");

        //    string accessToken = data.token;
        //    string refreshToken = data.refreshToken;

        //    Request.Headers.TryGetValue("Authorization", out var headerValue);
        //    string jwt = headerValue.ToString();

        //    var principal = _authServise.GetPrincipalFromExpiredToken(jwt.Substring(7));

        //    //var principal = _authServise.GetPrincipalFromExpiredToken(accessToken);
        //    //var username = principal.Identity.Name; //this is mapped to the Name claim by default
        //    var user = _userContext.LoginModels.SingleOrDefault(u => u.UserName == username);

        //    if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        //        return BadRequest("Invalid client request");

        //    var newAccessToken = _authServise.GenerateAccessToken(principal.Claims);
        //    var newRefreshToken = _authServise.GenerateRefreshToken();
        //    user.RefreshToken = newRefreshToken;
        //    _userContext.SaveChanges();

        //    return Ok(new AuthenticatedResponse()
        //    {
        //        Token = newAccessToken,
        //        RefreshToken = newRefreshToken
        //    });
        //}

    }
}
