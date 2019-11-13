using RestaurantRecommender.MLCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantRecommender.API.Services
{
    public interface IUserProfileService
    {
        UserProfile Get_User_details(int id);
        List<UserProfile> Get_User_Visited_Restaurants(int id);
        List<UserProfile> GetUsers { get; }
    }
}
