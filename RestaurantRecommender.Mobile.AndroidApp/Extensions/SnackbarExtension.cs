using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;

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