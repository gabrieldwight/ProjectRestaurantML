using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RestaurantRecommender.MLCommon;
using RestaurantRecommender.MLCommon.ViewModel;
using RestaurantRecommender.Services.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantRecommender.Services.Core
{
    public class RestaurantRecommenderPayLoad
    {
        private HttpClient _client;
        private static readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private static CancellationToken _token = new CancellationToken();
        private JsonSerializer _jsonSerializer = new JsonSerializer();

        public RestaurantRecommenderPayLoad()
        {
            _token = _tokenSource.Token;
        }

        // To return a list of recommended restaurants to user
        public async Task<List<Restaurant_Recommendation_Results>> Create_Restaurant_Recommendation(int id)
        {
            var services = new ServiceCollection();
            services.AddHttpClient(NamedHttpClients.Recommend_RestaurantClient, client =>
            {
                client.BaseAddress = new Uri(URLHelper.BaseEndPoint);
                client.Timeout = TimeSpan.FromMinutes(10);
            });

            var serviceProvider = services.BuildServiceProvider();

            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            _client = httpClientFactory.CreateClient(NamedHttpClients.Recommend_RestaurantClient);

            List<Restaurant_Recommendation_Results> result = new List<Restaurant_Recommendation_Results>();
            var response = await _client.GetAsync($"api/restaurant/recommend/{id}", _token).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                /*await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                {
                    result = JsonConvert.DeserializeObject<List<Restaurant_Recommendation_Results>>(x.Result);
                }, _tokenSource.Token);*/
                await response.Content.ReadAsStreamAsync().ContinueWith((Task<Stream> x) =>
                {
                    using StreamReader reader = new StreamReader(x.Result);
                    using JsonTextReader json = new JsonTextReader(reader);
                    result = _jsonSerializer.Deserialize<List<Restaurant_Recommendation_Results>>(json);
                }, _tokenSource.Token);
                Console.WriteLine("Json result: " + result);
            }
            else
            {
                var content_result = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"{response.StatusCode}:{content_result}");
            }
            return result;
        }

        // Returning a list of recommended restaurants based on the type specified to user
        public async Task<List<Restaurant_Recommendation_Results>> Create_Restaurant_Type_Recommendation(string type, int id)
        {
            var services = new ServiceCollection();
            services.AddHttpClient(NamedHttpClients.Recommend_RestaurantTypeClient, client =>
            {
                client.BaseAddress = new Uri(URLHelper.BaseEndPoint);
                client.Timeout = TimeSpan.FromMinutes(10);
            });

            var serviceProvider = services.BuildServiceProvider();

            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            _client = httpClientFactory.CreateClient(NamedHttpClients.Recommend_RestaurantTypeClient);

            List<Restaurant_Recommendation_Results> result = new List<Restaurant_Recommendation_Results>();
            var response = await _client.GetAsync($"api/restaurant/recommended_type?type={type}&id={id}", _token).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                /*await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                {
                    result = JsonConvert.DeserializeObject<List<Restaurant_Recommendation_Results>>(x.Result);
                }, _tokenSource.Token);*/
                await response.Content.ReadAsStreamAsync().ContinueWith((Task<Stream> x) =>
                {
                    using StreamReader reader = new StreamReader(x.Result);
                    using JsonTextReader json = new JsonTextReader(reader);
                    result = _jsonSerializer.Deserialize<List<Restaurant_Recommendation_Results>>(json);
                }, _tokenSource.Token);
                Console.WriteLine("Json result: " + result);
            }
            else
            {
                var content_result = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"{response.StatusCode}:{content_result}");
            }
            return result;
        }

        // Returning a list of restaurants visited by the user
        public async Task<List<Restaurant>> Restaurants_Visited_By_User(int id)
        {
            var services = new ServiceCollection();
            services.AddHttpClient(NamedHttpClients.Restaurant_VisitedClient, client =>
            {
                client.BaseAddress = new Uri(URLHelper.BaseEndPoint);
                client.Timeout = TimeSpan.FromMinutes(10);
            });

            var serviceProvider = services.BuildServiceProvider();

            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            _client = httpClientFactory.CreateClient(NamedHttpClients.Restaurant_VisitedClient);

            List<Restaurant> result = new List<Restaurant>();
            var response = await _client.GetAsync($"api/restaurant/visited/{id}", _token).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                /*await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                {
                    result = JsonConvert.DeserializeObject<List<Restaurant_Recommendation_Results>>(x.Result);
                }, _tokenSource.Token);*/
                await response.Content.ReadAsStreamAsync().ContinueWith((Task<Stream> x) =>
                {
                    using StreamReader reader = new StreamReader(x.Result);
                    using JsonTextReader json = new JsonTextReader(reader);
                    result = _jsonSerializer.Deserialize<List<Restaurant>>(json);
                }, _tokenSource.Token);
                Console.WriteLine("Json result: " + result);
            }
            else
            {
                var content_result = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"{response.StatusCode}:{content_result}");
            }
            return result;
        }

        // Returning a list of users
        public async Task<List<UserProfile>> UserList()
        {
            var services = new ServiceCollection();
            services.AddHttpClient(NamedHttpClients.UserListClient, client =>
            {
                client.BaseAddress = new Uri(URLHelper.BaseEndPoint);
                client.Timeout = TimeSpan.FromMinutes(10);
            });

            var serviceProvider = services.BuildServiceProvider();

            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            _client = httpClientFactory.CreateClient(NamedHttpClients.UserListClient);

            List<UserProfile> result = new List<UserProfile>();
            var response = await _client.GetAsync("api/restaurant/users", _token).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                /*await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                {
                    result = JsonConvert.DeserializeObject<List<Restaurant_Recommendation_Results>>(x.Result);
                }, _tokenSource.Token);*/
                await response.Content.ReadAsStreamAsync().ContinueWith((Task<Stream> x) =>
                {
                    using StreamReader reader = new StreamReader(x.Result);
                    using JsonTextReader json = new JsonTextReader(reader);
                    result = _jsonSerializer.Deserialize<List<UserProfile>>(json);
                }, _tokenSource.Token);
                Console.WriteLine("Json result: " + result);
            }
            else
            {
                var content_result = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"{response.StatusCode}:{content_result}");
            }
            return result;
        }

        // Returning a list of restaurants
        public async Task<List<Restaurant>> RestaurantList()
        {
            var services = new ServiceCollection();
            services.AddHttpClient(NamedHttpClients.RestaurantListClient, client =>
            {
                client.BaseAddress = new Uri(URLHelper.BaseEndPoint);
                client.Timeout = TimeSpan.FromMinutes(10);
            });

            var serviceProvider = services.BuildServiceProvider();

            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            _client = httpClientFactory.CreateClient(NamedHttpClients.RestaurantListClient);

            List<Restaurant> result = new List<Restaurant>();
            var response = await _client.GetAsync("api/restaurant/list", _token).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                /*await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                {
                    result = JsonConvert.DeserializeObject<List<Restaurant_Recommendation_Results>>(x.Result);
                }, _tokenSource.Token);*/
                await response.Content.ReadAsStreamAsync().ContinueWith((Task<Stream> x) =>
                {
                    using StreamReader reader = new StreamReader(x.Result);
                    using JsonTextReader json = new JsonTextReader(reader);
                    result = _jsonSerializer.Deserialize<List<Restaurant>>(json);
                }, _tokenSource.Token);
                Console.WriteLine("Json result: " + result);
            }
            else
            {
                var content_result = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"{response.StatusCode}:{content_result}");
            }
            return result;
        }
    }
}
