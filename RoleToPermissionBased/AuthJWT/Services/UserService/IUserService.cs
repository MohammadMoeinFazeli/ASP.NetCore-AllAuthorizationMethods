using System.Collections.Generic;
using AuthJWT.Entities;

namespace AuthJWT.Services.UserService
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
    }
}
