using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using RestaurantRecommender.MLCommon;

namespace RestaurantRecommender.API.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private static readonly string _restaurant_visited_base_fileName = "Restaurant_Visited_By_Users.csv";
        private List<UserProfile> _users = new List<UserProfile>();
        
        public UserProfileService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _users = LoadUserDataSample();
        }
        
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
        public List<UserProfile> Get_User_Visited_Restaurants(int id)
        {
            return _users.Where(x => x.UserId == id)
                .Select(x => new UserProfile()
                {
                    restaurantId = x.restaurantId,
                    restaurantRating = x.restaurantRating
                })
                .ToList();
        }

        // Generated random restaurants visited by the user
        public List<UserProfile> LoadUserDataSample()
        {
            List<UserProfile> data_results = new List<UserProfile>();
            if (!File.Exists(Path.Combine(_hostingEnvironment.ContentRootPath, "Content", _restaurant_visited_base_fileName)))
            {
                throw new FileNotFoundException();
            }
            else
            {
                using (var reader = new StreamReader(Path.Combine(_hostingEnvironment.ContentRootPath, "Content", _restaurant_visited_base_fileName)))
                {
                    string headerLine = reader.ReadLine();
                    string line;
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        var values = line.Split(",");

                        data_results.Add(new UserProfile
                        {
                            UserId = Convert.ToInt32(values[0]),
                            UserImage = "https://avatars.dicebear.com/v2/avataaars/seed.svg?options[mouth][]=smile",
                            restaurantId = Convert.ToInt32(values[1]),
                            restaurantRating = Convert.ToInt32(values[2])
                        });
                    }
                }
            }
            return data_results;
        }
    }
}
