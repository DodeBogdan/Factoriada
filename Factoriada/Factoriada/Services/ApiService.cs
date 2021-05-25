using Factoriada.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            await InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            var result = (await _firebase
                .Child("User")
                .OnceAsync<User>()).FirstOrDefault(x => x.Object.Email == "admin");

            if (result != null)
                return;

            await _firebase
                .Child("User")
                .PostAsync(
                new User()
                {
                    Email = "admin",
                    Password = "admin"
                });
        }
    }
}
