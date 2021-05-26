using Factoriada.Exceptions;
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

            _ = await _firebase
                 .Child("Role")
                 .PostAsync(
                 new Role()
                 {
                     RoleId = Guid.NewGuid(),
                     RoleTypeName = "Admin"
                 });

            _ = await _firebase
               .Child("Role")
               .PostAsync(
                new Role()
                {
                    RoleId = Guid.NewGuid(),
                    RoleTypeName = "Owner"
                });

            _ = await _firebase
              .Child("Role")
              .PostAsync(
              new Role()
              {
                  RoleTypeName = "User",
                  RoleId = Guid.NewGuid()
              });
            
            _ = await _firebase
              .Child("Role")
              .PostAsync(
              new Role()
              {
                  RoleTypeName = "Default",
                  RoleId = Guid.NewGuid()
              });

            var role = (await _firebase
                   .Child("Role")
                   .OnceAsync<Role>()).FirstOrDefault(x => x.Object.RoleTypeName == "Admin").Object;

            if (role == null)
                return;

            _ = await _firebase
               .Child("User")
               .PostAsync(
               new User()
               {
                   UserId = Guid.NewGuid(),
                   Email = "admin",
                   Password = "admin",
                   Role = role
               });
        }

        public async Task<User> Login(string email, string password)
        {
            var result = (await _firebase
                .Child("User")
                .OnceAsync<User>())
                .FirstOrDefault(x => x.Object.Email == email);

            if (result == null)
                return null;

            return result.Object;    
        }

        public async Task LogAsync(string message)
        {
            _ = await _firebase
                .Child("Log")
                .PostAsync(message);
        }

        public async Task Register(User user)
        {
            user.UserId = Guid.NewGuid();
            user.Role = (await _firebase
                .Child("Role")
                .OnceAsync<Role>())
                .FirstOrDefault(x => x.Object.RoleTypeName == "Default").Object;
                
            _ = await _firebase
                .Child("User")
                .PostAsync(user);
        }
    }
}
