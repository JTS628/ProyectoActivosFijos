using Firebase.Auth.Providers;
using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoActivoFijo.Controllers
{
    public class FireBaseController
    {
        const string FB_API_KEY = "AIzaSyDrPHhCEFEHDVhqRBXXGdPeWYaWdhwYIRI";
        const string FB_DOMAIN = "dotnet-activos-fijos.firebaseapp.com";

        FirebaseAuthConfig fbConfig = new FirebaseAuthConfig
        {
            ApiKey = FB_API_KEY,
            AuthDomain = FB_DOMAIN,
            Providers = new FirebaseAuthProvider[]
            {
                // Add and configure individual providers
                // new GoogleProvider().AddScopes("email"),
                new EmailProvider()
                // ...
            },
            //// WPF:
            //UserRepository = new FileUserRepository("FirebaseSample") // persist data into %AppData%\FirebaseSample
            //// UWP: 
            //UserRepository = new StorageRepository() // persist data into ApplicationDataContainer
        };


        public async Task<UserCredential> SignInWithEmailAndPassowrd(string username, string password)
        {
            try
            {
                FirebaseAuthClient client = new FirebaseAuthClient(fbConfig);
                UserCredential userCredential = await client.SignInWithEmailAndPasswordAsync(username, password);
                return userCredential;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
