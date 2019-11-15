using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xamarin.Essentials;

namespace RestaurantRecommender.Services.Helper
{
    public static class URLHelper
    {
#if DEBUG
        //public static string IPAddress = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";
        public static string BaseEndPoint = "https://restaurantrecommenderapi.azurewebsites.net/";
#else
        public const string BaseEndPoint = "https://restaurantrecommenderapi.azurewebsites.net/";
#endif
    }
}
