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
using System.Threading;

namespace TicTacToeBlue
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SinglePlayerGameArena : AppCompatActivity, View.IOnTouchListener
    {
        private ImageView[] imageView = new ImageView[10];
        private Button quitButton;
        private Button turnButton;
        private TextView playerNameTextView;
        private TextView symbolTextView;
        private TextView wonTextView;
        private TextView lostTextView;
        private TextView tiedTextView;
        private bool IfMyTurn = false;
        private bool GameOver = false;
        private Button computerTurn = new Button(Application.Context);
        private Button myTurn = new Button(Application.Context);
        private Button YouWonButton = new Button(Application.Context);
        private Button YouLostButton = new Button(Application.Context);
        private Button GameTiedButton = new Button(Application.Context);
        private int filled = 0;
        private char[] box = new char[10] { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };

        private Android.Graphics.Color blueColor = Android.Graphics.Color.ParseColor("#6495ED");
        private Android.Graphics.Color whiteColor = Android.Graphics.Color.ParseColor("#FFFFFF");

        private AccountDataClass playerData;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SingleMain);

            imageView[1] = FindViewById<ImageView>(Resource.Id.imageView1);
            imageView[2] = FindViewById<ImageView>(Resource.Id.imageView2);
            imageView[3] = FindViewById<ImageView>(Resource.Id.imageView3);
            imageView[4] = FindViewById<ImageView>(Resource.Id.imageView4);
            imageView[5] = FindViewById<ImageView>(Resource.Id.imageView5);
            imageView[6] = FindViewById<ImageView>(Resource.Id.imageView6);
            imageView[7] = FindViewById<ImageView>(Resource.Id.imageView7);
            imageView[8] = FindViewById<ImageView>(Resource.Id.imageView8);
            imageView[9] = FindViewById<ImageView>(Resource.Id.imageView9);
            turnButton = FindViewById<Button>(Resource.Id.turn);
            quitButton = FindViewById<Button>(Resource.Id.quit);
            playerNameTextView = FindViewById<TextView>(Resource.Id.playerName);
            symbolTextView = FindViewById<TextView>(Resource.Id.symbol);
            wonTextView = FindViewById<TextView>(Resource.Id.won);
            lostTextView = FindViewById<TextView>(Resource.Id.lost);
            tiedTextView = FindViewById<TextView>(Resource.Id.tied);


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
            computerTurn.Click += ComputerTurn_Click;
            myTurn.Click += MyTurn_Click;
            YouWonButton.Click += YouWonButton_Click;
            YouLostButton.Click += YouLostButton_Click;
            GameTiedButton.Click += GameTiedButton_Click;
            turnButton.Click += TurnButton_Click;

            ThreadPool.QueueUserWorkItem(o => FirstPhase());
        }

        private void MyTurn_Click(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                turnButton.SetBackgroundColor(blueColor);
                turnButton.Text = "Your Turn";
                turnButton.SetTextColor(whiteColor);
                IfMyTurn = true;
            });
        }

        private void TurnButton_Click(object sender, EventArgs e)
        {
            if (GameOver == true)
            {
                GameOver = false;
                turnButton.Text = "Loading...";
                quitButton.Text = "Quit";
                IfMyTurn = false;
                filled = 0;
                box = new char[10] { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
                ThreadPool.QueueUserWorkItem(o => FirstPhase());
            }
        }

        private void ComputerTurn_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(o => CallComputer());
        }

        private void CallComputer()
        {
            int index;
            Thread.Sleep(2000);
            for (int i = 1; i < 10; i++)
            {
                if (box[i] == ' ')
                {
                    box[i] = 'X';
                    int x = gameOver();
                    if (x == 10)
                    {
                        box[i] = 'X';
                        RunOnUiThread(() =>
                        {
                            imageView[i].SetImageResource(Resource.Drawable.Cross);
                            displayResult();
                        });
                        return;
                    }
                    box[i] = ' ';
                }
            }
            var a = AlphaBeta(-100, 100, true);
            index = a[0];
            box[index] = 'X';
            filled++;
            RunOnUiThread(() =>
            {
                imageView[index].SetImageResource(Resource.Drawable.Cross);
            });
            int temp = gameOver();
            if (filled >= 9 || temp != -1)
            {
                RunOnUiThread(() =>
                {
                    displayResult();
                });
                return;
            }
            RunOnUiThread(() =>
            {
                myTurn.PerformClick();
            });
            return;
        }

        private void FirstPhase()
        {

            Random rnd = new Random();
            if (rnd.Next(0, 2) == 0)
            {
                IfMyTurn = true;
            }
            else
            {
                IfMyTurn = false;
            }
            var path = Application.Context.FilesDir.Path;
            var filePath = System.IO.Path.Combine(path, "AccountData.json");
            var edata = System.IO.File.ReadAllText(filePath);
            playerData = JsonConvert.DeserializeObject<AccountDataClass>(edata);
            RunOnUiThread(() =>
            {
                imageView[1].SetImageResource(Resource.Drawable.Blank);
                imageView[2].SetImageResource(Resource.Drawable.Blank);
                imageView[3].SetImageResource(Resource.Drawable.Blank);
                imageView[4].SetImageResource(Resource.Drawable.Blank);
                imageView[5].SetImageResource(Resource.Drawable.Blank);
                imageView[6].SetImageResource(Resource.Drawable.Blank);
                imageView[7].SetImageResource(Resource.Drawable.Blank);
                imageView[8].SetImageResource(Resource.Drawable.Blank);
                imageView[9].SetImageResource(Resource.Drawable.Blank);
                playerNameTextView.Text = playerData.MyName;
                wonTextView.Text = playerData.WinsSinglePlayer.ToString();
                lostTextView.Text = playerData.LosesSinglePlayer.ToString();
                tiedTextView.Text = playerData.TiesSinglePlayer.ToString();
                symbolTextView.Text = "O";
                if (IfMyTurn == true)
                {
                    turnButton.SetBackgroundColor(blueColor);
                    turnButton.Text = "Your Turn";
                    turnButton.SetTextColor(whiteColor);
                }
                else
                {
                    turnButton.SetBackgroundResource(Resource.Drawable.myWhiteButton);
                    turnButton.Text = "Opponent's Turn";
                    turnButton.SetTextColor(blueColor);
                    computerTurn.PerformClick();
                }
            });
        }

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
            }
            Finish();
        }

        public override void OnBackPressed()
        {

        }

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
                                    if (box[i + 1] == ' ')
                                    {
                                        RunOnUiThread(() =>
                                        {
                                            box[i + 1] = 'O';
                                            filled++;
                                            imageView[i + 1].SetImageResource(Resource.Drawable.Circle);
                                            turnButton.SetBackgroundResource(Resource.Drawable.myWhiteButton);
                                            turnButton.Text = "Opponent's Turn";
                                            turnButton.SetTextColor(blueColor);
                                        });
                                        int temp = gameOver();
                                        if (filled >= 9 || temp != -1)
                                        {
                                            RunOnUiThread(() =>
                                            {
                                                displayResult();
                                            });
                                            return true;
                                        }
                                        else
                                        {
                                            computerTurn.PerformClick();
                                            return true;
                                        }
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

        private void displayResult()
        {

            int x = gameOver();
            if (x == 10)
            {
                RunOnUiThread(() =>
                {
                    YouLostButton.PerformClick();
                });
                return;
            }
            else if (x == -10)
            {
                RunOnUiThread(() =>
                {
                    YouWonButton.PerformClick();
                });
                return;
            }
            else
            {
                RunOnUiThread(() =>
                {
                    GameTiedButton.PerformClick();
                });
                return;
            }
        }

        private int gameOver()
        {

            if (box[1] == box[2] && box[2] == box[3] && box[2] != ' ')
                return box[2] == 'X' ? 10 : -10;

            if (box[4] == box[5] && box[5] == box[6] && box[4] != ' ')
                return box[4] == 'X' ? 10 : -10;

            if (box[7] == box[8] && box[8] == box[9] && box[8] != ' ')
                return box[8] == 'X' ? 10 : -10;

            if (box[1] == box[4] && box[4] == box[7] && box[1] != ' ')
                return box[1] == 'X' ? 10 : -10;

            if (box[2] == box[5] && box[5] == box[8] && box[2] != ' ')
                return box[2] == 'X' ? 10 : -10;

            if (box[3] == box[6] && box[6] == box[9] && box[3] != ' ')
                return box[3] == 'X' ? 10 : -10;

            if (box[1] == box[5] && box[5] == box[9] && box[1] != ' ')
                return box[1] == 'X' ? 10 : -10;

            if (box[3] == box[5] && box[5] == box[7] && box[3] != ' ')
                return box[3] == 'X' ? 10 : -10;

            if (filled == 9)
                return 0;

            return -1;
        }

        /* You Won Functions */

        private void YouWonButton_Click(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                GameOver = true;
                playerData.WinsSinglePlayer = playerData.WinsSinglePlayer + 1;
                playerData.Wins = playerData.Wins + 1;
                wonTextView.Text = playerData.WinsSinglePlayer.ToString();
                var path = Application.Context.FilesDir.Path;
                var filePath = System.IO.Path.Combine(path, "AccountData.json");
                string edata = JsonConvert.SerializeObject(playerData);
                System.IO.File.Delete(filePath);
                System.IO.File.WriteAllText(filePath, edata);
                StartActivity(new Intent(Application.Context, typeof(YouWon)));
                turnButton.Text = "RESET";
                quitButton.Text = "EXIT";
            });
        }



        /* You Lost Function */

        private void YouLostButton_Click(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                GameOver = true;
                playerData.LosesSinglePlayer = playerData.LosesSinglePlayer + 1;
                playerData.Loses = playerData.Loses + 1;
                lostTextView.Text = playerData.LosesSinglePlayer.ToString();
                var path = Application.Context.FilesDir.Path;
                var filePath = System.IO.Path.Combine(path, "AccountData.json");
                string edata = JsonConvert.SerializeObject(playerData);
                System.IO.File.Delete(filePath);
                System.IO.File.WriteAllText(filePath, edata);
                StartActivity(new Intent(Application.Context, typeof(YouLost)));
                turnButton.Text = "RESET";
                quitButton.Text = "EXIT";
            });
        }



        /* Game Tied Function */

        private void GameTiedButton_Click(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                GameOver = true;
                playerData.TiesSinglePlayer = playerData.TiesSinglePlayer + 1;
                playerData.Ties = playerData.Ties + 1;
                tiedTextView.Text = playerData.TiesSinglePlayer.ToString();
                var path = Application.Context.FilesDir.Path;
                var filePath = System.IO.Path.Combine(path, "AccountData.json");
                string edata = JsonConvert.SerializeObject(playerData);
                System.IO.File.Delete(filePath);
                System.IO.File.WriteAllText(filePath, edata);
                StartActivity(new Intent(Application.Context, typeof(GameTied)));
                turnButton.Text = "RESET";
                quitButton.Text = "EXIT";
            });
        }


        private int[] AlphaBeta(int alpha, int beta, bool player)
        {

            int x = gameOver();
            int[] a = new int[2];
            int i, j, k, l, index = 0;
            if (x != -1)
            {
                a[0] = -1;
                a[1] = x;
                return a;
            }

            if (player == true)
            {
                int mx = -100;
                for (i = 1; i < 10; i++)
                {
                    if (box[i] == ' ')
                    {
                        box[i] = 'X';
                        filled++;
                        a = AlphaBeta(alpha, beta, !(player));
                        if (mx < a[1])
                        {
                            mx = a[1];
                            index = i;
                            alpha = Math.Max(alpha, mx);

                            if (beta <= alpha)
                            {
                                box[i] = ' ';
                                filled--;
                                break;
                            }
                        }
                        box[i] = ' ';
                        filled--;
                    }
                }
                a[0] = index;
                a[1] = mx;
                return a;
            }
            else
            {
                int mn = 100;
                for (i = 1; i < 10; i++)
                {
                    if (box[i] == ' ')
                    {
                        box[i] = 'O';
                        filled++;
                        a = AlphaBeta(alpha, beta, !(player));
                        if (mn > a[1])
                        {
                            mn = a[1];
                            index = i;
                            beta = Math.Min(beta, mn);
                            if (beta <= alpha)
                            {
                                box[i] = ' ';
                                filled--;
                                break;
                            }
                        }
                        box[i] = ' ';
                        filled--;
                    }
                }
                a[0] = index;
                a[1] = mn;
                return a;
            }
        }
    }
}