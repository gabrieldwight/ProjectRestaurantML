using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantRecommender.MLCommon
{
    public class RestaurantRating
    {
        [LoadColumn(0)]
        public float userId;

        [LoadColumn(1)]
        public float restaurantId;

        [LoadColumn(2)]
        public float Label;
    }
}
