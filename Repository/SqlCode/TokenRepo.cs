using Account_Management.Models.Domain;
using Account_Management.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Account_Management.Repository.SqlCode
{
    public class TokenRepo : ITokenRepository
    {
        private readonly IConfiguration configuration;

        public TokenRepo(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string CreateJwtToken(User user, List<string> roles)
        {
            // Token creation logic
            //Generates the list of claims
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Role, user.username));
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            //Take the key and credentials to pass to token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            // create a new jwtsecuritytoken and write the token and return it
            var token = new JwtSecurityToken(
                    configuration["Jwt:Issuer"],
                    configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);

            
        }

        
    }
}
