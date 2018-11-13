using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
    // general purpose Extensions class
    // making it static so that we don't create new instances
    public static class Extensions
    {
        // extra extensions class we are creating so that we can include CORS stuff for error handling - Section 5 Lecture 49
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            // the following two lines are for displaying proper error messages on the Angular app without the CORS error problem
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}