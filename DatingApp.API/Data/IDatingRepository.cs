using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
        // generic because we are either adding user or photo
        // just saving duplicate code for user and photo. Same with delete
         void Add<T>(T entity) where T:class;
         void Delete<T>(T entity) where T:class;
         Task<bool> SaveAll();
         Task<PagedList<User>> GetUsers(UserParams userParams);
         Task<User> GetUser(int id);
         Task<Photo> GetPhoto(int id);
         Task<Photo> GetMainPhotoForUser(int userID);
         Task<Like> GetLike(int userID, int recipientID);
    }
}