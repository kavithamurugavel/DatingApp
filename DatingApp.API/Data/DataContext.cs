using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext: DbContext
    {
        // the foll.lines of code are part of normal DBContext code in an ASP.NET Web App, say. Unsure of another way to add this code other than physically type it out in a ASP.NET Web API
        public DataContext(DbContextOptions<DataContext> options): base (options){}

        public DbSet<Value> Values { get; set; }
    }
}