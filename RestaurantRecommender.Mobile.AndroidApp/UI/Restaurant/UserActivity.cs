using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using RestaurantRecommender.MLCommon;
using RestaurantRecommender.Mobile.AndroidApp.Adapter;

namespace RestaurantRecommender.Mobile.AndroidApp.UI.Restaurant
{
    [Activity(Label = "@string/app_name")]
    public class UserActivity : AppCompatActivity, UserAdapterListener
    {
        private List<UserProfile> userItemModel = new List<UserProfile>();
        private UserListAdapter userListAdapter;
        RecyclerView user_recyclerView;
        RecyclerView.LayoutManager userlayoutManager;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AppCenter.Start(GetString(Resource.String.app_center_api), typeof(Analytics), typeof(Crashes));
            // Create your application here
            SetContentView(Resource.Layout.activity_user);
            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            SupportActionBar.SetDisplayShowTitleEnabled(true);

            user_recyclerView = FindViewById<RecyclerView>(Resource.Id.user_recyclerview);

            userItemModel = JsonConvert.DeserializeObject<List<UserProfile>>(Intent.GetStringExtra("User_List"));

            userlayoutManager = new LinearLayoutManager(user_recyclerView.Context);
            user_recyclerView.SetLayoutManager(userlayoutManager);

            userListAdapter = new UserListAdapter(userItemModel, user_recyclerView.Context, this);
            userListAdapter.ItemClick += UserListAdapter_ItemClick;
            user_recyclerView.SetAdapter(userListAdapter);
        }

        private void UserListAdapter_ItemClick(object sender, int position)
        {
            onUserSelected(userItemModel[position]);
        }

        public void onUserSelected(UserProfile userProfile)
        {
            // To go to next activity
            Intent intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra("User_List", JsonConvert.SerializeObject(userProfile));
            intent.PutExtra("Restaurant_List", Intent.GetStringExtra("Restaurant_List"));
            this.StartActivity(intent);
        }
    }
}