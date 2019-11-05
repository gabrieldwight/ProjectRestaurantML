using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using RestaurantRecommender.MLCommon;

namespace RestaurantRecommender.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly PredictionEnginePool<RestaurantRating, RestaurantPrediction> _machine_model;

        public RestaurantController(PredictionEnginePool<RestaurantRating, RestaurantPrediction> machine_model)
        {
            _machine_model = machine_model;
        }

        public ActionResult Recommend_Restaurant(int id)
        {
            
        }
    }
}