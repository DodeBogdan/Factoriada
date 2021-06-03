using System;
using System.IO;
using System.Threading.Tasks;
using Factoriada.Models;
using Firebase.Database;
using Firebase.Storage;
using Xamarin.Forms;

namespace Factoriada.Services
{
    public class ApiStorageService
    {
        private static ApiStorageService _ServiceClientInstance;
        private readonly FirebaseStorage _firebase;
        public static ApiStorageService ServiceClientInstance
        {
            get
            {
                if (_ServiceClientInstance == null)
                    _ServiceClientInstance = new ApiStorageService();

                return _ServiceClientInstance;
            }
        }

        private ApiStorageService()
        {
            _firebase = new FirebaseStorage("factoriada-40e0f.appspot.com");
        }
    }
}