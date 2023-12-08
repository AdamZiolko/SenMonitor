using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Graphics;
using Android.Util;
using Android.Preferences;

namespace SenMonitorowanie
{
    public class Page5Fragment : Fragment
    {
        private DatabaseManager _databaseManager;
        private static int kolejnaCzcionka = 0;
        public Page5Fragment(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_my4, container, false);
            ViewHelper.SetFontForAllViews(view, Activity);

            Button ustawienia = view.FindViewById<Button>(Resource.Id.Ustawione);
            Button czcionka = view.FindViewById<Button>(Resource.Id.toggleFontButton);

            // Dodaj akcję do przycisku
            ustawienia.Click += (sender, e) => {
                _databaseManager.ClearTable("BazaSnow");
                _databaseManager.ClearTable("DaneSerca");
                _databaseManager.ClearTable("IloscRuchow");

                Toast.MakeText(Context, "Dane zostały usunięte.", ToastLength.Short).Show();
            };

            czcionka.Click += (sender, e) => {
                string[] tablicaCzcionek = { 
                    "AlegreyaSans-ExtraBold.ttf", 
                    "Anonymous_Pro.ttf", 
                    "RobotoCondensed-Regular.ttf", 
                    "Mulish-ExtraBold.ttf", 
                    "zai_ConsulPolishTypewriter.ttf" 
                };

                kolejnaCzcionka++;
                int index = kolejnaCzcionka % 5;

                AppSettings.CurrentFontPath = "fonts/" + tablicaCzcionek[index];
                ViewHelper.SetFontForAllViews(view, Activity);

                // Zapisanie informacji o aktualnej czcionce do SharedPreferences
                var preferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
                var editor = preferences.Edit();
                editor.PutString("CurrentFontPath", AppSettings.CurrentFontPath);
                editor.Apply();
            };

            return view;
        }
    }
}