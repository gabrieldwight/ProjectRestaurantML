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
        public List<Restaurant> _restaurants = new List<Restaurant>(LoadRestaurantData());
        public readonly static string _machinelearning_modelpath = @"Restaurant_RecommenderModel.zip";
        private static readonly string restaurant_base_fileName = "SampleRestaurantList.csv";
        private readonly IHostingEnvironment _hostingEnvironment;
        // Constructor to initialize dependency injection for IHosting service
        public RestaurantService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public List<Restaurant> GetBestRestaurants
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public List<Restaurant> Get_All_Restaurants()
        {
            return _restaurants;
        }

        public Restaurant Get_Restaurant_Detail(int id)
        {
            return _restaurants.SingleOrDefault(r => r.RestaurantId == id);
        }

        public List<Restaurant> PredictRestaurantList()
        {
            throw new NotImplementedException();
        }

        public string Trained_MachineLearning_ModelPath()
        {
            return Path.Combine(_hostingEnvironment.ContentRootPath, "Models", _machinelearning_modelpath);
        }

        private List<Restaurant> LoadRestaurantData()
        {
            List<Restaurant> restaurant_results = new List<Restaurant>();
            // reading the restaurant csv file
            if (!File.Exists(Path.Combine(_hostingEnvironment.ContentRootPath, "Content", restaurant_base_fileName)))
            {
                throw new FileNotFoundException();
            }
            else
            {
                using (var reader = new StreamReader(Path.Combine(_hostingEnvironment.ContentRootPath, "Content", restaurant_base_fileName)))
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
