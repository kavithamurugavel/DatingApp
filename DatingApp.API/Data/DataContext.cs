using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext: DbContext
    {
        // the foll.lines of code are part of normal DBContext code in an ASP.NET Web App, say. Unsure of another way to add this code other than physically type it out in a ASP.NET Web API
        public DataContext(DbContextOptions<DataContext> options): base (options){}

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }

        // overriding the OnModelCreating method, based on Fluent API (basically a means to define 
        // custom code-first conventions with EF), for our like functionality Sec 15 Lec 150
        // https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/fluent/types-and-properties
        // https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/conventions/custom
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // making our composite primary key a combination of liker and likee ID, since we 
            // don't want one user liking another user more than once
            builder.Entity<Like>()
            .HasKey(k => new {k.LikerID, k.LikeeID});

            // user can like many users and also be liked by many users
            builder.Entity<Like>()
            .HasOne(u => u.Likee)
            .WithMany(u => u.Likers)
            .HasForeignKey(u => u.LikeeID) // going back to users table to get the user's ID that corresponds to the LikeeID
            .OnDelete(DeleteBehavior.Restrict); // we don't want deletion of a like be a cascading deletion of a user

            builder.Entity<Like>()
            .HasOne(u => u.Liker)
            .WithMany(u => u.Likees)
            .HasForeignKey(u => u.LikerID)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}