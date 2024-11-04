using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Ecommerce.DataAccess;
using Ecommerce.Domain.Models;
using Ecommerce.Service.Interfaces;

namespace Ecommerce.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHashService _hashService;
        private readonly JwtConfig _jwtConfig;
        private readonly EcommerceContext _context;

        public AuthService(IHashService hashService, IOptions<JwtConfig> jwtConfig, EcommerceContext context)
        {
            _hashService = hashService;
            _jwtConfig = jwtConfig.Value;
            _context = context;
        }

        public void RegisterUser(string username, string password)
        {
            if (_context.Users.Any(u => u.Username == username))
            {
                throw new ArgumentException("Username already exists.");
            }

            var hashedPassword = _hashService.HashPassword(password);
            var user = new User
            {
                Username = username,
                PasswordHash = hashedPassword
            };
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public bool Authenticate(string username, string password, out string token)
        {
            token = string.Empty;
            var user = _context.Users.SingleOrDefault(u => u.Username == username);

            if (user != null && _hashService.VerifyPassword(user.PasswordHash, password))
            {
                token = GenerateJwtToken(user.Username);
                return true;
            }

            return false;
        }

        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
