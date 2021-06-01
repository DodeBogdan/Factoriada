﻿using Factoriada.Exceptions;
using Factoriada.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Factoriada.Services
{
    public class ApiService
    {
        private static ApiService _ServiceClientInstance;
        readonly FirebaseClient _firebase;
        public static ApiService ServiceClientInstance
        {
            get
            {
                if (_ServiceClientInstance == null)
                    _ServiceClientInstance = new ApiService();

                return _ServiceClientInstance;
            }
        }

        private ApiService()
        {
            _firebase = new FirebaseClient("https://factoriada-40e0f-default-rtdb.firebaseio.com/");

            Initialize();
        }

        private async void Initialize()
        {
            var result = (await _firebase
                .Child("User")
                .OnceAsync<User>()).FirstOrDefault(x => x.Object.Email == "admin");

            if (result != null)
                return;

            var id = Guid.NewGuid();

            await _firebase
                 .Child("Role")
                 .Child(id.ToString())
                 .PutAsync(
                 new Role()
                 {
                     RoleId = id,
                     RoleTypeName = "Admin"
                 });

            id = Guid.NewGuid();

            await _firebase
               .Child("Role")
               .Child(id.ToString())
               .PutAsync(
                new Role()
                {
                    RoleId = id,
                    RoleTypeName = "Owner"
                });

            id = Guid.NewGuid();

            await _firebase
              .Child("Role")
              .Child(id.ToString())
              .PutAsync(
              new Role()
              {
                  RoleTypeName = "User",
                  RoleId = id
              });

            id = Guid.NewGuid();

            await _firebase
              .Child("Role")
              .Child(id.ToString())
              .PutAsync(
              new Role()
              {
                  RoleTypeName = "Default",
                  RoleId = id
              });

            var role = (await _firebase
                   .Child("Role")
                   .OnceAsync<Role>()).FirstOrDefault(x => x.Object.RoleTypeName == "Admin")
                ?.Object;

            if (role == null)
                return;

            id = Guid.NewGuid();

            await _firebase
               .Child("User")
               .Child(id.ToString())
               .PutAsync(
               new User()
               {
                   UserId = id,
                   Email = "admin",
                   Password = "admin",
                   Role = role
               });
        }

        public async Task<User> Login(string email)
        {
            var result = (await _firebase
                .Child("User")
                .OnceAsync<User>())
                .FirstOrDefault(x => x.Object.Email == email);

            return result?.Object;    
        }

        public async Task Register(User user)
        {
            user.UserId = Guid.NewGuid();

            user.Role = (await _firebase
                .Child("Role")
                .OnceAsync<Role>())
                .FirstOrDefault(x => x.Object.RoleTypeName == "Default")
                ?.Object;

            await _firebase
                .Child("User")
                .Child(user.UserId.ToString())
                .PutAsync(user);
        }

        public async Task UpdateUser(User currentUser)
        {
            await _firebase
                .Child("User")
                .Child(currentUser.UserId.ToString())
                .PutAsync(currentUser);
        }

        public async Task SaveApartment(ApartmentDetail currentApartment)
        {
            await ChangeUserRoleTo(currentApartment.Owner, "Owner");

            currentApartment.Owner = await _firebase
                .Child("User")
                .Child(currentApartment.Owner.UserId.ToString())
                .OnceSingleAsync<User>();

            await _firebase
                .Child("ApartmentDetail")
                .Child(currentApartment.ApartmentDetailId.ToString)
                .PutAsync(currentApartment);
        }

        private async Task ChangeUserRoleTo(User user, string role)
        {
            var result = (await _firebase
                .Child("Role")
                .OnceAsync<Role>()).FirstOrDefault(x
                => x.Object.RoleTypeName == role)
                ?.Object;

            user.Role = result;

            await _firebase.Child("User")
                .Child(user.UserId.ToString())
                .PutAsync(user);
        }

        public async Task<ApartmentDetail> GetApartmentByToken(string result)
        {
            return (await _firebase
                .Child("ApartmentDetail")
                .OnceAsync<ApartmentDetail>())
                .FirstOrDefault(x => x.Object.Token == result)
                ?.Object;
        }

        public async void JoinApartment(Apartment apartment)
        {
            await ChangeUserRoleTo(apartment.User, "User");

            apartment.User = await _firebase
                .Child("User")
                .Child(apartment.User.UserId.ToString())
                .OnceSingleAsync<User>();

            await _firebase
                .Child("Apartment")
                .Child(apartment.ApartmentId.ToString())
                .PutAsync(apartment);
        }
    }
}
