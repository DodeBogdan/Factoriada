﻿using Factoriada.Exceptions;
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

            user.Address = new Address()
            {
                AddressId = Guid.NewGuid()
            };

            await ApiService.ServiceClientInstance.Register(user);
        }

        public async Task ChangePassword(User currentUser, string newPassword)
        {
            TestPassword(newPassword);

            currentUser.Password = newPassword;

            await ApiService.ServiceClientInstance.UpdateUser(currentUser);
        }

        public async Task ChangeProfile(User currentUser)
        {
            TestEditUser(currentUser);
            TestPhone(currentUser.PhoneNumber);

            await ApiService.ServiceClientInstance.UpdateUser(currentUser);
        }

        private static void TestPhone(string phone)
        {
            if (phone == null) throw new UserException("Numarul de telefon este invalid.");

            if(phone.Length != 10) throw new UserException("Numarul de telefon este invalid.");
        }

        private static void TestPassword(string password)
        {
            if (password == null) throw new UserException("Parola este invalida.");

            if (password.Length < 8 || password.Length > 50)
                throw new UserException("Parola trebuie sa contina cel putin 8 caractere si maxim 50.");
        }

        private void TestEditUser(User user)
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

            if (user.Email == "admin")
                return;

            var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w)+)+)$");

            if (regex.Match(user.Email) == Match.Empty || user.Email.Length < 4 || user.Email.Length > 50)
                throw new UserException("Email-ul este invalid.");
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

            TestPassword(user.Password);
        }



        public async Task UpdateUserAddress(User currentUser)
        {
            TestAddress(currentUser.Address);

            await ApiService.ServiceClientInstance.UpdateUser(currentUser);
        }

        public async Task SaveProfilePicture(User currentUser)
        {
            await ApiService.ServiceClientInstance.UpdateUser(currentUser);
        }

        private static void TestAddress(Address userAddress)
        {
            if (userAddress.Country == null) throw new AddressException("Tara trebuie sa fie completat.");

            if (userAddress.Country.Length < 3 || userAddress.Country.Length > 50)
                throw new AddressException("Tara trebuie sa aiba minim 3 caractere sau maxim 50.");

            if (userAddress.City == null) throw new AddressException("Orasul trebuie sa fie completat.");

            if (userAddress.City.Length < 3 || userAddress.City.Length > 50)
                throw new AddressException("Orasul trebuie sa aiba minim 3 caractere sau maxim 50.");

            if (userAddress.Street == null) throw new AddressException("Strada trebuie sa fie completat.");

            if (userAddress.Street.Length < 3 || userAddress.Street.Length > 50)
                throw new AddressException("Strada trebuie sa aiba minim 3 caractere sau maxim 50.");
        }
    }
}
