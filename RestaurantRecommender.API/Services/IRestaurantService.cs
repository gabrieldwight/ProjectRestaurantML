using RestaurantRecommender.MLCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantRecommender.API.Services
{
    public interface IRestaurantService
    {
        Restaurant Get_Restaurant_Detail(int id);
        List<Restaurant> Get_All_Restaurants();
        string Trained_MachineLearning_ModelPath();
        List<Restaurant> PredictRestaurantList ();

        List<Restaurant> GetBestRestaurants { get; }
    }
}
