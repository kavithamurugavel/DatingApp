using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        // testing to see if user id matches the liker id and recepient id matches the likee id
        public async Task<Like> GetLike(int userID, int recipientID)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerID == userID && u.LikeeID == recipientID);
        }

        public async Task<Photo> GetMainPhotoForUser(int userID)
        {
            return await _context.Photos.Where(u => u.UserID == userID)
                    .FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(e => e.ID == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.ID == id);

            return user;
        }

        // action updated for pagination in section 14 lecture 139
        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            // previously we had toListAsync in the foll. line, but due to the pagination code addition, this has been deferred to the PagedList class
            // ASQueryable: https://weblogs.asp.net/zeeshanhirani/using-asqueryable-with-linq-to-objects-and-linq-to-sql
            // https://stackoverflow.com/questions/17366907/what-is-the-purpose-of-asqueryable
            var users = _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();

            users = users.Where(u => u.ID != userParams.UserID); // this would filter out the current logged in user from their matches list

            users = users.Where(u => u.Gender == userParams.Gender); // this would filter out users acc.to gender

            // filtering only the users that either have a liker or a likee id from the likes table
            if(userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserID, userParams.Likers); // it is fine to use userParams.likers here,
                // since this is anyway a boolean that gets sent to GetUserLikes
                users = users.Where(u => userLikers.Contains(u.ID));
            }

            if(userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserID, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.ID));
            }

            // if user customizes the filter for age
            if(userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                // since we have to calculate the age using date of birth for minDOB, we are subtracting maxAge from today
                // the -1 is because we are asking for maximum year of a person
                // That is, for eg, if user searches with maxAge as 30 on Jan 18 2019, then AddYears(-maxAge) will give Jan 18, 1989. But
                // suppose someone turns 31 tomorrow i.e. on Jan 19, 2019, he should still show up in the user's search of maxAge 30.
                // Which means the DOB of that person would be Jan 19 1988. That's why we have -1 in the formula. Basically since a person is
                // the same age for 364 days, we include the -1.
                // https://stackoverflow.com/questions/9/how-do-i-calculate-someones-age-in-c
                var minDOB = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDOB = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDOB && u.DateOfBirth <= maxDOB);
            }

            // if user wants to see the results in a specific order
            if(!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch(userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }
            
            // since CreateAsync is a static class, we can call it directly as below
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        // id is the currently logged in user's id
        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users.Include(x => x.Likers)
            .Include(x => x.Likees).FirstOrDefaultAsync(u => u.ID == id);

            // returning the user ids that have liked the currently logged in user
            // i.e. returning list of liker user ids of the currently logged in (likee) user
            if(likers)
            {
                return user.Likers.Where(u => u.LikeeID == id).Select(i => i.LikerID);
            }
            // returning the likees of the currently logged in user
            else
            {
                return user.Likees.Where(u => u.LikerID == id).Select(i => i.LikeeID);
            }
        }

        public async Task<bool> SaveAll()
        {
            // the 0 is the no. of changes saved to the db
            return await _context.SaveChangesAsync() > 0;
        }
    }
}