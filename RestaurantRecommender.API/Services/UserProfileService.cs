using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestaurantRecommender.MLCommon;

namespace RestaurantRecommender.API.Services
{
    public class UserProfileService : IUserProfileService
    {
        private List<UserProfile> _users = new List<UserProfile>();
        // Encapsulating the list of loaded users from the comma separated value file
        public List<UserProfile> GetUsers
        {
            get
            {
                return _users;
            }
        }

        // Accessing simulated user details
        public UserProfile Get_User_details(int id)
        {
            foreach (UserProfile userProfile in _users)
            {
                if (id.Equals(userProfile.UserId))
                {
                    return userProfile;
                }
            }
            return null;
        }

        // Getting a list of restaurants visited by the user
        public List<(int restaurantId, int restaurantRating)> Get_User_Visited_Restaurants(int id)
        {
            foreach (UserProfile userProfile in _users)
            {
                if (id.Equals(userProfile.UserId))
                {
                    return userProfile.UserRestaurantRatings;
                }
            }
            return null;
        }
    }
}
