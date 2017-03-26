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
using Newtonsoft.Json;
using TicTacToeBlue.Model;
using System.Threading.Tasks;
using System.Net.Http;
using Android.Support.V7.App;

namespace TicTacToeBlue
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class StatsActivity : AppCompatActivity
    {
        TextView name;
        TextView id;
        TextView singleWins;
        TextView singleLosses;
        TextView singleTies;
        TextView twoWins;
        TextView twoLosses;
        TextView twoTies;
        TextView totalWins;
        TextView totalTies;
        TextView Totallosses;
        Button logOutButton;
        Button backButton;
        AccountDataClass playerData;
        string res = "NoProblem";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Stats);
            name = FindViewById<TextView>(Resource.Id.nameTextView);
            id = FindViewById<TextView>(Resource.Id.idTextView);
            singleWins = FindViewById<TextView>(Resource.Id.singleWinsTextView);
            singleLosses = FindViewById<TextView>(Resource.Id.singleLossesView);
            singleTies = FindViewById<TextView>(Resource.Id.SingleTiesTextView);
            twoWins = FindViewById<TextView>(Resource.Id.twoWinsView);
            twoLosses = FindViewById<TextView>(Resource.Id.twoLossesTextView);
            twoTies = FindViewById<TextView>(Resource.Id.twoTiesTextView);
            totalWins = FindViewById<TextView>(Resource.Id.TotalWinsTextView);
            totalTies = FindViewById<TextView>(Resource.Id.totalTiesTextView);
            Totallosses = FindViewById<TextView>(Resource.Id.totalLossesTextView);
            logOutButton = FindViewById<Button>(Resource.Id.logOutButton);
            backButton = FindViewById<Button>(Resource.Id.backButton);
            logOutButton.Click += LogOutButton_Click;
            backButton.Click += BackButton_Click;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void LogOutButton_Click(object sender, EventArgs e)
        {

            RunOnUiThread(() => {
                logOutButton.Text = "Logging Out..";
            });
            Task.Run(() => UpdateAccountRecord()).Wait();
            if(res!="Done")
            {
                RunOnUiThread(() => {
                    logOutButton.Text = "Log Out";
                });
                Toast.MakeText(Application.Context, "Check Your Internet Connection", ToastLength.Long).Show();
            }
            else
            {
                var path = Application.Context.FilesDir.Path;
                var filePath = System.IO.Path.Combine(path, "AccountData.json");
                System.IO.File.Delete(filePath);
                var intent = new Intent(Application.Context, typeof(LoginActivity));
                intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(intent);
            }
        }

        public async Task UpdateAccountRecord()
        {
            string url1 = "http://games.robonauts.in/Android/UpdateAccountRecord";
            string updatedJson = JsonConvert.SerializeObject(playerData);
            HttpContent q1 = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("updatedJson", updatedJson) });


            using (var httpClient = new HttpClient())
            {
                try
                {
                    Task<HttpResponseMessage> getResponse = httpClient.PostAsync(url1, q1);
                    HttpResponseMessage response = await getResponse;
                    var myContent = await response.Content.ReadAsStringAsync();
                    if(myContent=="true")
                    {
                        res = "Done";
                        return;
                    }
                    else
                    {
                        res = "NotDone";
                        return;
                    }
                }
                catch (Exception ex)
                {
                    res = "NetworkProblem";
                    return;
                }

            }

        }

        protected override void OnResume()
        {
            base.OnResume();
            var path = Application.Context.FilesDir.Path;
            var filePath = System.IO.Path.Combine(path, "AccountData.json");
            var edata = System.IO.File.ReadAllText(filePath);
            playerData = JsonConvert.DeserializeObject<AccountDataClass>(edata);
            name.Text = playerData.MyName;
            id.Text = playerData.MyID;
            singleWins.Text = playerData.WinsSinglePlayer.ToString();
            singleLosses.Text = playerData.LosesSinglePlayer.ToString();
            singleTies.Text = playerData.TiesSinglePlayer.ToString();
            twoWins.Text = playerData.WinsTwoPlayer.ToString();
            twoLosses.Text = playerData.LosesTwoPlayer.ToString();
            twoTies.Text = playerData.TiesTwoPlayer.ToString();
            totalWins.Text = playerData.Wins.ToString();
            Totallosses.Text = playerData.Loses.ToString();
            totalTies.Text = playerData.Ties.ToString();
        }
    }
}