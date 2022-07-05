using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Bumptech.Glide.Request.Target;
using Google.Android.Material.Button;
using RestaurantRecommender.MLCommon;

namespace RestaurantRecommender.Mobile.AndroidApp.Adapter
{
    public class UserListAdapter : RecyclerView.Adapter
    {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;

        public List<UserProfile> _userItemModel;
        public Context _context;
        public UserAdapterListener _userAdapterListener;
        public UserListAdapter(List<UserProfile> userItemModel, Context context, UserAdapterListener userAdapterListener)
        {
            _userItemModel = userItemModel;
            _context = context;
            _userAdapterListener = userAdapterListener;
        }

        public override int ItemCount
        {
            get
            {
                // TODO count the number of chef in the list
                return _userItemModel.Count();
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            // TODO bind the results to the model
            UserViewHolder userViewHolder = holder as UserViewHolder;
            userViewHolder.DisplayUserList(_userItemModel[position], _context);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the chef list layout:
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_user_list_layout, parent, false);

            // register viewholder clicks
            UserViewHolder userViewHolder = new UserViewHolder(itemView, OnClick);
            return userViewHolder;
        }

        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            if (ItemClick != null)
            {
                //ItemClick(this, position);
                _userAdapterListener.onUserSelected(_userItemModel[position]);
            }
        }
    }

    public interface UserAdapterListener
    {
        void onUserSelected(UserProfile userProfile);
    }

    public class UserViewHolder : RecyclerView.ViewHolder, IRequestListener
    {
        public ImageView User_Image { get; private set; }
        public TextView User_Name { get; private set; }
        public ProgressBar User_Progress { get; private set; }
        public MaterialButton User_View { get; private set; }

        // Get references to the views defined in the CardView layout.
        public UserViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            // Locate and cache view reference
            User_Image = itemView.FindViewById<ImageView>(Resource.Id.userimage);
            User_Name = itemView.FindViewById<TextView>(Resource.Id.user_name_tv);
            User_Progress = itemView.FindViewById<ProgressBar>(Resource.Id.userprogressBar);
            User_View = ItemView.FindViewById<MaterialButton>(Resource.Id.select_user_btn);

            // Detect user clicks on the item view and report which item
            // was clicked (by position) to the listener:
            //itemView.Click += (sender, e) => listener(AdapterPosition);
            User_View.Click += (sender, e) => listener(AdapterPosition);
        }

        public void DisplayUserList(UserProfile useritemModel, Context context)
        {
            User_Name.Text = $"User {useritemModel.UserId.ToString()}";
            Glide.With(context).Load(useritemModel.UserImage).Listener(this).Into(User_Image);
        }

        public bool OnLoadFailed(GlideException p0, Java.Lang.Object p1, ITarget p2, bool p3)
        {
            User_Progress.Visibility = ViewStates.Gone;
            return false;
        }

        public bool OnResourceReady(Java.Lang.Object p0, Java.Lang.Object p1, ITarget p2, DataSource p3, bool p4)
        {
            User_Progress.Visibility = ViewStates.Gone;
            return false;
        }
    }
}