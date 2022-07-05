using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using RestaurantRecommender.MLCommon;

namespace RestaurantRecommender.Mobile.AndroidApp.Adapter
{
    public class VisitedRestaurantListAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;

        public List<Restaurant> _restaurantItemModel;

        public VisitedRestaurantListAdapter(List<Restaurant> restaurantItemModel)
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
            VisitedRestaurantViewHolder visitedrestaurantViewHolder = holder as VisitedRestaurantViewHolder;
            visitedrestaurantViewHolder.DisplayVisitedRestaurantList(_restaurantItemModel[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the chef list layout:
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_restaurant_list_layout, parent, false);

            // register viewholder clicks
            VisitedRestaurantViewHolder visitedrestaurantViewHolder = new VisitedRestaurantViewHolder(itemView, OnClick);
            return visitedrestaurantViewHolder;
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

    public class VisitedRestaurantViewHolder : RecyclerView.ViewHolder
    {
        public TextView Restaurant_Item { get; private set; }

        // Get references to the views defined in the CardView layout.
        public VisitedRestaurantViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            // Locate and cache view reference
            Restaurant_Item = itemView.FindViewById<TextView>(Resource.Id.restaurant_item_txt);

            // Detect user clicks on the item view and report which item
            // was clicked (by position) to the listener:
            itemView.Click += (sender, e) => listener(AdapterPosition);
        }

        public void DisplayVisitedRestaurantList(Restaurant restaurant)
        {
            Restaurant_Item.Text = restaurant.RestaurantName;
        }
    }
}