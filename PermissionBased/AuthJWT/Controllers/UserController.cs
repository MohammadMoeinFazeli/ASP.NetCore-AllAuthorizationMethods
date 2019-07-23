using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthJWT.Config.Permissions;
using AuthJWT.DtoModels;
using AuthJWT.Entities;
using AuthJWT.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthJWT.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public User Login([FromBody] UserDto user)
        {
            var User = _userService.Authenticate(user.Username, user.Password);

            return User;
        }

        [PermissionAuthorize(Permissions.User.GetAll)]
        [HttpGet("all")]
        public IEnumerable<User> GetAllUser ()
        {
            return _userService.GetAll();
        }

        [PermissionAuthorize(Permissions.User.CheckUser)]
        [HttpGet("checkUser")]
        public string CheckUser()
        {
            return "Hello You have the right access";
        }
    }
}
