using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantRecommender.Services.Helper
{
    public static class NamedHttpClients
    {
        // Machine Learning for restaurant recommendation
        public const string Recommend_RestaurantClient = "Recommend_RestaurantClient";
        public const string Recommend_RestaurantTypeClient = "Recommend_RestaurantTypeClient";
        public const string Restaurant_VisitedClient = "Restaurant_VisitedClient";
        public const string UserListClient = "UserListClient";
        public const string RestaurantListClient = "RestaurantListClient";
    }
}
