using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Chip;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using RestaurantRecommender.MLCommon.ViewModel;
using RestaurantRecommender.Mobile.AndroidApp.Adapter;
using RestaurantRecommender.Services.Core;
using Xamarin.Essentials;

namespace RestaurantRecommender.Mobile.AndroidApp.UI.Restaurant
{
    public class RecommendCategoryFragment : Android.Support.V4.App.Fragment, ChipGroup.IOnCheckedChangeListener
    {
        private List<Restaurant_Recommendation_Results> restaurantItemModel = new List<Restaurant_Recommendation_Results>();
        private RestaurantRecommendationListAdapter restaurantrecommendationAdapter;
        RecyclerView restaurant_recyclerView;
        RecyclerView.LayoutManager restaurantlayoutManager;
        ChipGroup chipGroup;
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
            var rootview = inflater.Inflate(Resource.Layout.fragment_recommend_category, container, false);
            restaurant_recyclerView = rootview.FindViewById<RecyclerView>(Resource.Id.recommend_restaurant_category_recyclerview);
            chipGroup = rootview.FindViewById<ChipGroup>(Resource.Id.chipGroup);
            chipGroup.SetOnCheckedChangeListener(this);
            return rootview;
            //return base.OnCreateView(inflater, container, savedInstanceState);
        }

        private async Task LoadRestaurantCategoryRecommendations(string type, int id)
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                try
                {
                    restaurantItemModel = await restaurantRecommenderPayLoad.Create_Restaurant_Type_Recommendation(type, id);
                    if (restaurantItemModel.Count() > 0)
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            restaurantlayoutManager = new LinearLayoutManager(restaurant_recyclerView.Context);
                            restaurant_recyclerView.SetLayoutManager(restaurantlayoutManager);
                            restaurantrecommendationAdapter = new RestaurantRecommendationListAdapter(restaurantItemModel);
                            restaurant_recyclerView.SetAdapter(restaurantrecommendationAdapter);
                        });
                    }
                }
                catch (Exception ex)
                {
                    Activity.RunOnUiThread(() =>
                    {
                        Android.Support.V7.App.AlertDialog.Builder dialog = new Android.Support.V7.App.AlertDialog.Builder(Context);
                        Android.Support.V7.App.AlertDialog alert = dialog.Create();
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
                    Android.Support.V7.App.AlertDialog.Builder dialog = new Android.Support.V7.App.AlertDialog.Builder(Context);
                    Android.Support.V7.App.AlertDialog alert = dialog.Create();
                    alert.SetTitle("Error");
                    alert.SetMessage("Please connect to the internet");
                    alert.SetCancelable(true);
                    alert.Show();
                });
            }
        }

        public void OnCheckedChanged(ChipGroup p0, int p1)
        {
            var chip = (Chip)p0.FindViewById(p1);
            //Toast.MakeText(Context, $"{chip.Text} checked", ToastLength.Short).Show();

            string selectedchip = null;
            switch (chip.Text)
            {
                case "Buffet":
                    selectedchip = "Buffet";
                    break;
                case "Cafe":
                    selectedchip = "Cafe";
                    break;
                case "Casual Dining":
                    selectedchip = "Casual Dining";
                    break;
                case "Family Style":
                    selectedchip = "Family Style";
                    break;
                case "Fast Food":
                    selectedchip = "Fast Food";
                    break;
                case "Snack Bar":
                    selectedchip = "Snack Bar";
                    break;
            }
            if (!string.IsNullOrEmpty(selectedchip))
            {
                System.Threading.ThreadPool.QueueUserWorkItem(async state =>
                {
                    if (Arguments != null)
                    {
                        await LoadRestaurantCategoryRecommendations(selectedchip, Arguments.GetInt("User_Id"));
                    }
                });
            }
        }
    }
}