using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ServicesLibrary.Authentication
{
    public class TokenService : ITokenService
    {
        //public string GenerateAccessToken(IEnumerable<Claim> claims)
        //{
        //    var claims = new[] {
        //                        //new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
        //                        //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //                        //new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
        //                        new Claim("UserId", authenticateUser.id.ToString()),
        //                        new Claim("CompanyId", authenticateUser.company_id.ToString()),
        //                        new Claim("DisplayName", string.Format("{0} {1}",authenticateUser.first_name,authenticateUser.last_name)),
        //                        new Claim("email", authenticateUser.email),
        //                        new Claim("role",Enum.GetName(typeof(UsersRole), authenticateUser.role)?? ""),
        //                    };

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //    var signinCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //    var token = new JwtSecurityToken(
        //        issuer: _configuration["Jwt:Issuer"],
        //        audience: _configuration["Jwt:Audience"],
        //        claims,
        //        expires: DateTime.UtcNow.AddMinutes(10),
        //        signingCredentials: signinCredentials
        //    );

        //    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
        //    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        //    var tokeOptions = new JwtSecurityToken(
        //        issuer: "https://localhost:5001",
        //        audience: "https://localhost:5001",
        //        claims: claims,
        //        expires: DateTime.Now.AddMinutes(5),
        //        signingCredentials: signinCredentials
        //    );
        //    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        //    return tokenString;
        //}

        //public string GenerateRefreshToken()
        //{
        //    var randomNumber = new byte[32];
        //    using (var rng = RandomNumberGenerator.Create())
        //    {
        //        rng.GetBytes(randomNumber);
        //        return Convert.ToBase64String(randomNumber);
        //    }
        //}
        //public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        //{
        //    var tokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
        //        ValidateIssuer = false,
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345")),
        //        ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
        //    };
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    SecurityToken securityToken;
        //    var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        //    var jwtSecurityToken = securityToken as JwtSecurityToken;
        //    if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        //        throw new SecurityTokenException("Invalid token");
        //    return principal;
        //}
    }
}
