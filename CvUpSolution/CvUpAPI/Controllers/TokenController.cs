using AuthLibrary;
using DataModelsLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {

        //private readonly UserContext _userContext;
        //private readonly ITokenService _tokenService;
        //public TokenController(UserContext userContext, ITokenService tokenService)
        //{
        //    this._userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        //    this._tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        //}

        //private IAuthServise _authServise;
        //public TokenController(IAuthServise authServise)
        //{
        //    _authServise = authServise;
        //}

        //[HttpPost]
        //[Route("refresh")]
        //public IActionResult Refresh(RefreshTokenModel data)
        //{
        //    if (data is null)
        //        return BadRequest("Invalid client request");

        //    string accessToken = data.token;
        //    string refreshToken = data.refreshToken;
        //    var principal = _authServise.GetPrincipalFromExpiredToken(accessToken);
        //    var username = principal.Identity.Name; //this is mapped to the Name claim by default
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

        //[HttpPost, Authorize]
        //[Route("revoke")]
        //public IActionResult Revoke()
        //{
        //    var username = User.Identity.Name;
        //    var user = _userContext.LoginModels.SingleOrDefault(u => u.UserName == username);
        //    if (user == null) return BadRequest();
        //    user.RefreshToken = null;
        //    _userContext.SaveChanges();
        //    return NoContent();
        //}

    }
}
