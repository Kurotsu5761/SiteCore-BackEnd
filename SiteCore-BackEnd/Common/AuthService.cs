using Library.Core.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SiteCore_BackEnd.Common
{
    public class AuthService : IAuthService 
    {
        private IUserRepository _userRepository;
        private string SecretKey;

        public AuthService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            SecretKey = config.GetValue<string>("SecretKey");
        }

        public User Authenticate(string username)
        {
            var user = _userRepository.GetUserByUsername(username);

            if (user == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim("EmailAddress", user.EmailAddress),
                    new Claim("IsAdmin", true.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            return user;
        }

        public User Authorize(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var read = tokenHandler.ReadJwtToken(token);

                var id = int.Parse(read.Claims.First(claim => claim.Type == "UserId").Value);
                var emailAddress = read.Claims.First(claim => claim.Type == "EmailAddress").Value;
                var isAdmin = bool.Parse(read.Claims.First(claim => claim.Type == "IsAdmin").Value);

                if ( String.IsNullOrEmpty(emailAddress))
                {
                    return null;
                }

                User user = new User() { EmailAddress = emailAddress, UserId = id, IsAdmin = isAdmin };
                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
