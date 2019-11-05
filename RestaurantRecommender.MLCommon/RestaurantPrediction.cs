using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantRecommender.MLCommon
{
    public class RestaurantPrediction
    {
        public float Label;

        [ColumnName("Score")]
        public float PredictedRating;
    }
}
