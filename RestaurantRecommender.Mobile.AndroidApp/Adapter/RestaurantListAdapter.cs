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

namespace RestaurantRecommender.Mobile.AndroidApp.Adapter
{
    // Items within the restaurant sections
    public class RestaurantListAdapter : RecyclerView.Adapter
    {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;

        // Help data set
        public List<RestaurantSection.RestaurantItems> _restaurantitemModel;
        public RestaurantItemAdapterListener _restaurantItemAdapterListener;

        public RestaurantListAdapter(List<RestaurantSection.RestaurantItems> restaurantitemModel, RestaurantItemAdapterListener restaurantItemAdapterListener)
        {
            _restaurantitemModel = restaurantitemModel;
            _restaurantItemAdapterListener = restaurantItemAdapterListener;
        }

        public override int ItemCount
        {
            get
            {
                // TODO count the number of chef in the list
                return _restaurantitemModel.Take(5).Count();
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            // TODO bind the results to the model
            RestaurantItemViewHolder restaurantitemViewHolder = holder as RestaurantItemViewHolder;
            restaurantitemViewHolder.DisplayRestaurantList(_restaurantitemModel[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the chef list layout:
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_restaurant_list_layout, parent, false);

            // register viewholder clicks
            RestaurantItemViewHolder restaurantitemViewHolder = new RestaurantItemViewHolder(itemView, OnClick);
            return restaurantitemViewHolder;
        }

        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            if (ItemClick != null)
            {
                //ItemClick(this, position);
                _restaurantItemAdapterListener.onRestaurantSelected(_restaurantitemModel[position], position);
            }
        }
    }

    public interface RestaurantItemAdapterListener
    {
        void onRestaurantSelected(RestaurantSection.RestaurantItems restaurant, int position);
    }

    public class RestaurantSection
    {
        public string SectionName { get; set; }
        public List<RestaurantItems> ItemsInSection { get; set; }

        public RestaurantSection()
        {
            ItemsInSection = new List<RestaurantItems>();
        }
        public RestaurantSection(List<string> items)
        {
            ItemsInSection = new List<RestaurantItems>();
            foreach (var item in items)
            {
                ItemsInSection.Add(new RestaurantItems()
                {
                    RestaurantName = item
                });
            }
        }

        public class RestaurantItems
        {
            public string RestaurantName { get; set; }
        }
    }

    public class RestaurantItemViewHolder : RecyclerView.ViewHolder
    {
        public TextView Restaurant_Item { get; private set; }

        // Get references to the views defined in the CardView layout.
        public RestaurantItemViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            // Locate and cache view reference
            Restaurant_Item = itemView.FindViewById<TextView>(Resource.Id.restaurant_item_txt);

            // Detect user clicks on the item view and report which item
            // was clicked (by position) to the listener:
            itemView.Click += (sender, e) => listener(AdapterPosition);
        }

        public void DisplayRestaurantList(RestaurantSection.RestaurantItems restaurant)
        {
            Restaurant_Item.Text = restaurant.RestaurantName;
        }
    }

    // Sections part
    public class RestaurantSectionAdapter : RecyclerView.Adapter
    {
        public List<RestaurantSection> _restaurantsectionModel;
        public Context _context;

        public RestaurantSectionAdapter(List<RestaurantSection> restaurantsectionModel, Context context)
        {
            _restaurantsectionModel = restaurantsectionModel;
            _context = context;
        }

        public override int ItemCount
        {
            get
            {
                // TODO count the number of chef in the list
                return _restaurantsectionModel.Count();
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            // TODO bind the results to the model
            RestaurantSectionViewHolder restaurantsectionViewHolder = holder as RestaurantSectionViewHolder;
            restaurantsectionViewHolder.DisplayRestaurantSectionList(_restaurantsectionModel[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the chef list layout:
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_restaurant_section_list_layout, parent, false);

            // register viewholder clicks
            RestaurantSectionViewHolder restaurantsectionViewHolder = new RestaurantSectionViewHolder(itemView);
            return restaurantsectionViewHolder;
        }
    }

    public class RestaurantSectionViewHolder : RecyclerView.ViewHolder, RestaurantItemAdapterListener
    {
        public TextView Restaurant_Section_Name { get; private set; }
        public RecyclerView Restaurant_Item_Recyclerview { get; private set; }

        // Get references to the views defined in the CardView layout.
        public RestaurantSectionViewHolder(View itemView) : base(itemView)
        {
            // Locate and cache view reference
            Restaurant_Section_Name = itemView.FindViewById<TextView>(Resource.Id.restaurant_section_txt);
            Restaurant_Item_Recyclerview = itemView.FindViewById<RecyclerView>(Resource.Id.restaurant_item_recyclerview);
        }

        public void DisplayRestaurantSectionList(RestaurantSection restaurant)
        {
            Restaurant_Section_Name.Text = restaurant.SectionName;
            RecyclerView.LayoutManager linearLayoutManager = new LinearLayoutManager(Restaurant_Item_Recyclerview.Context);
            Restaurant_Item_Recyclerview.SetLayoutManager(linearLayoutManager);
            RestaurantListAdapter restaurantListAdapter = new RestaurantListAdapter(restaurant.ItemsInSection, this);
            restaurantListAdapter.ItemClick += (sender, e) =>
            {
                onRestaurantSelected(restaurant.ItemsInSection[e], e);
            };
            Restaurant_Item_Recyclerview.SetAdapter(restaurantListAdapter);
        }

        public void onRestaurantSelected(RestaurantSection.RestaurantItems restaurant, int position)
        {
            // TO Do add more views
        }
    }
}