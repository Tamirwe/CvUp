using Database.models;
using DataModelsLibrary.Enums;
using DataModelsLibrary.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthLibrary
{
    public partial class AuthServise
    {
        public async Task<TokenModel?> CheckUpdateRefreshToken(string token, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            bool isUserId = int.TryParse(principal.Claims.Where(x => x.Type == "UserId").First().Value, out var userId);
            bool iscompanyId = int.TryParse(principal.Claims.Where(x => x.Type == "CompanyId").First().Value, out var companyId);

            if (isUserId && iscompanyId)
            {
                var userRefreshToken = await _authQueries.GetUserRefreshTokens(companyId, userId, refreshToken);

                if (userRefreshToken != null && userRefreshToken.token == refreshToken &&
                    userRefreshToken.token_expire != null
                    && userRefreshToken.token_expire > DateTime.Now)
                {
                    var newToken = GenerateUserToken(principal.Claims);
                    var newRefreshToken = GenerateRefreshToken();
                    userRefreshToken.token = newToken;
                    int RefreshTokenHoursExpiration = Convert.ToInt32(_config["Jwt:RefreshTokenHoursExpiration"]);
                    userRefreshToken.token_expire = DateTime.Now.AddHours(RefreshTokenHoursExpiration);
                    await _authQueries.UPdateRefreshToken(userRefreshToken);
                    return new TokenModel { token = newToken, refreshToken = newRefreshToken };
                }
            }

            return null;
        }

        private string GenerateUserToken(IEnumerable<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));

            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }

        public async Task<TokenModel> GenerateAccessToken(user authenticateUser, bool isRemember)
        {
            var claims = new[] {
                                //new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                                //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                //new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                                new Claim("UserId", authenticateUser.id.ToString()),
                                new Claim("CompanyId", authenticateUser.company_id.ToString()),
                                new Claim("DisplayName", string.Format("{0} {1}",authenticateUser.first_name,authenticateUser.last_name)),
                                new Claim("email", authenticateUser.email),
                                new Claim("role",Enum.Parse<UserPermission>(authenticateUser.permission_type).ToString()),
                            };

            var newToken = GenerateUserToken(claims);
            string newRefreshToken = "";

            if (isRemember)
            {
                newRefreshToken = GenerateRefreshToken();
                await _authQueries.DeleteExpiredTokens();
                int RefreshTokenHoursExpiration= Convert.ToInt32(_config["Jwt:RefreshTokenHoursExpiration"]);
                await _authQueries.AddUserRefreshToken(authenticateUser.company_id, authenticateUser.id, newRefreshToken, RefreshTokenHoursExpiration);
            }

            return new TokenModel { token = newToken, refreshToken = newRefreshToken };
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"])),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        public async Task RevokeUser(int companyId, int userId)
        {
            await _authQueries.RevokeUser( companyId,  userId);
        }
    }
}
