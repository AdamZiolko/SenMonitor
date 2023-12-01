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


namespace SenMonitorowanie
{
    public class Page3Fragment : Fragment
    {

        private DatabaseManager _databaseManager;

        public Page3Fragment(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_my2, container, false);
            ViewHelper.SetFontForAllViews(view, Activity);

            view.FindViewById<TextView>(Resource.Id.sredniaCzasu).Text = $"Średni czas spania: {(_databaseManager.GetAverage("CzasTrwania", "BazaSnow")/3600).ToString("F2")} godzin";
            view.FindViewById<TextView>(Resource.Id.sredniaOcena).Text = $"Średnia ocena spania: {(_databaseManager.GetAverage("Ocena", "BazaSnow")).ToString("F2")}";
            view.FindViewById<TextView>(Resource.Id.iloscSnow).Text = $"Mierzenie spania: {_databaseManager.GetRecordCount()} razy";
            view.FindViewById<TextView>(Resource.Id.senNaDzien).Text = $"Średnia ilość spania na dzień: {(_databaseManager.GetAverageRecordsPerDate()).ToString("F2")}";

            // Example: Display a line chart

            return view;
        }

    }
}