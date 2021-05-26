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

            var role = (await _firebase
                   .Child("Role")
                   .OnceAsync<Role>()).FirstOrDefault(x => x.Object.RoleTypeName == "Admin").Object;

            if (role == null)
                return;

            _ = await _firebase
               .Child("AdminUser")
               .PostAsync(
               new User()
               {
                   UserId = Guid.NewGuid(),
                   Email = "admin",
                   Password = "admin",
                   Role = role
               });
        }
    }
}
