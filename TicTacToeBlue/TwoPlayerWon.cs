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
    public class TwoPlayerWon : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.YouWon);
            Button btn = FindViewById<Button>(Resource.Id.coolButton);
            TextView body = FindViewById<TextView>(Resource.Id.bodyTextView);
            var str=Intent.GetStringExtra("Player");
            if(str=="One")
            {
                body.Text = "Player 1 Won!!";
            }
            else
            {
                body.Text = "Player 2 Won!!";
            }
            btn.Click += Btn_Click;
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Finish();
        }
    }
}