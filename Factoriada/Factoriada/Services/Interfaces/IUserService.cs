using Factoriada.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Factoriada.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> LogIn(string email, string password);
        Task Register(User user);
    }
}
