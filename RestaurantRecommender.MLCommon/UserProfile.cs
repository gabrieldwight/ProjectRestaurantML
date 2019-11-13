using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantRecommender.MLCommon
{
    public class UserProfile
    {
        public int UserId { get; set; }
        public string UserImage { get; set; }
        public int restaurantId { get; set; }
        public int restaurantRating { get; set; }
    }
}
