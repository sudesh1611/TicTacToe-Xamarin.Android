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

namespace TicTacToeBlue.Model
{
    public class AccountDataClass
    {
        public int ID { get; set; }
        public string MyName { set; get; }
        public string MyID { set; get; }
        public string Password { get; set; }
        public long Wins { get; set; }
        public long Loses { get; set; }
        public long Ties { get; set; }
        public long WinsTwoPlayer { set; get; }
        public long LosesTwoPlayer { set; get; }
        public long TiesTwoPlayer { set; get; }
        public long WinsSinglePlayer { set; get; }
        public long LosesSinglePlayer { set; get; }
        public long TiesSinglePlayer { set; get; }
    }
}