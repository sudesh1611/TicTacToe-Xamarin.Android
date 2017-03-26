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
    [Activity(ScreenOrientation =Android.Content.PM.ScreenOrientation.Portrait)]
    public class GameModesActivity : AppCompatActivity
    {
        private Button singlePlayer1;
        private Button singlePlayer2;
        private Button twoPlayerOffline1;
        private Button twoPlayerOffline2;
        private Button twoPlayers1;
        private Button twoPlayers2;
        private Button stats;
        private Button about;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.GameModes);

            singlePlayer1 = FindViewById<Button>(Resource.Id.buttonSinglePlayer1);
            singlePlayer2 = FindViewById<Button>(Resource.Id.buttonSinglePlayer2);
            twoPlayerOffline1 = FindViewById<Button>(Resource.Id.buttonTwoPlayers1Offline);
            twoPlayerOffline2 = FindViewById<Button>(Resource.Id.buttonTwoPlayers2Offline);
            twoPlayers1 = FindViewById<Button>(Resource.Id.buttonTwoPlayers1);
            twoPlayers2 = FindViewById<Button>(Resource.Id.buttonTwoPlayers2);
            stats = FindViewById<Button>(Resource.Id.buttonStats);
            about = FindViewById<Button>(Resource.Id.buttonAbout);

            singlePlayer1.Click += SinglePlayerButtonClicked;
            singlePlayer2.Click += SinglePlayerButtonClicked;
            twoPlayerOffline1.Click += TwoPlayerOffline1_Click;
            twoPlayerOffline2.Click += TwoPlayerOffline1_Click;
            twoPlayers1.Click += TwoPlayersButtonClicked;
            twoPlayers2.Click += TwoPlayersButtonClicked;
            stats.Click += Stats_Click;
            about.Click += About_Click;
        }

        private void TwoPlayerOffline1_Click(object sender, EventArgs e)
        {
            StartActivityForResult(new Intent(Application.Context, typeof(TwoPlayersOfflineGameArena)), 234);
        }

        private void About_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(AboutActivity)));
        }

        private void Stats_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(StatsActivity)));
        }

        private void TwoPlayersButtonClicked(object sender, EventArgs e)
        {
            StartActivityForResult(new Intent(Application.Context, typeof(TwoPlayerStartActivity)),209);
        }

        private void SinglePlayerButtonClicked(object sender, EventArgs e)
        {
            StartActivityForResult(new Intent(Application.Context, typeof(SinglePlayerGameArena)),210);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if(requestCode==209)
            {
                if (resultCode == Result.Ok)
                {
                    StartActivityForResult(new Intent(Application.Context, typeof(TwoPlayersSecondActivity)), 221);
                }
                else if (data.GetStringExtra("Error") == "Cancel")
                {
                    Toast.MakeText(this, "Cancelled", ToastLength.Long).Show();
                }
                else if (data.GetStringExtra("Error") == "Network")
                {
                    Toast.MakeText(this, "Check Your Internet Connection", ToastLength.Long).Show();
                }
                else if (data.GetStringExtra("Error") == "Paused")
                {
                    StartActivityForResult(new Intent(Application.Context, typeof(TwoPlayerStartActivity)), 209);
                }
            }
            else if (requestCode == 221)
            {
                if (resultCode == Result.Ok)
                {
                    StartActivityForResult(new Intent(Application.Context, typeof(TwoPlayerGameArena)),219);
                }
                else if (data.GetStringExtra("Error") == "False")
                {
                    Toast.MakeText(this, "Opponent Abandoned Game", ToastLength.Long).Show();
                }
                else if (data.GetStringExtra("Error") == "Network")
                {
                    Toast.MakeText(this, "Check Your Internet Connection", ToastLength.Long).Show();
                }
            }
            else if(requestCode==219)
            {
                if(resultCode==Result.Ok)
                {
                    return;
                }
                else if(data.GetStringExtra("GameResult") == "Network")
                {
                    Toast.MakeText(this, "Connection With Server Lost", ToastLength.Long).Show();
                }
                else if(data.GetStringExtra("GameResult")== "IQuit")
                {
                    Toast.MakeText(this, "Game Terminated", ToastLength.Long).Show();
                }
                else if(data.GetStringExtra("GameResult") == "OpponentQuit")
                {
                    Toast.MakeText(this, "Opponent Abandoned Game", ToastLength.Long).Show();
                }
            }
            else if (requestCode == 210)
            {
                if (resultCode == Result.Ok)
                {
                    return;
                }
                else if (data.GetStringExtra("GameResult") == "IQuit")
                {
                    Toast.MakeText(this, "Game Terminated", ToastLength.Long).Show();
                }
            }
            else if (requestCode == 234)
            {
                if (resultCode == Result.Ok)
                {
                    return;
                }
                else if (data.GetStringExtra("GameResult") == "IQuit")
                {
                    Toast.MakeText(this, "Game Terminated", ToastLength.Long).Show();
                }
            }
        }
    }
}