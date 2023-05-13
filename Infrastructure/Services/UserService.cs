using Application.Interface.Services;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly List<User> _users = new List<User>
    {
        new User { Id = 1, Username = "user1", Password = "password1" },
        new User { Id = 2, Username = "user2", Password = "password2" }
    };

        public User Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(u => u.Username == username && u.Password == password);
            return user;
        }
    }
}
