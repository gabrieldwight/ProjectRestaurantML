using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantRecommender.MLCommon
{
    public class UserProfile
    {
        public int UserId { get; set; }
        public string UserImage { get; set; }
        public List<(int restaurantId, int restaurantRating)> UserRestaurantRatings { get; set; }
    }
}
