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
    public class Page4Fragment : Fragment
    {
        private ListView listView;
        private ArrayAdapter<string> adapter; // Zmiana typu adaptera na ArrayAdapter<string>
        private DatabaseManager _databaseManager;

        public Page4Fragment(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_my3, container, false);
            ViewHelper.SetFontForAllViews(view, Activity);

            // Inicjalizacja ListView i ArrayAdapter<string>
            listView = view.FindViewById<ListView>(Resource.Id.wypisDanych);
            adapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1);
            listView.Adapter = adapter;

            // Wywołaj funkcję do pobrania danych i ustawienia adaptera
            UpdateListView();

            return view;
        }

        private void UpdateListView()
        {
            // Tutaj dostarcz swój DatabaseManager (przykładowo za pomocą konstruktora lub wstrzykiwania zależności)
            DatabaseManager databaseManager = _databaseManager;

            List<BazaSnowData> daneList = databaseManager.GetLast60DaneSnow();

            // Przetwórz dane na odpowiedni format stringa i dodaj do adaptera
            adapter.Add($"{"Data",-6}  {"Długość",3}  {"Ocena",-5}");

            foreach (var dane in daneList)
            {
                int dokładnyCzas = dane.CzasTrwania;
                TimeSpan czasTrwania = TimeSpan.FromSeconds(dokładnyCzas);
                string koncowyCzasTrwania = $"{(int)czasTrwania.TotalHours}:{czasTrwania.Minutes:D2}";
                Console.WriteLine("Czas Poczatku: " + dane.CzasPoczatku);
                Console.WriteLine("Czas Zakończenia: " + dane.CzasZakonczenia);

                string formattedData = $"{dane.Data,-15}  {koncowyCzasTrwania,-5}  {dane.Ocena,-5}";
                adapter.Add(formattedData);
            }

            // Powiadom adapter o zmianach
            adapter.NotifyDataSetChanged();
        }



    }

    public class BazaSnowData
    {
        public string Data { get; set; }
        public int CzasTrwania { get; set; }
        public int Ocena { get; set; }
        public int CzasPoczatku { get; set; } // Dodana właściwość CzasPoczatku
        public int CzasZakonczenia { get; set; } // Dodana właściwość CzasZakonczenia
    }
}