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
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class TwoPlayersOfflineGameArena : AppCompatActivity, View.IOnTouchListener
    {
        private ImageView[] imageView = new ImageView[10];
        private Button quitButton;
        private Button turnButton;
        private TextView winsPlayer1;
        private TextView winsPlayer2;
        private TextView ties;
        private bool GameOver = false;
        private bool IfPlayer1Turn = false;
        private Button WonButton = new Button(Application.Context);
        private Button TieButton = new Button(Application.Context);
        private bool IfPlayer1Wins = false;
        private Button CheckGameStatus = new Button(Application.Context);
        private int Player1Wins = 0;
        private int Player2Wins = 0;
        private int TiesPlayer = 0;
        private bool canPlayGame = true;
        private char[] box = new char[10] { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };

        private Android.Graphics.Color blueColor = Android.Graphics.Color.ParseColor("#6495ED");
        private Android.Graphics.Color whiteColor = Android.Graphics.Color.ParseColor("#FFFFFF");

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TwoPlayersOffline);

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
            winsPlayer1 = FindViewById<TextView>(Resource.Id.winsPlayer1);
            winsPlayer2 = FindViewById<TextView>(Resource.Id.winsPlayer2);
            ties = FindViewById<TextView>(Resource.Id.ties);

            imageView[1].SetOnTouchListener(this);
            imageView[2].SetOnTouchListener(this);
            imageView[3].SetOnTouchListener(this);
            imageView[4].SetOnTouchListener(this);
            imageView[5].SetOnTouchListener(this);
            imageView[6].SetOnTouchListener(this);
            imageView[7].SetOnTouchListener(this);
            imageView[8].SetOnTouchListener(this);
            imageView[9].SetOnTouchListener(this);

            quitButton = FindViewById<Button>(Resource.Id.quit);
            turnButton = FindViewById<Button>(Resource.Id.turn);
            quitButton.Click += QuitButton_Click;
            turnButton.Click += TurnButton_Click;
            WonButton.Click += WonButton_Click;
            TieButton.Click += TieButton_Click;
            CheckGameStatus.Click += CheckGameStatus_Click;
            FirstPhase();
        }

        private void TurnButton_Click(object sender, EventArgs e)
        {
            if (GameOver == true)
            {
                GameOver = false;
                turnButton.Text = "Loading...";
                quitButton.Text = "Quit";
                IfPlayer1Turn = false;
                canPlayGame = true;
                box = new char[10] { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
                FirstPhase();
            }
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

        private void CheckGameStatus_Click(object sender, EventArgs e)
        {
            char tempSymbol;
            if (IfPlayer1Turn == true)
            {
                tempSymbol = 'X';
                IfPlayer1Wins = true;
            }
            else
            {
                IfPlayer1Wins = false;
                tempSymbol = 'O';
            }
            if (box[1] == tempSymbol && box[2] == tempSymbol && box[3] == tempSymbol)
            {
                WonButton.PerformClick();
                return;
            }
            else if(box[4] == tempSymbol && box[5] == tempSymbol && box[6] == tempSymbol)
            {
                WonButton.PerformClick();
                return;
            }
            else if (box[7] == tempSymbol && box[8] == tempSymbol && box[9] == tempSymbol)
            {
                WonButton.PerformClick();
                return;
            }
            else if (box[1] == tempSymbol && box[4] == tempSymbol && box[7] == tempSymbol)
            {
                WonButton.PerformClick();
                return;
            }
            else if (box[2] == tempSymbol && box[5] == tempSymbol && box[8] == tempSymbol)
            {
                WonButton.PerformClick();
                return;
            }
            else if (box[3] == tempSymbol && box[6] == tempSymbol && box[9] == tempSymbol)
            {
                WonButton.PerformClick();
                return;
            }
            else if (box[1] == tempSymbol && box[5] == tempSymbol && box[9] == tempSymbol)
            {
                WonButton.PerformClick();
                return;
            }
            else if (box[3] == tempSymbol && box[5] == tempSymbol && box[7] == tempSymbol)
            {
                WonButton.PerformClick();
                return;
            }
            else if(box[1]!=' '&& box[2] != ' ' && box[3] != ' ' && box[4] != ' ' && box[5] != ' ' && box[6] != ' ' && box[7] != ' ' && box[8] != ' ' && box[9] != ' ')
            {
                TieButton.PerformClick();
                return;
            }
            else
            {
                if(IfPlayer1Turn==true)
                {
                    IfPlayer1Turn = false;
                }
                else
                {
                    IfPlayer1Turn = true;
                }
            }
        }

        private void TieButton_Click(object sender, EventArgs e)
        {
            GameOver = true;
            canPlayGame = false;
            TiesPlayer = TiesPlayer + 1;
            var intent = new Intent(Application.Context, typeof(GameTied));
            StartActivity(intent);
            ties.Text = TiesPlayer.ToString();
            turnButton.Text = "RESET";
            quitButton.Text = "EXIT";
        }

        private void WonButton_Click(object sender, EventArgs e)
        {
            GameOver = true;
            canPlayGame = false;
            var intent = new Intent(Application.Context, typeof(TwoPlayerWon));
            if (IfPlayer1Wins == true)
            {
                Player1Wins = Player1Wins + 1;
                winsPlayer1.Text = Player1Wins.ToString();
                intent.PutExtra("Player", "One");
            }
            else
            {
                Player2Wins = Player2Wins + 1;
                winsPlayer2.Text = Player2Wins.ToString();
                intent.PutExtra("Player", "Two");
            }
            StartActivity(intent);

            turnButton.Text = "RESET";
            quitButton.Text = "EXIT";

        }

        private void FirstPhase()
        {
            Random rnd = new Random();
            if (rnd.Next(0, 2) == 0)
            {
                IfPlayer1Turn = true;
            }
            else
            {
                IfPlayer1Turn = false;
            }
            imageView[1].SetImageResource(Resource.Drawable.Blank);
            imageView[2].SetImageResource(Resource.Drawable.Blank);
            imageView[3].SetImageResource(Resource.Drawable.Blank);
            imageView[4].SetImageResource(Resource.Drawable.Blank);
            imageView[5].SetImageResource(Resource.Drawable.Blank);
            imageView[6].SetImageResource(Resource.Drawable.Blank);
            imageView[7].SetImageResource(Resource.Drawable.Blank);
            imageView[8].SetImageResource(Resource.Drawable.Blank);
            imageView[9].SetImageResource(Resource.Drawable.Blank);
            winsPlayer1.Text = Player1Wins.ToString();
            winsPlayer2.Text = Player2Wins.ToString();
            ties.Text = TiesPlayer.ToString(); ;
            if (IfPlayer1Turn == true)
            {
                turnButton.SetBackgroundColor(blueColor);
                turnButton.Text = "Player 1";
                turnButton.SetTextColor(whiteColor);
            }
            else
            {
                turnButton.SetBackgroundResource(Resource.Drawable.myWhiteButton);
                turnButton.Text = "Player 2";
                turnButton.SetTextColor(blueColor);
            }
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (canPlayGame == true)
            {
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
                                        if (IfPlayer1Turn == true)
                                        {
                                            box[i + 1] = 'X';
                                            imageView[i + 1].SetImageResource(Resource.Drawable.Cross);
                                            turnButton.SetBackgroundResource(Resource.Drawable.myWhiteButton);
                                            turnButton.Text = "Player 2";
                                            turnButton.SetTextColor(blueColor);
                                        }
                                        else
                                        {
                                            box[i + 1] = 'O';
                                            imageView[i + 1].SetImageResource(Resource.Drawable.Circle);
                                            turnButton.SetBackgroundColor(blueColor);
                                            turnButton.Text = "Player 1";
                                            turnButton.SetTextColor(whiteColor);
                                        }
                                        CheckGameStatus.PerformClick();
                                        return true;
                                    }
                                    else
                                    {
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
    }
}