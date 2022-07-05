using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using Google.Android.Material.Snackbar;

namespace RestaurantRecommender.Mobile.AndroidApp.Extensions
{
    public class SnackbarExtension
    {
        public static void Display_Offline_SnackMessage(CoordinatorLayout coordinatorLayout, Context context)
        {
            var snack = Snackbar.Make(coordinatorLayout, context.GetString(Resource.String.offline_message), Snackbar.LengthIndefinite);
            View snackView = snack.View;
            snackView.SetBackgroundColor(new Android.Graphics.Color(ContextCompat.GetColor(context, Resource.Color.snackbarOffline)));
            TextView textView = snackView.FindViewById<TextView>(Resource.Id.snackbar_text);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                textView.TextAlignment = TextAlignment.Center;
            }
            else
            {
                textView.Gravity = GravityFlags.CenterHorizontal;
            }
            snack.Show();
        }

        public static void Display_Online_SnackMessage(CoordinatorLayout coordinatorLayout, Context context)
        {
            var snack = Snackbar.Make(coordinatorLayout, context.GetString(Resource.String.online_message), Snackbar.LengthShort);
            View snackView = snack.View;
            snackView.SetBackgroundColor(new Android.Graphics.Color(ContextCompat.GetColor(context, Resource.Color.snackbarOnline)));
            TextView textView = snackView.FindViewById<TextView>(Resource.Id.snackbar_text);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                textView.TextAlignment = TextAlignment.Center;
            }
            else
            {
                textView.Gravity = GravityFlags.CenterHorizontal;
            }
            snack.Show();
        }
    }
}