using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Clay.Services
{
    public class TokenService
    {
        private readonly string JwtKey;
        private readonly string JwtIssuer;
        public TokenService(string jwtKey, string jwtIssuer)
        {
            JwtKey = jwtKey;
            JwtIssuer = jwtIssuer;
        }

        public string GenerateToken(string username,string id,string rolename)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,username),
                new Claim(ClaimTypes.Role,rolename),
                new Claim(ClaimTypes.NameIdentifier,id) 
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: JwtIssuer,
                audience: JwtIssuer,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public string Authenticate(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            List<Exception> validationFailures = null;
            SecurityToken validatedToken;
            var validator = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters();
            validationParameters.ValidIssuer = JwtIssuer;
            validationParameters.ValidAudience = JwtIssuer;
            validationParameters.IssuerSigningKey = key;
            validationParameters.ValidateIssuerSigningKey = true;
            validationParameters.ValidateAudience = true;

            if (!validator.CanReadToken(token))
                return null;
            ClaimsPrincipal principal;
            try
            {
                principal = validator.ValidateToken(token, validationParameters, out validatedToken);
                if (principal.HasClaim(c => c.Type == ClaimTypes.Name))
                {
                    return principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                }
            }
            catch (Exception e)
            {
                // token is invalid because of exception e
            }

            return null;
        }
    }

}