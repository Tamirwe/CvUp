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

        public async Task<TokenModel?> RefreshToken(string token, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            bool isUserId = int.TryParse(principal.Claims.Where(x => x.Type == "UserId").First().Value, out var userId);

            if (isUserId)
            {
                var user = await _authQueries.GetUser(userId);

                if (user != null && user.refresh_token == refreshToken && user.refresh_token_expiry < DateTime.Now)
                {
                    return await GeneratedToken(principal.Claims, user, true);
                }
            }

            return null;
        }

        private async Task<TokenModel> GeneratedToken(IEnumerable<Claim> claims, user authenticateUser, bool isRemember)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            string refreshToken = "";

            if (isRemember)
            {
                refreshToken = GenerateRefreshToken();
                await _authQueries.SaveRefreshToken(refreshToken, authenticateUser);
            }


            return new TokenModel { token = tokenString, refreshToken = refreshToken };
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

            return await GeneratedToken(claims, authenticateUser, isRemember);

            //var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            //var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            //var token = new JwtSecurityToken(
            //    issuer: _configuration["Jwt:Issuer"],
            //    audience: _configuration["Jwt:Audience"],
            //    claims,
            //    expires: DateTime.UtcNow.AddMinutes(1),
            //    signingCredentials: signinCredentials
            //);

            //var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            //var refreshToken = GenerateRefreshToken();
            //_authQueries.SaveRefreshToken(refreshToken, authenticateUser);

            //return new TokenModel { token = tokenString, refreshToken = refreshToken };

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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"])),
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

        public async Task RevokeToken(int userId)
        {
            await _authQueries.RevokeUserToken(userId);
        }
    }
}
