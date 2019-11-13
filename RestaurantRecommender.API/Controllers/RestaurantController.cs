using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using Microsoft.ML;
using RestaurantRecommender.API.Services;
using RestaurantRecommender.MLCommon;
using RestaurantRecommender.MLCommon.ViewModel;

namespace RestaurantRecommender.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IUserProfileService _userProfileService;
        private readonly ILogger<RestaurantController> _logger;
        private readonly PredictionEnginePool<RestaurantRating, RestaurantPrediction> _machine_model;
        private readonly MLContext _mlContext;

        public RestaurantController(MLContext mlContext, PredictionEnginePool<RestaurantRating, RestaurantPrediction> machine_model, ILogger<RestaurantController> logger, IRestaurantService restaurantService, IUserProfileService userProfileService)
        {
            _mlContext = mlContext;
            _machine_model = machine_model;
            _logger = logger;
            _restaurantService = restaurantService;
            _userProfileService = userProfileService;
        }

        // Returning a list of recommended restaurants
        //api/Restaurant/recommend/(id)
        [HttpGet("recommend/{id}")]
        public IActionResult Recommend_Restaurant([FromRoute] int id)
        {
            var activeuser = _userProfileService.Get_User_details(id);

            List<(int RestaurantId, float score)> ratings = new List<(int restaurantId, float score)>();

            RestaurantPrediction restaurantPrediction = null;
            List<Restaurant> Best_Restaurants = _restaurantService.GetBestRestaurants;
            foreach (var restaurant in Best_Restaurants)
            {
                // To determine the possible results of visiting the best restaurants generated
                restaurantPrediction = _machine_model.GetPredictionEngine("Restaurant_Recommendation_Model").Predict(new RestaurantRating
                {
                    userId = id,
                    restaurantId = restaurant.RestaurantId
                });

                // Using a range of between 1 and 5 to predict the possible results outcome
                float _score = (float)Math.Round(restaurantPrediction.PredictedRating, 1);

                // using the _score to create a recommendation for each restaurant in the best restaurant list
                ratings.Add((restaurant.RestaurantId, _score));
            }

            var results = new List<Restaurant_Recommendation_Results>();
            foreach (var item in ratings)
            {
                results.Add(new Restaurant_Recommendation_Results()
                {
                    Restaurant_Name = Best_Restaurants
                    .FirstOrDefault(x => x.RestaurantId == item.RestaurantId).RestaurantName,
                    Restaurant_Type = Best_Restaurants
                    .FirstOrDefault(x => x.RestaurantId == item.RestaurantId).RestaurantType,
                    Restaurant_Rating = item.score
                });
            }
            return Ok(results);
        }

        // Returning a list of restaurants based on the type specified
        //api/Restaurant/recommended_type?(type=x)&(id=x)
        [HttpGet("recommended_type")]
        public IActionResult Recommended_Restaurant_Type([FromQuery] string type, [FromQuery] int id)
        {
            List<(int RestaurantId, float score)> ratings = new List<(int restaurantId, float score)>();

            RestaurantPrediction restaurantPrediction = null;
            List<Restaurant> Best_Restaurants = _restaurantService.GetBestRestaurantByType(type);
            foreach (var restaurant in Best_Restaurants)
            {
                // To determine the possible results of visiting the best restaurants generated
                restaurantPrediction = _machine_model.GetPredictionEngine("Restaurant_Recommendation_Model").Predict(new RestaurantRating
                {
                    userId = id,
                    restaurantId = restaurant.RestaurantId
                });

                // Using a range of between 1 and 5 to predict the possible results outcome
                float _score = (float)Math.Round(restaurantPrediction.PredictedRating, 1);

                // using the _score to create a recommendation for each restaurant in the best restaurant list
                ratings.Add((restaurant.RestaurantId, _score));
            }

            var results = new List<Restaurant_Recommendation_Results>();
            foreach (var item in ratings)
            {
                results.Add(new Restaurant_Recommendation_Results()
                {
                    Restaurant_Name = Best_Restaurants
                    .FirstOrDefault(x => x.RestaurantId == item.RestaurantId).RestaurantName,
                    Restaurant_Type = Best_Restaurants
                    .FirstOrDefault(x => x.RestaurantId == item.RestaurantId).RestaurantType,
                    Restaurant_Rating = item.score
                });
            }
            return Ok(results);
        }

        // Returning a list of restaurants visited by the user
        //api/Restaurant/visited/(id)
        [HttpGet("visited/{id}")]
        public IActionResult Visited_Restaurant([FromRoute] int id)
        {
            var RestaurantRatings = _userProfileService.Get_User_Visited_Restaurants(id);
            List<Restaurant> VisitedRestuarants = new List<Restaurant>();

            foreach (var item in RestaurantRatings)
            {
                VisitedRestuarants.Add(_restaurantService.Get_Restaurant_Detail(item.restaurantId));
            }

            return Ok(VisitedRestuarants);
        }

        // Returning a list of users
        //api/Restaurant/users
        [HttpGet("users")]
        public IActionResult User_Profiles()
        {
            return Ok(_userProfileService.GetUsers);
        }

        // Returning a list of restaurants
        //api/Restaurant/list
        [HttpGet("list")]
        public IActionResult Restaurant_List()
        {
            return Ok(_restaurantService.Get_All_Restaurants());
        }
    }
}