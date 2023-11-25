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
            int kolejnaCzcionka = 0;

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


    public static class ViewHelper
    {
        public static void SetFontForAllViews(View view, Context context)
        {
            string fontPath = AppSettings.CurrentFontPath;

            if (view is ViewGroup viewGroup)
            {
                int childCount = viewGroup.ChildCount;

                for (int i = 0; i < childCount; i++)
                {
                    Android.Views.View childView = viewGroup.GetChildAt(i);

                    if (childView is Button button)
                    {
                        SetFontForButton(button, context, fontPath);
                    }
                    else if (childView is TextView textView)
                    {
                        SetFontForTextView(textView, context, fontPath);
                    }
                    else if (childView is ListView listView)
                    {
                        SetFontForListView(listView, context, fontPath);
                    }

                    if (childView is ViewGroup)
                    {
                        SetFontForAllViews((ViewGroup)childView, context);
                    }
                }
            }
        }

        private static void SetFontForButton(Button button, Context context, string fontPath)
        {
            Typeface typeface = Typeface.CreateFromAsset(context.Assets, fontPath);
            button.Typeface = typeface;
        }

        private static void SetFontForTextView(TextView textView, Context context, string fontPath)
        {
            Typeface typeface = Typeface.CreateFromAsset(context.Assets, fontPath);
            textView.Typeface = typeface;
        }

        private static void SetFontForListView(ListView listView, Context context, string fontPath)
        {
            //
        }
    }


    public class AppSettings
    {
        private static string _currentFontPath;

        public static string CurrentFontPath
        {
            get => _currentFontPath ?? "fonts/RobotoCondensed-Regular.ttf"; // Domyślna czcionka, jeśli nie ustawiono innej
            set => _currentFontPath = value;
        }
    }


}