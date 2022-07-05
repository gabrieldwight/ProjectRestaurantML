using System.Collections.Generic;
using Android.OS;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Newtonsoft.Json;
using RestaurantRecommender.Mobile.AndroidApp.Adapter;

namespace RestaurantRecommender.Mobile.AndroidApp.UI.Restaurant
{
    public class HomeFragment : AndroidX.Fragment.App.Fragment
    {
        private List<RestaurantSection> restaurantItemModel = new List<RestaurantSection>();
        private RestaurantSectionAdapter restaurantSectionAdapter;
        RecyclerView restaurant_recyclerView;
        RecyclerView.LayoutManager restaurantlayoutManager;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            var rootview = inflater.Inflate(Resource.Layout.fragment_home, container, false);
            restaurant_recyclerView = rootview.FindViewById<RecyclerView>(Resource.Id.restaurant_sectioned_recyclerview);
            restaurantlayoutManager = new LinearLayoutManager(restaurant_recyclerView.Context);
            restaurant_recyclerView.SetLayoutManager(restaurantlayoutManager);
            if (Arguments != null)
            {
                restaurantItemModel = JsonConvert.DeserializeObject<List<RestaurantSection>>(Arguments.GetString("Restaurant_List"));
            }           
            restaurantSectionAdapter = new RestaurantSectionAdapter(restaurantItemModel, restaurant_recyclerView.Context);
            restaurant_recyclerView.SetAdapter(restaurantSectionAdapter);
            return rootview;
            //return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}