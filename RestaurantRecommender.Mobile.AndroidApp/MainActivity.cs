﻿using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content.PM;
using Android.Support.Design.Widget;
using RestaurantRecommender.Mobile.AndroidApp.Extensions;
using RestaurantRecommender.Mobile.AndroidApp.UI.Restaurant;
using Newtonsoft.Json;
using RestaurantRecommender.Mobile.AndroidApp.Adapter;
using System.Collections.Generic;
using RestaurantRecommender.MLCommon;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace RestaurantRecommender.Mobile.AndroidApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : AppCompatActivity
    {
        BottomNavigationView bottomNavigationView;
        private CoordinatorLayout coordinatorLayout;
        private UserProfile selecteduser = new UserProfile();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AppCenter.Start(GetString(Resource.String.app_center_api), typeof(Analytics), typeof(Crashes));
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            selecteduser = JsonConvert.DeserializeObject<UserProfile>(Intent.GetStringExtra("User_List"));
            SupportActionBar.Subtitle = $"Welcome User {selecteduser.UserId}";

            bottomNavigationView = FindViewById<BottomNavigationView>(Resource.Id.nav_view);
            bottomNavigationView.NavigationItemSelected += BottomNavigationView_NavigationItemSelected;
            coordinatorLayout = FindViewById<CoordinatorLayout>(Resource.Id.coordinatorlayout);

            
            /*var a = restaurantItemModel.GroupBy(x => x.RestaurantType)
                .Select(group => new Restaurant()
                {
                    RestaurantType = group.Key,
                    RestaurantName = group.FirstOrDefault().RestaurantName
                }).ToList();*/

            // Load the first fragment
            LoadFragment(Resource.Id.navigation_home);
        }

        // Activity lifecycle
        protected override void OnResume()
        {
            base.OnResume();
            Xamarin.Essentials.Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        private void Connectivity_ConnectivityChanged(object sender, Xamarin.Essentials.ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet)
            {
                SnackbarExtension.Display_Online_SnackMessage(coordinatorLayout, this);
            }
            else
            {
                SnackbarExtension.Display_Offline_SnackMessage(coordinatorLayout, this);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            Xamarin.Essentials.Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }

        private void BottomNavigationView_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            LoadFragment(e.Item.ItemId);
        }

        private void LoadFragment(int position)
        {
            Android.Support.V4.App.Fragment fragment = null;
            Bundle bundle = new Bundle();
            switch (position)
            {
                case Resource.Id.navigation_home:
                    fragment = new HomeFragment();
                    bundle.PutString("Restaurant_List", Intent.GetStringExtra("Restaurant_List"));
                    fragment.Arguments = bundle;
                    break;
                case Resource.Id.navigation_recommendation:
                    fragment = new RecommendFragment();
                    bundle.PutInt("User_Id", selecteduser.UserId);
                    fragment.Arguments = bundle;
                    break;
                case Resource.Id.navigation_more_recommendation:
                    fragment = new RecommendCategoryFragment();
                    bundle.PutInt("User_Id", selecteduser.UserId);
                    fragment.Arguments = bundle;
                    break;
                case Resource.Id.navigation_visited:
                    fragment = new VisitedFragment();
                    bundle.PutInt("User_Id", selecteduser.UserId);
                    fragment.Arguments = bundle;
                    break;
            }
            if (fragment != null)
            {
                SupportFragmentManager.BeginTransaction()
                               .Replace(Resource.Id.main_content_frame, fragment)
                               .Commit();
            }
        }

        public override void OnBackPressed()
        {
            // Disable going back to the SigninActivity
            MoveTaskToBack(true);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}