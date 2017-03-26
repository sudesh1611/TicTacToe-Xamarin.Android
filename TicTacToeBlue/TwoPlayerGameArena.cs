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
using TicTacToeBlue.Model;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;

namespace TicTacToeBlue
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class TwoPlayerGameArena : AppCompatActivity, View.IOnTouchListener
    {

        private ImageView[] imageView = new ImageView[10];
        private Button quitButton;
        private Button turnButton;
        private TextView playerNameTextView;
        private TextView opponentNameTextView;
        private TextView symbolTextView;
        private Android.Graphics.Color blueColor = Android.Graphics.Color.ParseColor("#6495ED");
        private Android.Graphics.Color whiteColor = Android.Graphics.Color.ParseColor("#FFFFFF");

        private Button checkStatusButton = new Button(Application.Context);
        private Button updateStatusButton = new Button(Application.Context);
        private Button NetworkProblemButton = new Button(Application.Context);
        private Button OpponentQuitButton = new Button(Application.Context);
        private Button YouCanPlayTheGameButton = new Button(Application.Context);
        private Button YouWonButton = new Button(Application.Context);
        private Button YouLostButton = new Button(Application.Context);
        private Button GameTiedButton = new Button(Application.Context);


        private GameDataClass gameData;
        private AccountDataClass playerData;


        private string myID;
        private string myName;


        private string res = "NoProblem";
        private bool IfMyTurn = false;
        private bool GameOver = false;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            imageView[1] = FindViewById<ImageView>(Resource.Id.imageView1);
            imageView[2] = FindViewById<ImageView>(Resource.Id.imageView2);
            imageView[3] = FindViewById<ImageView>(Resource.Id.imageView3);
            imageView[4] = FindViewById<ImageView>(Resource.Id.imageView4);
            imageView[5] = FindViewById<ImageView>(Resource.Id.imageView5);
            imageView[6] = FindViewById<ImageView>(Resource.Id.imageView6);
            imageView[7] = FindViewById<ImageView>(Resource.Id.imageView7);
            imageView[8] = FindViewById<ImageView>(Resource.Id.imageView8);
            imageView[9] = FindViewById<ImageView>(Resource.Id.imageView9);
            quitButton = FindViewById<Button>(Resource.Id.quit);
            turnButton = FindViewById<Button>(Resource.Id.turn);
            playerNameTextView = FindViewById<TextView>(Resource.Id.playerName);
            opponentNameTextView = FindViewById<TextView>(Resource.Id.opponentName);
            symbolTextView = FindViewById<TextView>(Resource.Id.symbol);
            imageView[1].SetImageResource(Resource.Drawable.Blank);
            imageView[2].SetImageResource(Resource.Drawable.Blank);
            imageView[3].SetImageResource(Resource.Drawable.Blank);
            imageView[4].SetImageResource(Resource.Drawable.Blank);
            imageView[5].SetImageResource(Resource.Drawable.Blank);
            imageView[6].SetImageResource(Resource.Drawable.Blank);
            imageView[7].SetImageResource(Resource.Drawable.Blank);
            imageView[8].SetImageResource(Resource.Drawable.Blank);
            imageView[9].SetImageResource(Resource.Drawable.Blank);
            imageView[1].SetOnTouchListener(this);
            imageView[2].SetOnTouchListener(this);
            imageView[3].SetOnTouchListener(this);
            imageView[4].SetOnTouchListener(this);
            imageView[5].SetOnTouchListener(this);
            imageView[6].SetOnTouchListener(this);
            imageView[7].SetOnTouchListener(this);
            imageView[8].SetOnTouchListener(this);
            imageView[9].SetOnTouchListener(this);
            quitButton.Click += QuitButton_Click;

            checkStatusButton.Click += CheckStatusButton_Click;
            updateStatusButton.Click += UpdateStatusButton_Click;
            NetworkProblemButton.Click += NetworkProblemButton_Click;
            OpponentQuitButton.Click += OpponentQuitButton_Click;
            YouCanPlayTheGameButton.Click += YouCanPlayTheGameButton_Click;
            YouWonButton.Click += YouWonButton_Click;
            YouLostButton.Click += YouLostButton_Click;
            GameTiedButton.Click += GameTiedButton_Click;
            ThreadPool.QueueUserWorkItem(o => FirstPhase());
        }


        /* First phase of app*/

        private void FirstPhase()
        {
            var path = Application.Context.FilesDir.Path;
            var filePath = System.IO.Path.Combine(path, "AccountData.json");
            var edata = System.IO.File.ReadAllText(filePath);
            playerData = JsonConvert.DeserializeObject<AccountDataClass>(edata);
            myName = playerData.MyName;
            myID = playerData.MyID;
            Task.Run(() => ReturnMyData()).Wait();
            if (res == "NetworkProblem")
            {
                NetworkProblemButton.PerformClick();
                return;
            }
            if (gameData.Turn == "Mine")
            {
                RunOnUiThread(() =>
                {
                    playerNameTextView.Text = playerData.MyName;
                    turnButton.SetBackgroundColor(blueColor);
                    turnButton.Text = "Your Turn";
                    turnButton.SetTextColor(whiteColor);
                    symbolTextView.Text = gameData.MySymbol;
                    opponentNameTextView.Text = gameData.OpponentName;
                    YouCanPlayTheGameButton.PerformClick();
                });
            }
            else
            {
                RunOnUiThread(() =>
                {
                    playerNameTextView.Text = playerData.MyName;
                    turnButton.SetBackgroundResource(Resource.Drawable.myWhiteButton);
                    //turnButton.SetBackgroundColor(whiteColor);
                    turnButton.Text = "Opponent's Turn";
                    turnButton.SetTextColor(blueColor);
                    symbolTextView.Text = gameData.MySymbol;
                    opponentNameTextView.Text = gameData.OpponentName;
                    checkStatusButton.PerformClick();
                });
            }
        }

        public async Task ReturnMyData()
        {
            string url1 = "http://games.robonauts.in/Android/ReturnMyData";

            HttpContent q1 = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("ID", myID) });


            using (var httpClient = new HttpClient())
            {
                try
                {
                    Task<HttpResponseMessage> getResponse = httpClient.PostAsync(url1, q1);
                    HttpResponseMessage response = await getResponse;
                    var myContent = await response.Content.ReadAsStringAsync();
                    gameData = JsonConvert.DeserializeObject<GameDataClass>(myContent);
                    return;
                }
                catch (Exception ex)
                {
                    res = "NetworkProblem";
                    return;
                }

            }

        }



        /* Update UI */

        private void UpdateUI()
        {
            RunOnUiThread(() =>
            {
                for (int i = 0; i < 9; i++)
                {
                    if (gameData.SymbolPresent[i] == "b")
                    {
                        imageView[i + 1].SetImageResource(Resource.Drawable.Blank);
                    }
                    else if (gameData.SymbolPresent[i] == "O")
                    {
                        imageView[i + 1].SetImageResource(Resource.Drawable.Circle);
                    }
                    else
                    {
                        imageView[i + 1].SetImageResource(Resource.Drawable.Cross);
                    }
                }
                if (gameData.Turn == "Mine")
                {
                    turnButton.SetBackgroundColor(blueColor);
                    turnButton.SetTextColor(whiteColor);
                    turnButton.Text = "Your Turn";
                }
                else
                {
                    turnButton.SetBackgroundResource(Resource.Drawable.myWhiteButton);
                    //turnButton.SetBackgroundColor(whiteColor);
                    turnButton.SetTextColor(blueColor);
                    turnButton.Text = "Opponent's Turn";
                }
            });
        }



        /* YouCanPlay Functions */

        private void YouCanPlayTheGameButton_Click(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                UpdateUI();
                IfMyTurn = true;
            });
        }



        /* Opponent Quit Function */

        private void OpponentQuitButton_Click(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                var intent = new Intent();
                intent.PutExtra("GameResult", "OpponentQuit");
                Task.Run(() => DeleteQuitted()).Wait();
                SetResult(Result.Canceled, intent);
                Finish();
            });
        }

        public async Task DeleteQuitted()
        {
            string url1 = "http://games.robonauts.in/Android/DeleteQuitted";

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
                    }
                    return;
                }
                catch (Exception ex)
                {
                    res = "NetworkProblem";
                    return;
                }

            }

        }



        /* Network Problem Function */

        private void NetworkProblemButton_Click(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                var intent = new Intent();
                intent.PutExtra("GameResult", "Network");
                SetResult(Result.Canceled, intent);
                Finish();
            });
        }



        /* Update Status Functions */

        private void UpdateStatusButton_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(o => CallUpdateStatus());
        }

        private void CallUpdateStatus()
        {
            Task.Run(() => updateStatus()).Wait();
            if (res == "NetworkProblem")
            {
                RunOnUiThread(() =>
                {
                    NetworkProblemButton.PerformClick();
                });
                return;
            }
            RunOnUiThread(() =>
            {
                checkStatusButton.PerformClick();
            });
        }

        public async Task updateStatus()
        {
            string url1 = "http://games.robonauts.in/Android/updateStatus";

            string updatedJson = JsonConvert.SerializeObject(gameData);

            HttpContent q1 = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("updatedJson", updatedJson) });


            using (var httpClient = new HttpClient())
            {
                try
                {
                    Task<HttpResponseMessage> getResponse = httpClient.PostAsync(url1, q1);
                    HttpResponseMessage response = await getResponse;
                    var myContent = await response.Content.ReadAsStringAsync();
                    if (myContent != "Okay")
                    {
                        res = "NetworkProblem";
                    }
                    return;
                }
                catch (Exception ex)
                {
                    res = "NetworkProblem";
                    return;
                }

            }

        }



        /* Check Status Functions */

        private void CheckStatusButton_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(o => CallCheckStatus());
        }

        private void CallCheckStatus()
        {
            while (gameData.Turn != "Mine")
            {
                Thread.Sleep(2000);
                Task.Run(() => checkStatus()).Wait();
                if (res == "NetworkProblem" || gameData.IfQuit == "True" || gameData.Status == "Won" || gameData.Status == "Lost" || gameData.Status == "Tie")
                {
                    break;
                }
            }
            if (res == "NetworkProblem")
            {
                RunOnUiThread(() =>
                {
                    NetworkProblemButton.PerformClick();
                });
                return;
            }
            if (gameData.IfQuit == "True")
            {
                RunOnUiThread(() =>
                {
                    OpponentQuitButton.PerformClick();
                });

                return;
            }
            else if (gameData.Status == "Won")
            {
                RunOnUiThread(() =>
                {
                    YouWonButton.PerformClick();
                });
                return;
            }
            else if (gameData.Status == "Lost")
            {
                RunOnUiThread(() =>
                {
                    YouLostButton.PerformClick();
                });
                return;
            }
            else if (gameData.Status == "Tie")
            {
                RunOnUiThread(() =>
                {
                    GameTiedButton.PerformClick();
                });
                return;
            }
            else if (gameData.Turn == "Mine")
            {
                RunOnUiThread(() =>
                {
                    YouCanPlayTheGameButton.PerformClick();
                });
                return;
            }
        }

        public async Task checkStatus()
        {
            string url1 = "http://games.robonauts.in/Android/checkStatus";

            HttpContent q1 = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("ID", myID) });


            using (var httpClient = new HttpClient())
            {
                try
                {
                    Task<HttpResponseMessage> getResponse = httpClient.PostAsync(url1, q1);
                    HttpResponseMessage response = await getResponse;
                    var myContent = await response.Content.ReadAsStringAsync();
                    gameData = JsonConvert.DeserializeObject<GameDataClass>(myContent);
                    return;
                }
                catch (Exception ex)
                {
                    res = "NetworkProblem";
                    return;
                }

            }

        }


        /* Quit Button Function */

        private void QuitButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent();
            if (GameOver == true)
            {
                intent.PutExtra("GameResult", "Over");
                SetResult(Result.Ok, intent);
            }
            else
            {
                intent.PutExtra("GameResult", "IQuit");
                SetResult(Result.Canceled, intent);
                Task.Run(() => this.IQuit()).Wait();
            }
            Finish();
        }

        public async Task IQuit()
        {
            string url1 = "http://games.robonauts.in/Android/IQuit";

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
                    }
                    return;
                }
                catch (Exception ex)
                {
                    res = "NetworkProblem";
                    return;
                }

            }

        }



        /* When user clicks on image */

        public bool OnTouch(View v, MotionEvent e)
        {
            if (IfMyTurn == true)
            {
                IfMyTurn = false;
                switch (e.Action)
                {
                    case MotionEventActions.Down:
                        {
                            for (int i = 0; i <= 8; i++)
                            {
                                if (v.Id == imageView[i + 1].Id)
                                {
                                    if (gameData.IfClicked[i] == 0)
                                    {
                                        gameData.IfClicked[i] = 1;
                                        if (gameData.MySymbol == "X")
                                        {
                                            gameData.SymbolPresent[i] = "X";
                                            imageView[i + 1].SetImageResource(Resource.Drawable.Cross);
                                        }
                                        else
                                        {
                                            gameData.SymbolPresent[i] = "O";
                                            imageView[i + 1].SetImageResource(Resource.Drawable.Circle);
                                        }
                                        gameData.Turn = "Opponent";
                                        turnButton.SetBackgroundResource(Resource.Drawable.myWhiteButton);
                                        //turnButton.SetBackgroundColor(whiteColor);
                                        turnButton.Text = "Opponent's Turn";
                                        turnButton.SetTextColor(blueColor);
                                        updateStatusButton.PerformClick();
                                        return true;
                                    }
                                    else
                                    {
                                        IfMyTurn = true;
                                        return false;
                                    }
                                }
                            }

                            break;
                        }
                }
            }
            return false;
        }



        /* You Won Functions */

        private void YouWonButton_Click(object sender, EventArgs e)
        {
            UpdateUI();
            GameOver = true;
            playerData.WinsTwoPlayer = playerData.WinsTwoPlayer + 1;
            playerData.Wins = playerData.Wins + 1;
            var path = Application.Context.FilesDir.Path;
            var filePath = System.IO.Path.Combine(path, "AccountData.json");
            string edata = JsonConvert.SerializeObject(playerData);
            System.IO.File.Delete(filePath);
            System.IO.File.WriteAllText(filePath, edata);
            Task.Run(() => UpdateAccountRecord()).Wait();
            StartActivity(new Intent(Application.Context, typeof(YouWon)));
            quitButton.Text = "EXIT";
        }



        /* You Lost Function */

        private void YouLostButton_Click(object sender, EventArgs e)
        {
            UpdateUI();
            GameOver = true;
            playerData.LosesTwoPlayer = playerData.LosesTwoPlayer + 1;
            playerData.Loses = playerData.Loses + 1;
            var path = Application.Context.FilesDir.Path;
            var filePath = System.IO.Path.Combine(path, "AccountData.json");
            string edata = JsonConvert.SerializeObject(playerData);
            System.IO.File.Delete(filePath);
            System.IO.File.WriteAllText(filePath, edata);
            Task.Run(() => UpdateAccountRecord()).Wait();
            StartActivity(new Intent(Application.Context, typeof(YouLost)));
            quitButton.Text = "EXIT";
        }



        /* Game Tied Function */

        private void GameTiedButton_Click(object sender, EventArgs e)
        {
            UpdateUI();
            GameOver = true;
            playerData.TiesTwoPlayer = playerData.TiesTwoPlayer + 1;
            playerData.Ties = playerData.Ties + 1;
            var path = Application.Context.FilesDir.Path;
            var filePath = System.IO.Path.Combine(path, "AccountData.json");
            string edata = JsonConvert.SerializeObject(playerData);
            System.IO.File.Delete(filePath);
            System.IO.File.WriteAllText(filePath, edata);
            Task.Run(() => UpdateAccountRecord()).Wait();
            StartActivity(new Intent(Application.Context, typeof(GameTied)));
            quitButton.Text = "EXIT";
        }



        /* Update Account Info To Database */

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
                    return;
                }
                catch (Exception ex)
                {
                    res = "NetworkProblem";
                    return;
                }

            }

        }

        public override void OnBackPressed()
        {

        }
    }
}