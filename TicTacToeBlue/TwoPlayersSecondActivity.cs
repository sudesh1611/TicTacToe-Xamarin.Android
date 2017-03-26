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
using System.Threading;
using Newtonsoft.Json;
using TicTacToeBlue.Model;
using System.Threading.Tasks;
using System.Net.Http;

namespace TicTacToeBlue
{
    [Activity(NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class TwoPlayersSecondActivity : AppCompatActivity
    {
        private string myID;
        private string res = "NoProblem";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.LoadingScreen);
            TextView textView = FindViewById<TextView>(Resource.Id.textToDisplay);
            textView.Text = "Loading Game Data...";
            
        }

        protected override void OnResume()
        {
            base.OnResume();
            ThreadPool.QueueUserWorkItem(o => DoTheWork());
        }

        private void DoTheWork()
        {
            var path = Application.Context.FilesDir.Path;
            var filePath = System.IO.Path.Combine(path, "AccountData.json");
            var edata = System.IO.File.ReadAllText(filePath);
            var temp = JsonConvert.DeserializeObject<AccountDataClass>(edata);
            myID = temp.MyID;
            Thread.Sleep(4000);
            Task.Run(() => this.CheckIfOpponentAlive()).Wait();
            var intent = new Intent();
            if (res == "NetworkProblem")
            {
                intent.PutExtra("Error", "Network");
                SetResult(Result.Canceled, intent);
                Finish();
            }
            else if(res=="True")
            {
                intent.PutExtra("Error", "");
                SetResult(Result.Ok, intent);
                Finish();
            }
            else if(res=="False")
            {
                intent.PutExtra("Error", "False");
                SetResult(Result.Canceled, intent);
                Finish();
            }
        }
        public override void OnBackPressed()
        {
        }

        private async Task CheckIfOpponentAlive()
        {
            string url1 = "http://games.robonauts.in/Android/checkIfOpponentAlive";

            HttpContent q1 = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("ID", myID) });


            using (var httpClient = new HttpClient())
            {
                try
                {
                    Task<HttpResponseMessage> getResponse = httpClient.PostAsync(url1, q1);
                    HttpResponseMessage response = await getResponse;
                    var myContent = await response.Content.ReadAsStringAsync();
                    if (myContent == "True")
                    {
                        res = "True";
                        return;
                    }
                    else
                    {
                        res = "False";
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
    }
}