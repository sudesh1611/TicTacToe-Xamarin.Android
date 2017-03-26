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
using System.IO;
using Newtonsoft.Json;
using TicTacToeBlue.Model;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;

namespace TicTacToeBlue
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class TwoPlayerStartActivity : AppCompatActivity
    {
        private string myName;
        private string myID;
        private string res = "NoProblem";
        bool waitingForPlayers = false;
        bool IFQUIT = false;
        bool IFPAUSE = false;
        private Button quitButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.WaitingForPlayer);
            quitButton = FindViewById<Button>(Resource.Id.quit);
            quitButton.Click += QuitButton_Click;
            ThreadPool.QueueUserWorkItem(o => DoTheWork());
        }

        protected override void OnResume()
        {
            base.OnResume();
            if(IFPAUSE==true)
            {
                var intent = new Intent();
                intent.PutExtra("Error", "Paused");
                Task.Run(() => this.QuitGame()).Wait();
                SetResult(Result.Canceled, intent);
                Finish();
            }
            
        }

        protected override void OnPause()
        {
            base.OnPause();
            IFPAUSE = true;
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            IFQUIT = true;
        }

        private void DoTheWork()
        {
            var path = Application.Context.FilesDir.Path;
            var filePath = System.IO.Path.Combine(path, "AccountData.json");
            var edata = System.IO.File.ReadAllText(filePath);
            var temp = JsonConvert.DeserializeObject<AccountDataClass>(edata);
            myName = temp.MyName;
            myID = temp.MyID;
            Task.Run(() => this.ConnectToGame()).Wait();
            var intent = new Intent();
            if (res == "NetworkProblem")
            {
                intent.PutExtra("Error", "Network");
                SetResult(Result.Canceled, intent);
                Finish();
            }
            while (IFPAUSE==false && IFQUIT == false && waitingForPlayers == false)
            {
                //This function is being called continously
                Task.Run(() => this.WaitingForPlayer()).Wait();
                Thread.Sleep(2000);
            }
            if(IFPAUSE==true)
            {
                return;
            }
            if (IFQUIT == true)
            {
                intent.PutExtra("Error", "Cancel");
                Task.Run(() => this.QuitGame()).Wait();
                SetResult(Result.Canceled, intent);
                Finish();
            }
            if (res == "NetworkProblem")
            {
                intent.PutExtra("Error", "Network");
                SetResult(Result.Canceled, intent);
                Finish();
            }
            if (waitingForPlayers == true)
            {
                intent.PutExtra("Error", "");
                SetResult(Result.Ok, intent);
                Finish();
            }
        }

        public override void OnBackPressed()
        {
            IFQUIT = true;
        }


        private async Task QuitGame()
        {
            string url1 = "http://games.robonauts.in/Android/EmptyThePlayersFile";

            HttpContent q1 = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("id", myID) });


            using (var httpClient = new HttpClient())
            {
                try
                {
                    Task<HttpResponseMessage> getResponse = httpClient.PostAsync(url1, q1);
                    HttpResponseMessage response = await getResponse;
                    var myContent = await response.Content.ReadAsStringAsync();
                    if (myContent != "Deleted")
                    {
                        res = "NetworkProblem";
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

        private async Task ConnectToGame()
        {
            string url1 = "http://games.robonauts.in/Android/ConnectToGame";

            HttpContent q1 = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("PlayerName", myName), new KeyValuePair<string, string>("PlayerID", myID) });


            using (var httpClient = new HttpClient())
            {
                try
                {
                    Task<HttpResponseMessage> getResponse = httpClient.PostAsync(url1, q1);
                    HttpResponseMessage response = await getResponse;
                    var myContent = await response.Content.ReadAsStringAsync();
                    if (myContent != "WaitingForPlayer")
                    {
                        res = "NetworkProblem";
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
        private async Task WaitingForPlayer()
        {
            string url2 = "http://games.robonauts.in/Android/WaitingForPlayer";
            HttpContent q2 = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("ID", myID) });
            using (var httpClient = new HttpClient())
            {
                try
                {
                    Task<HttpResponseMessage> getResponse = httpClient.PostAsync(url2, q2);
                    HttpResponseMessage response = await getResponse;
                    var myContent = await response.Content.ReadAsStringAsync();
                    if (myContent == "True")
                    {
                        waitingForPlayers = true;
                        return;
                    }
                    else
                    {
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