﻿using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using TestDB.Droid.Model;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;

namespace TestDB.Droid
{
    [Activity(Label = "TestDB", MainLauncher = true, Icon = "@mipmap/icon", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {

        private EditText input_name, input_email;
        private ListView list_data;
        private ProgressBar circular_progress;

        private List<Account> list_users = new List<Account>();
        private ListViewAdapter adapter;
        private Account selectedAccount;

        private const string FirebaseURL = "https://test-b5fcc.firebaseio.com/";
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "TEST Firebase";
            SetSupportActionBar(toolbar);

            circular_progress = FindViewById<ProgressBar>(Resource.Id.circularProgress);
            input_name = FindViewById<EditText>(Resource.Id.name);
            input_email = FindViewById<EditText>(Resource.Id.email);
            list_data = FindViewById<ListView>(Resource.Id.list_data);
            list_data.ItemClick += (s, e) =>
            {
                Account acc = list_users[e.Position];
                selectedAccount = acc;
                input_name.Text = acc.name;
                input_email.Text = acc.email;

            };

            await LoadData();

           
        }

        private async Task LoadData()
        {
            circular_progress.Visibility = Android.Views.ViewStates.Visible;
            list_data.Visibility = Android.Views.ViewStates.Invisible;

            var firebase = new FirebaseClient(FirebaseURL);
            var items = await firebase.Child("users").OnceAsync<Account>();

            list_users.Clear();
            adapter = null;
            foreach(var item in items){
                Account acc = new Account();
                acc.uid = item.Key;
                acc.name = item.Object.name;
                acc.email = item.Object.email;

                list_users.Add(acc);
            }
            adapter = new ListViewAdapter(this, list_users);
            adapter.NotifyDataSetChanged();
            list_data.Adapter = adapter;

            circular_progress.Visibility = Android.Views.ViewStates.Invisible;
            list_data.Visibility = Android.Views.ViewStates.Visible;
        }

        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.menu_add)
            {
                CreateUser();
            }
            else if (id == Resource.Id.menu_save) // Update
            {
                UpdateUser(selectedAccount.uid, input_name.Text, input_email.Text);
            }
            else if (id == Resource.Id.menu_remove)
            {
                DeleteUser(selectedAccount.uid);
            }

            return base.OnOptionsItemSelected(item);
        }

        private async void DeleteUser(string uid)
        {
            var firebase = new FirebaseClient(FirebaseURL);
            await firebase.Child("users").Child(uid).DeleteAsync();

            await LoadData();
        }

        private async void UpdateUser(string uid, string name, string email)
        {
            var firebase = new FirebaseClient(FirebaseURL);
            await firebase.Child("users").Child(uid).Child("name").PutAsync(name);
            await firebase.Child("users").Child(uid).Child("email").PutAsync(email );

            await LoadData();
        }

        private async void CreateUser()
        {
            Account user = new Account();
            user.uid = String.Empty;
            user.name = input_name.Text;
            user.email = input_email.Text;

            var firebase = new FirebaseClient(FirebaseURL);

            var item = await firebase.Child("users").PostAsync<Account>(user);
        }
    }
}

