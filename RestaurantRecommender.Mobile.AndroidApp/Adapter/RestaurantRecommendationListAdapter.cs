using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using RestaurantRecommender.MLCommon.ViewModel;

namespace RestaurantRecommender.Mobile.AndroidApp.Adapter
{
    public class RestaurantRecommendationListAdapter : RecyclerView.Adapter
    {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;

        public List<Restaurant_Recommendation_Results> _restaurantItemModel;
        public RestaurantRecommendationListAdapter(List<Restaurant_Recommendation_Results> restaurantItemModel)
        {
            _restaurantItemModel = restaurantItemModel;
        }

        public override int ItemCount
        {
            get
            {
                // TODO count the number of chef in the list
                return _restaurantItemModel.Count();
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            // TODO bind the results to the model
            RestaurantRecommendationViewHolder restaurantrecommendationViewHolder = holder as RestaurantRecommendationViewHolder;
            restaurantrecommendationViewHolder.DisplayRestaurantRecommendationList(_restaurantItemModel[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the chef list layout:
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_restaurant_recommendation_list_layout, parent, false);

            // register viewholder clicks
            RestaurantRecommendationViewHolder restaurantrecommendationViewHolder = new RestaurantRecommendationViewHolder(itemView, OnClick);
            return restaurantrecommendationViewHolder;
        }

        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            if (ItemClick != null)
            {
                ItemClick(this, position);
            }
        }
    }

    public class RestaurantRecommendationViewHolder : RecyclerView.ViewHolder
    {
        public TextView Restaurant_Name { get; private set; }
        public TextView Restaurant_Type { get; private set; }
        public TextView Restaurant_Rating { get; private set; }
        public ProgressBar Probability_Progress { get; private set; }

        // Get references to the views defined in the CardView layout.
        public RestaurantRecommendationViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            // Locate and cache view reference
            Restaurant_Name = itemView.FindViewById<TextView>(Resource.Id.restaurant_name_tv);
            Restaurant_Type = itemView.FindViewById<TextView>(Resource.Id.restaurant_type_tv);
            Restaurant_Rating = itemView.FindViewById<TextView>(Resource.Id.predicted_rating);
            Probability_Progress = ItemView.FindViewById<ProgressBar>(Resource.Id.probability_progressBar);

            // Detect user clicks on the item view and report which item
            // was clicked (by position) to the listener:
            itemView.Click += (sender, e) => listener(AdapterPosition);
        }

        public void DisplayRestaurantRecommendationList(Restaurant_Recommendation_Results restaurantitemModel)
        {
            Restaurant_Name.Text = restaurantitemModel.Restaurant_Name;
            Restaurant_Type.Text = restaurantitemModel.Restaurant_Type;
            Restaurant_Rating.Text = restaurantitemModel.Restaurant_Rating > 3 
                ? $"Predicted rating - {restaurantitemModel.Restaurant_Rating} ({(restaurantitemModel.Restaurant_Rating / 5) * 100}%) Recommended Restaurant" 
                : $"Predicted rating - {restaurantitemModel.Restaurant_Rating} ({(restaurantitemModel.Restaurant_Rating / 5) * 100}%) Give a try";
            Probability_Progress.Progress = Convert.ToInt32((restaurantitemModel.Restaurant_Rating / 5) * 100);
        }
    }
}