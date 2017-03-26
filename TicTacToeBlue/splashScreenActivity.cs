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
using System.Threading.Tasks;
using System.IO;

namespace TicTacToeBlue
{
    [Activity(Theme = "@style/MyTheme.Splash", NoHistory = true,MainLauncher =true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class splashScreenActivity : AppCompatActivity
    {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentBundle)
        {
            base.OnCreate(savedInstanceState, persistentBundle);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() =>
            {
                checkIfLoggedIn();
            });
            startupWork.Start();
        }
        public override void OnBackPressed()
        {
        }

        void checkIfLoggedIn()
        {
            var path = Application.Context.FilesDir.Path;
            var filePath = System.IO.Path.Combine(path, "AccountData.json");
            if (File.Exists(filePath))
            {
                StartActivity(new Intent(Application.Context, typeof(GameModesActivity)));
            }
            else
            {
                StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
            }
        }
    }
}