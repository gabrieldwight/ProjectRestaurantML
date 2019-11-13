using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using RestaurantRecommender.MLCommon;

namespace RestaurantRecommender.API.Services
{
    public class RestaurantService : IRestaurantService
    {
        public Lazy<List<Restaurant>> _restaurants = new Lazy<List<Restaurant>>(LoadRestaurantData);
        public static readonly string _machinelearning_modelpath = "Restaurant_RecommenderModel.zip";
        private static readonly string _restaurant_base_fileName = "SampleRestaurantList.csv";
        private static IHostingEnvironment _hostingEnvironment;
        private static readonly Random random = new Random();
        // Constructor to initialize dependency injection for IHosting service
        public RestaurantService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        // List property to store the best restaurants
        public List<Restaurant> GetBestRestaurants
        {
            get
            {
               return LoadBestRestaurants();
            }
        }

        // To return a list of restaurants based on the filter specified by the user
        public List<Restaurant> GetBestRestaurantByType(string type)
        {
            return _restaurants.Value.Where(x => x.RestaurantType.Equals(type, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => random.Next()).Take(7).ToList();
        }

        // The method used to load any of 7 restaurants that can be used to display the best restaurant
        public List<Restaurant> LoadBestRestaurants()
        {
            return _restaurants.Value.OrderBy(x => random.Next()).Take(7).ToList();
        }

        // To return a list of all the restaurants created 
        public List<Restaurant> Get_All_Restaurants()
        {
            return _restaurants.Value;
        }

        // To return the details of the restaurant accessed
        public Restaurant Get_Restaurant_Detail(int id)
        {
            return _restaurants.Value.SingleOrDefault(r => r.RestaurantId == id);
        }

        // To get the path of the trained machine learning model
        public string Trained_MachineLearning_ModelPath()
        {
            return Path.Combine(_hostingEnvironment.ContentRootPath, "Models", _machinelearning_modelpath);
        }

        // To be applied in lazy loading of all the restaurants data
        private static List<Restaurant> LoadRestaurantData()
        {
            List<Restaurant> restaurant_results = new List<Restaurant>();
            // reading the restaurant csv file
            if (!File.Exists(Path.Combine(_hostingEnvironment.ContentRootPath, "Content", _restaurant_base_fileName)))
            {
                throw new FileNotFoundException();
            }
            else
            {
                using (var reader = new StreamReader(Path.Combine(_hostingEnvironment.ContentRootPath, "Content", _restaurant_base_fileName)))
                {
                    string headerLine = reader.ReadLine();
                    string line;
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        var values = line.Split(",");

                        restaurant_results.Add(new Restaurant()
                        {
                            RestaurantId = Convert.ToInt32(values[0]),
                            RestaurantName = values[1],
                            RestaurantType = values[2]
                        });
                    }
                }
            }
            return restaurant_results;
        }
    }
}
