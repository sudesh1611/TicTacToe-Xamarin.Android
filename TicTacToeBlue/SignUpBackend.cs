using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using TicTacToeBlue.Model;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace TicTacToeBlue
{
    [Activity(NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SignUpBackend : AppCompatActivity
    {
        private string res;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.LoadingScreen);
            string userName = Intent.GetStringExtra("UserName");
            string password = Intent.GetStringExtra("Password");
            ThreadPool.QueueUserWorkItem(o => ThreadDoTheWork(userName, password));
        }

        private void ThreadDoTheWork(string userName, string password)
        {
            Task.Run(() => this.DoTheWork(userName, password)).Wait();
            var intent = new Intent();
            intent.PutExtra("UserName", userName);
            intent.PutExtra("Password", password);
            if (res == "AccountCreated")
            {
                intent.PutExtra("Error", "");
                SetResult(Result.Ok, intent);
                Finish();
            }
            else if (res == "UserNameExists")
            {
                intent.PutExtra("Error", "UserName");
                SetResult(Result.Canceled, intent);
                Finish();
            }
            else
            {
                intent.PutExtra("Error", "Network");
                SetResult(Result.Canceled, intent);
                Finish();
            }
        }

        private async Task DoTheWork(string userName, string password)
        {
            string urlUserName = "http://games.robonauts.in/Android/UserNameCheck";
            string urlSignUp = "http://games.robonauts.in/Android/SignUpUser";
            HttpContent q = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("userName", userName) });
            HttpContent q1 = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("userName", userName), new KeyValuePair<string, string>("password", password) });
            using (var httpClient = new HttpClient())
            {
                try
                {
                    Task<HttpResponseMessage> getResponse = httpClient.PostAsync(urlUserName, q);
                    HttpResponseMessage response = await getResponse;
                    var myContent = await response.Content.ReadAsStringAsync();
                    if (myContent == "true")
                    {
                        res = "UserNameExists";
                        return;
                    }
                }
                catch (Exception ex)
                {
                    res = "NetworkProblem";
                    return;
                }

            }
            using (var httpClient = new HttpClient())
            {
                try
                {
                    Task<HttpResponseMessage> getResponse = httpClient.PostAsync(urlSignUp, q1);
                    HttpResponseMessage response = await getResponse;
                    var myContent = await response.Content.ReadAsStringAsync();
                    var account = JsonConvert.DeserializeObject<AccountDataClass>(myContent);
                    if(account.ID==-5)
                    {
                        res = "NetworkProblem";
                        return;
                    }
                    var path = Application.Context.FilesDir.Path;
                    var filePath = System.IO.Path.Combine(path, "AccountData.json");
                    var edata = JsonConvert.SerializeObject(account);
                    if (File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        System.IO.File.WriteAllText(filePath, edata);
                        res = "AccountCreated";
                        return;
                    }
                    else
                    {
                        System.IO.File.WriteAllText(filePath, edata);
                        res = "AccountCreated";
                        return;
                    }
                }
                catch (Exception ex)
                {
                    res = "NetworkProblem";
                    return;
                }
            }
            res = "NetworkProblem";
            return;
        }


        public override void OnBackPressed()
        {
        }
    }
}