using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using RestaurantRecommender.MLCommon;
using RestaurantRecommender.Mobile.AndroidApp.Adapter;
using RestaurantRecommender.Mobile.AndroidApp.UI.Restaurant;
using RestaurantRecommender.Services.Core;
using Xamarin.Essentials;

namespace RestaurantRecommender.Mobile.AndroidApp
{
    [Activity(Theme = "@style/AppTheme.Splash", Label = "@string/app_name", MainLauncher = true, NoHistory = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashActivity : AppCompatActivity
    {
        ProgressBar progressBar;
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;
        private readonly RestaurantRecommenderPayLoad restaurantRecommenderPayLoad = new RestaurantRecommenderPayLoad();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AppCenter.Start(GetString(Resource.String.app_center_api), typeof(Analytics), typeof(Crashes));
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Create your application here
            SetContentView(Resource.Layout.activity_splash);
            progressBar = FindViewById<ProgressBar>(Resource.Id.splashprogressBar);
            System.Threading.ThreadPool.QueueUserWorkItem(async state => await LoadRestaurantList());
        }

        // Prevent the back button from canceling the startup process
        public override void OnBackPressed()
        {

        }

        private async Task LoadRestaurantList()
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var result = await restaurantRecommenderPayLoad.RestaurantList();
                    var user_result = await restaurantRecommenderPayLoad.UserList();
                    RunOnUiThread(() =>
                    {
                        progressBar.Visibility = ViewStates.Gone;
                        if (result.Count() > 0 && user_result.Count() > 0)
                        {
                            Intent intent = new Intent(this, typeof(UserActivity));
                            var restraunt_group = result.GroupBy(x => x.RestaurantType)
                            .Select(group => new RestaurantSection(group.Select(x => x.RestaurantName).ToList())
                            {
                                SectionName = group.Key,
                            }).OrderBy(x => x.SectionName).ToList();
                            intent.PutExtra("User_List", JsonConvert.SerializeObject(user_result));
                            intent.PutExtra("Restaurant_List", JsonConvert.SerializeObject(restraunt_group));
                            this.StartActivity(intent);
                        }
                    });
                }
                catch (Exception ex)
                {
                    RunOnUiThread(() =>
                    {
                        Android.Support.V7.App.AlertDialog.Builder dialog = new Android.Support.V7.App.AlertDialog.Builder(this);
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
                RunOnUiThread(() =>
                {
                    Android.Support.V7.App.AlertDialog.Builder dialog = new Android.Support.V7.App.AlertDialog.Builder(this);
                    Android.Support.V7.App.AlertDialog alert = dialog.Create();
                    alert.SetTitle("Error");
                    alert.SetMessage("Please connect to the internet");
                    alert.SetCancelable(true);
                    alert.Show();
                });
            }
        }
    }
}