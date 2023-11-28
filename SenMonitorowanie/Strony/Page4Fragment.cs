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
using System.Runtime.InteropServices;
using Microcharts.Droid;
using Microcharts;
using SkiaSharp;
using System.Threading.Tasks;

namespace SenMonitorowanie
{
    public class Page4Fragment : Fragment
    {
        private ListView listView;
        private ArrayAdapter<string> adapter; // Zmiana typu adaptera na ArrayAdapter<string>
        private DatabaseManager _databaseManager;
        List<BazaSnowData> daneList;


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

            ChartView chartView = view.FindViewById<ChartView>(Resource.Id.chartView);


            // Wywołaj funkcję do pobrania danych i ustawienia adaptera
            UpdateListView();
            LoadChartDataAsync(chartView);


            return view;
        }

        private async void LoadChartDataAsync(ChartView chartView)
        {
            // Load data asynchronously
            List<Tuple<DateTime, double>> extremeHeartRates = await Task.Run(() => _databaseManager.GetExtremeHeartRatesWithDate());
            daneList = _databaseManager.GetLast60DaneSnow();
            long poczatekSekundy = extremeHeartRates[0].Item1.Ticks / TimeSpan.TicksPerSecond;


            DateTime poczatekCzasuUnixowego = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


            // Obliczanie czasu względnego dla każdego elementu w liście
            extremeHeartRates = extremeHeartRates.Select(t => new Tuple<DateTime, double>(poczatekCzasuUnixowego.AddSeconds((t.Item1.Ticks / TimeSpan.TicksPerSecond) - poczatekSekundy), t.Item2)).ToList();

            // Display chart
            DisplayChart(chartView, extremeHeartRates);
        }


        private void UpdateListView()
        {
            // Tutaj dostarcz swój DatabaseManager (przykładowo za pomocą konstruktora lub wstrzykiwania zależności)
            DatabaseManager databaseManager = _databaseManager;

            List<Tuple<DateTime, double>> extremeHeartRates = databaseManager.GetExtremeHeartRatesWithDate();

            daneList = _databaseManager.GetLast60DaneSnow();
            //CustomAdapter adapter = new CustomAdapter(_databaseManager, daneList);
            //listView.Adapter = adapter;

            //adapter.NotifyDataSetChanged();

            // Przetwórz dane na odpowiedni format stringa i dodaj do adaptera
            //adapter.Add($"{"Data",-6}  {"Długość",3}  {"Ocena",-5}");

            foreach (var dane in daneList)
            {
                int dokładnyCzas = dane.CzasTrwania;
                TimeSpan czasTrwania = TimeSpan.FromSeconds(dokładnyCzas);
                string koncowyCzasTrwania = $"{(int)czasTrwania.TotalHours}:{czasTrwania.Minutes:D2}";
                Console.WriteLine("Czas Poczatku: " + dane.CzasPoczatku);
                Console.WriteLine("Czas Zakończenia: " + dane.CzasZakonczenia);

                string formattedData = $"Data: {dane.Data.Substring(5, dane.Data.Length - 8)} \nCzas trwania: {koncowyCzasTrwania}h \nOcena: {dane.Ocena}" +
                    $"\nŚrednie tętno: 45 \nMax tętno: 324 15:47\nMin tętno: -34 \nZnacznych zmian tętna: 32 \nIlość ruchów: 321";
                adapter.Add(formattedData);
            }

            // Powiadom adapter o zmianach
            adapter.NotifyDataSetChanged();
        }

        private void DisplayChart(ChartView chartView, List<Tuple<DateTime, double>> data)
        {
            var entries = new List<ChartEntry>();
            int labelInterval = data.Count < 8 ? 7 : data.Count / 8; // Wybierz co ile punktów wyświetlić etykietę


            for (int i = 0; i < data.Count; i++)
            {
                var tuple = data[i];
                int intValue = (int)tuple.Item2; // Konwersja double na int

                entries.Add(new ChartEntry(intValue)
                {
                    Label = i % labelInterval == 0 ? tuple.Item1.ToString("HH:mm") : string.Empty,
                    ValueLabel = i % labelInterval == 0 ? intValue.ToString() : string.Empty,
                    Color = SKColor.Parse("#FF1493"),
                }); 

            }

            var chart = new LineChart { Entries = entries };
            //chart.LabelOrientation = Microcharts.Orientation.Horizontal;

            chartView.Chart = chart;

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

    public class CustomAdapter : BaseAdapter<BazaSnowData>
    {
        private readonly List<BazaSnowData> _data;
        private readonly DatabaseManager _databaseManager;

        public CustomAdapter(DatabaseManager databaseManager, List<BazaSnowData> data)
        {
            _databaseManager = databaseManager;
            _data = data;
        }

        public override int Count => _data.Count;

        public override BazaSnowData this[int position] => _data[position];

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView ?? LayoutInflater.From(parent.Context).Inflate(Resource.Layout.clitem, parent, false);

            TextView textViewData = view.FindViewById<TextView>(Resource.Id.textViewData);
            ChartView chartView = view.FindViewById<ChartView>(Resource.Id.chartView);

            textViewData.Text = $"Data: {_data[position].Data}";

            // Call the GetExtremeHeartRatesWithDate method using the DatabaseManager instance
            List<Tuple<DateTime, double>> extremeHeartRates = _databaseManager.GetExtremeHeartRatesWithDate();

            // Pass the correct data for the chart
            DisplayChart(chartView, extremeHeartRates);

            return view;
        }

        private void DisplayChart(ChartView chartView, List<Tuple<DateTime, double>> data)
        {
            var entries = new List<ChartEntry>();

            foreach (var tuple in data)
            {
                entries.Add(new ChartEntry((float)tuple.Item2)
                {
                    Label = tuple.Item1.ToString(),
                    ValueLabel = tuple.Item2.ToString(),
                    Color = SKColor.Parse("#FF1493"),
                });
            }

            var chart = new LineChart { Entries = entries };
            chart.LabelOrientation = Microcharts.Orientation.Horizontal;
            chartView.SetBackgroundColor(Android.Graphics.Color.Blue);

            chartView.Chart = chart;
        }
    }



}