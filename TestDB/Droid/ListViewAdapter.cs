using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using TestDB.Droid.Model;

namespace TestDB.Droid
{
    public class ListViewAdapter:BaseAdapter
    {
        Activity activity;
        List<Account> lstAccount;
        LayoutInflater inflater;

        public ListViewAdapter(Activity activity, List<Account> lstAccount)
        {
            this.activity = activity;
            this.lstAccount = lstAccount;
        }

        public override int Count {
            get{
                return lstAccount.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            inflater = (LayoutInflater)activity.BaseContext.GetSystemService(Context.LayoutInflaterService);
            View itemView = inflater.Inflate(Resource.Layout.List_item, null);

            TextView txtUser = itemView.FindViewById<TextView>(Resource.Id.list_name);
            TextView txtEmail = itemView.FindViewById<TextView>(Resource.Id.list_email);

            if (lstAccount.Count > 0)
            {
                txtUser.Text = lstAccount[position].name;
                txtEmail.Text = lstAccount [position].email;
            }
            return itemView;
        }
    }
}
