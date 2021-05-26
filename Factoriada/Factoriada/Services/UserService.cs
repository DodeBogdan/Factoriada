using Factoriada.Exceptions;
using Factoriada.Models;
using Factoriada.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Factoriada.Services
{
    class UserService : IUserService
    {
        public async Task<User> LogIn(string email, string password)
        {
            var result =  await ApiService.ServiceClientInstance.Login(email, password);

            if (result == null)
                throw new UserException("Nu exista nici un user cu acest email.");

            if (result.Password != password)
                throw new UserException("Email-ul si parola nu corespund.");

            return result;
        }

        public async Task Register(User user)
        {
            TestUser(user);

            await ApiService.ServiceClientInstance.Register(user);
        }

        public void TestUser(User user)
        {
            if (user.FirstName == null) throw new UserException("Prenumele trebuie sa fie completat.");

            if (user.FirstName.Length < 3 || user.FirstName.Length > 50)
                throw new UserException("Prenumele trebuie sa aiba minim 3 caractere sau maxim 50.");

            if (!user.FirstName.All(a => char.IsLetter(a) || char.IsWhiteSpace(a) || a == '-'))
                throw new UserException("Prenumele nu trebuie sa aiba caractere invalide.");

            if (user.LastName == null) throw new UserException("Numele trebuie sa fie completat.");

            if (user.LastName.Length < 3 || user.LastName.Length > 50)
                throw new UserException("Numele trebuie sa aiba minim 3 caractere sau maxim 50.");

            if (!user.LastName.All(a => char.IsLetter(a) || char.IsWhiteSpace(a) || a == '-'))
                throw new UserException("Numele nu trebuie sa aiba caractere invalide.");

            var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w)+)+)$");

            if (regex.Match(user.Email) == Match.Empty || user.Email.Length < 4 || user.Email.Length > 50)
                throw new UserException("Email-ul este invalid.");

            if (user.Password == null) throw new UserException("Parola este invalida.");

            if (user.Password.Length < 8 || user.Password.Length > 50)
                throw new UserException("Parola trebuie sa contina cel putin 8 caractere si maxim 50.");
        }
    }
}
