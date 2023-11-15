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

namespace SenMonitor
{
    public class Page4Fragment : AndroidX.Fragment.App.Fragment
    {
        private ListView listView;
        private ArrayAdapter<string> adapter; // Zmiana typu adaptera na ArrayAdapter<string>

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_my3, container, false);

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
            DatabaseManager databaseManager = new DatabaseManager(Activity);

            List<BazaSnowData> daneList = databaseManager.GetLast60DaneSnow();

            // Przetwórz dane na odpowiedni format stringa i dodaj do adaptera
            adapter.Add($"{"Data",-6}  {"Długość",3}  {"Ocena",-5}");

            foreach (var dane in daneList)
            {
                int kolumna1 = 10 - dane.Data.Length;

                string formattedData = $"{dane.Data,-15}  {dane.CzasTrwania,-5}  {dane.Ocena,-5}";
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
    }
}