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

            // Znajdź przycisk w fragmencie
            Button ustawienia = view.FindViewById<Button>(Resource.Id.Ustawione);
            Button czcionka = view.FindViewById<Button>(Resource.Id.toggleFontButton);

            // Dodaj akcję do przycisku
            ustawienia.Click += (sender, e) => {
                _databaseManager.ClearAllBazaSnowData();
                Toast.MakeText(Context, "Dane zostały usunięte.", ToastLength.Short).Show();
            };

            czcionka.Click += (sender, e) => {
                string[] tablicaCzcionek = { "fonts/AlegreyaSans-ExtraBold.ttf", "fonts/Anonymous_Pro.ttf", "fonts/RobotoCondensed-Regular.ttf", "fonts/Mulish-ExtraBold.ttf", "fonts/zai_ConsulPolishTypewriter.ttf" };
                ++kolejnaCzcionka;
                Console.WriteLine(kolejnaCzcionka % 5);
                AppSettings.CurrentFontPath = tablicaCzcionek[kolejnaCzcionka%5];
                ViewHelper.SetFontForAllViews(view, Activity);

                //Toast.MakeText(Context, "Czcionka została zmieniona.", ToastLength.Short).Show();
            };

            return view;
        }
    }




}