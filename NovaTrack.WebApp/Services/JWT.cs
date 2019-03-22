using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NovaTrack.WebApp.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NovaTrack.WebApp.Services
{

    public class JWT
    {
        private readonly IConfiguration configuration;

        public JWT(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                // new Claim(JwtRegisteredClaimNames.Email, user.Email),
                // new Claim(ClaimTypes.Email, user.Email),
            };

            // claims.AddRange(user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(configuration["Jwt:ExpireDays"]));
            //var expires = DateTime.Now.AddYears(5);

            var token = new JwtSecurityToken(
                 configuration["Jwt:Issuer"],
                 configuration["Jwt:Issuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
