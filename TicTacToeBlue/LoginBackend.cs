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
using System.Net.Http;
using Newtonsoft.Json;
using TicTacToeBlue.Model;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace TicTacToeBlue
{
    [Activity(NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoginBackend : AppCompatActivity
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

        private void ThreadDoTheWork( string userName,string password)
        {
            Task.Run(() => this.DoTheWork(userName, password)).Wait();

            var intent = new Intent();
            intent.PutExtra("UserName", userName);
            intent.PutExtra("Password", password);
            if (res == "LoggedIn")
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
            else if (res == "PasswordWrong")
            {
                intent.PutExtra("Error", "Password");
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
            string urlUserPasswordCheck = "http://games.robonauts.in/Android/UserPasswordCheck";
            string urlLogInUser = "http://games.robonauts.in/Android/LogInUser";
            HttpContent q1 = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("userName", userName), new KeyValuePair<string, string>("password", password) });
            using (var httpClient=new HttpClient())
            {
                try
                {
                    Task<HttpResponseMessage> getResponse= httpClient.PostAsync(urlUserPasswordCheck, q1);
                    HttpResponseMessage response = await getResponse;
                    var myContent = await response.Content.ReadAsStringAsync();
                    if (myContent == "userFalse")
                    {
                        res = "UserNameExists";
                        return;
                    }
                    else if (myContent == "false")
                    {
                        res = "PasswordWrong";
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
                    Task<HttpResponseMessage> getResponse = httpClient.PostAsync(urlLogInUser, q1);
                    HttpResponseMessage response = await getResponse;
                    var myContent = await response.Content.ReadAsStringAsync();
                    var account = JsonConvert.DeserializeObject<AccountDataClass>(myContent);
                    var path = Application.Context.FilesDir.Path;
                    var filePath = System.IO.Path.Combine(path, "AccountData.json");
                    var edata = JsonConvert.SerializeObject(account);
                    if (File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        System.IO.File.WriteAllText(filePath, edata);
                        res = "LoggedIn";
                        return;
                    }
                    else
                    {
                        System.IO.File.WriteAllText(filePath, edata);
                        var tempString = System.IO.File.ReadAllText(filePath);
                        var tempObject = JsonConvert.DeserializeObject<AccountDataClass>(tempString);
                        res = "LoggedIn";
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