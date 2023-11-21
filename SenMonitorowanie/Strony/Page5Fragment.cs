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

            // Znajdź przycisk w fragmencie
            Button ustawienia = view.FindViewById<Button>(Resource.Id.Ustawione);

            // Dodaj akcję do przycisku
            ustawienia.Click += (sender, e) => {
                _databaseManager.ClearAllBazaSnowData();
                Toast.MakeText(Context, "Dane zostały usunięte.", ToastLength.Short).Show();
            };

            return view;
        }
    }

    public static class FontManager
    {
        public static void SetFont(TextView textView, Context context, string fontPath)
        {
            Typeface typeface = Typeface.CreateFromAsset(context.Assets, fontPath);
            textView.Typeface = typeface;
        }
    }
}