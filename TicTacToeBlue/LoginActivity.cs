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
    public class LoginActivity : AppCompatActivity
    {
        private EditText userNameEditText;
        private EditText passwordEditText;
        private Button login;
        private Button signup;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.LogInScreen);

            userNameEditText = FindViewById<EditText>(Resource.Id.editTextUserName);
            passwordEditText = FindViewById<EditText>(Resource.Id.editTextPassword);
            login = FindViewById<Button>(Resource.Id.buttonLogin);
            signup = FindViewById<Button>(Resource.Id.buttonSignUp);
            login.Click += LoginButtonClicked;
            signup.Click += SignUpButtonClicked;
        }

        private void SignUpButtonClicked(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(SignUpActivity)));
            Finish();
        }
        

        private void LoginButtonClicked(object sender, EventArgs e)
        {

            string userName = userNameEditText.Text.ToLower();
            string password = passwordEditText.Text;
            if(userName.Length<6)
            {
                userNameEditText.Text = "";
                Toast.MakeText(this, "Length of User Name can't be less than 6", ToastLength.Long).Show();
            }
            else if(userName.Contains(" "))
            {
                userNameEditText.Text = "";
                Toast.MakeText(this, "User Name can't contain white spaces", ToastLength.Long).Show();
            }
            else if(password.Length<6)
            {
                passwordEditText.Text = "";
                Toast.MakeText(this, "Length of Password can't be less than 6", ToastLength.Long).Show();
            }
            else if(password.Contains(" "))
            {
                passwordEditText.Text = "";
                Toast.MakeText(this, "Password can't contain white spaces", ToastLength.Long).Show();
            }
            else
            {
                var intent = new Intent(Application.Context, typeof(LoginBackend));
                intent.PutExtra("UserName", userName);
                intent.PutExtra("Password", password);
                StartActivityForResult(intent, 201);
            }
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok && requestCode == 201)
            {
                StartActivity(new Intent(Application.Context, typeof(GameModesActivity)));
                Finish();
            }
            else if (data.GetStringExtra("Error") == "UserName")
            {
                userNameEditText.Text = data.GetStringExtra("UserName");
                passwordEditText.Text = "";
                userNameEditText.ClearFocus();
                passwordEditText.ClearFocus();
                Toast.MakeText(this, "User Name is not registered", ToastLength.Long).Show();
            }
            else if (data.GetStringExtra("Error") == "Password")
            {
                userNameEditText.Text = data.GetStringExtra("UserName");
                passwordEditText.Text = "";
                Toast.MakeText(this, "Password is incorrect", ToastLength.Long).Show();
            }
            else if (data.GetStringExtra("Error") == "Network")
            {
                userNameEditText.Text = data.GetStringExtra("UserName");
                passwordEditText.Text = data.GetStringExtra("Password");
                userNameEditText.ClearFocus();
                passwordEditText.ClearFocus();
                Toast.MakeText(this, "Check Your Internet Connection", ToastLength.Long).Show();
            }
        }
    }
}