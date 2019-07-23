using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthJWT.Config;
using AuthJWT.Config.Permissions;
using AuthJWT.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthJWT.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly List<User> _users = new List<User>
        {
            new User
            {
                Id = 1, FirstName = "moein", LastName = "fazeli", Username = "user1", Password = "1234",
                Permissions = new string[] {"Permissions.User.GetAll"}, Role = "Admin"
            },
            new User
            {
                Id = 2, FirstName = "hassan", LastName = "saeedi", Username = "user2", Password = "1234",
                Permissions = new string[] {"Permissions.User.CheckUser"}, Role = "User"
            }
        };

        private readonly List<Role> _roles = new List<Role>
        {
            new Role
            {
                Name = "Admin", Permissions = new string[] {"Permissions.User.CheckUser", "Permissions.User.GetAll"}
            },
            new Role
            {
                Name = "User", Permissions = new string[] {"Permissions.User.CheckUser"}
            }
        };

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public User Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var claims = new ClaimsIdentity();
            foreach (var permission in user.Permissions)
            {
                claims.AddClaims(new[]
                {
                    new Claim(Permissions.Permission, permission)
                });
            }

            foreach (var rolePermission in _roles.Find(role=> role.Name == user.Role).Permissions)
            {
                if (!user.Permissions.Any(x => x == rolePermission))
                {
                    claims.AddClaims(new[]
                    {
                        new Claim(Permissions.Permission, rolePermission)
                    });
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            //user.Password = null;

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _users.ToList();
        }
    }
}