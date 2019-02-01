using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    public class Fallback : Controller
    {
        public IActionResult Index()
        {
            // MVC basically redirects our request and sends it back to index.html
            // which is the angular application. It means we are passing off
            // the angular routes/router here so that angular can deal with the url routing
            // for eg: localhost:5000/members will go to the angular app for routing to the
            // members page. 
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot", "index.html"), "text/HTML");
        }
    }
}