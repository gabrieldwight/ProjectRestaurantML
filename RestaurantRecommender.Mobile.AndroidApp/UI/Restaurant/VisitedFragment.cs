using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using RestaurantRecommender.Mobile.AndroidApp.Adapter;
using RestaurantRecommender.Services.Core;
using Xamarin.Essentials;

namespace RestaurantRecommender.Mobile.AndroidApp.UI.Restaurant
{
    public class VisitedFragment : AndroidX.Fragment.App.Fragment
    {
        private List<MLCommon.Restaurant> restaurantItemModel = new List<MLCommon.Restaurant>();
        private VisitedRestaurantListAdapter restaurantrecommendationAdapter;
        RecyclerView restaurant_recyclerView;
        RecyclerView.LayoutManager restaurantlayoutManager;
        private readonly RestaurantRecommenderPayLoad restaurantRecommenderPayLoad = new RestaurantRecommenderPayLoad();
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            var rootview = inflater.Inflate(Resource.Layout.fragment_visited, container, false);
            restaurant_recyclerView = rootview.FindViewById<RecyclerView>(Resource.Id.visited_restaurant_recyclerview);
            return rootview;
            //return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnResume()
        {
            base.OnResume();
            System.Threading.ThreadPool.QueueUserWorkItem(async state =>
            {
                if (Arguments != null)
                {
                    await LoadVisitedRestaurant(Arguments.GetInt("User_Id"));
                }
            });
        }

        private async Task LoadVisitedRestaurant(int id)
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                try
                {
                    restaurantItemModel = await restaurantRecommenderPayLoad.Restaurants_Visited_By_User(id);
                    if (restaurantItemModel.Count() > 0)
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            restaurantlayoutManager = new LinearLayoutManager(restaurant_recyclerView.Context);
                            restaurant_recyclerView.SetLayoutManager(restaurantlayoutManager);
                            restaurantrecommendationAdapter = new VisitedRestaurantListAdapter(restaurantItemModel);
                            restaurant_recyclerView.SetAdapter(restaurantrecommendationAdapter);
                        });
                    }
                }
                catch (Exception ex)
                {
                    Activity.RunOnUiThread(() =>
                    {
                        AndroidX.AppCompat.App.AlertDialog.Builder dialog = new AndroidX.AppCompat.App.AlertDialog.Builder(Context);
                        AndroidX.AppCompat.App.AlertDialog alert = dialog.Create();
                        alert.SetTitle("Error");
                        alert.SetMessage(ex.Message.ToString());
                        alert.SetCancelable(true);
                        alert.Show();
                    });
                }
            }
            else
            {
                Activity.RunOnUiThread(() =>
                {
                    AndroidX.AppCompat.App.AlertDialog.Builder dialog = new AndroidX.AppCompat.App.AlertDialog.Builder(Context);
                    AndroidX.AppCompat.App.AlertDialog alert = dialog.Create();
                    alert.SetTitle("Error");
                    alert.SetMessage("Please connect to the internet");
                    alert.SetCancelable(true);
                    alert.Show();
                });
            }
        }
    }
}