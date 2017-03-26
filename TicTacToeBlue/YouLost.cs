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
using Android.Support.V7.App;

namespace TicTacToeBlue
{
    [Activity(NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class YouLost : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.YouWon);
            Button btn = FindViewById<Button>(Resource.Id.coolButton);
            TextView heading = FindViewById<TextView>(Resource.Id.headingTextView);
            heading.Text = "  Blue | Defeat";
            TextView body = FindViewById<TextView>(Resource.Id.bodyTextView);
            body.Text = "Oh! We Lost!";
            btn.Text = "Okay";
            btn.Click += Btn_Click;
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Finish();
        }
    }
}