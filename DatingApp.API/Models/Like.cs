namespace DatingApp.API.Models
{
    // Basically creating a table of Like with Likee and Liker IDs, which in turn
    // need foreign key references to the User table to get the IDs of the liker and likee.
    // Hence the User Liker and User Likee variables below
    public class Like
    {
        public int LikerID { get; set; } // id of user liking another user
        public int LikeeID { get; set; } // id of user being liked by another user
        public User Liker { get; set; }
        public User Likee { get; set; }

    }
}