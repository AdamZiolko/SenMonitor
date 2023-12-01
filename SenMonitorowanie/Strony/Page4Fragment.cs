﻿using Android.App;
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
using Android.Graphics;
using Android.Graphics.Drawables;

namespace SenMonitorowanie
{
    public class Page4Fragment : Fragment
    {
        private ListView listView;
        private ArrayAdapter<string> adapter; // Zmiana typu adaptera na ArrayAdapter<string>
        private DatabaseManager _databaseManager;
        List<BazaSnowData> daneList;
        Dictionary<DateTime, int> extremeSensorDataCount;

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
            ChartView chartView2 = view.FindViewById<ChartView>(Resource.Id.chartView2);

            extremeSensorDataCount = _databaseManager.GetExtremeSensorDataCountPerHour();
            /*Random random = new Random();
            for (int i = 0; i < 3; i++)
            {
                DateTime randomDateTime = DateTime.Now.AddHours(i + 1); // Przyjmuję, że chcesz różne daty
                int randomValue = random.Next(1, 100); // Zakres losowych wartości (możesz dostosować)

                extremeSensorDataCount.Add(randomDateTime, randomValue);
            }*/
            // Wywołaj funkcję do pobrania danych i ustawienia adaptera
            UpdateListView();
            LoadChartDataAsync(chartView);
            LoadOtherChartDataAsync(chartView2);


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

        private async void LoadOtherChartDataAsync(ChartView chartView)
        {
            Console.WriteLine("1");
            // Load data asynchronously for the second chart
            int iloscRuchow = 0;
            Console.WriteLine("111");
            foreach (var entry in extremeSensorDataCount)
            {
                Console.WriteLine($"Hour: {entry.Key}, Count: {entry.Value}"); 
                iloscRuchow += entry.Value;    // ilość wszystkich ruchów
            }
            // Process and transform data if needed
            Console.WriteLine("11");
            DisplayOtherChart(chartView, extremeSensorDataCount);
            Console.WriteLine("112");
        }

        private void UpdateListView()
        {
            // Tutaj dostarcz swój DatabaseManager (przykładowo za pomocą konstruktora lub wstrzykiwania zależności)


            daneList = _databaseManager.GetLast60DaneSnow();
            CustomAdapter adapter = new CustomAdapter(_databaseManager, daneList);
            listView.Adapter = adapter;

            adapter.NotifyDataSetChanged();

            // Przetwórz dane na odpowiedni format stringa i dodaj do adaptera
            //adapter.Add($"{"Data",-6}  {"Długość",3}  {"Ocena",-5}");

           /* foreach (var dane in daneList)
            {
                int dokładnyCzas = dane.CzasTrwania;
                TimeSpan czasTrwania = TimeSpan.FromSeconds(dokładnyCzas);
                string koncowyCzasTrwania = $"{(int)czasTrwania.TotalHours}:{czasTrwania.Minutes:D2}";
                Console.WriteLine("Czas Poczatku: " + dane.CzasPoczatku);
                Console.WriteLine("Czas Zakończenia: " + dane.CzasZakonczenia);

                string formattedData = $"📆: {dane.Data.Substring(5, dane.Data.Length - 8)} \n🕒: {koncowyCzasTrwania}h \nOcena: {dane.Ocena}" +
                    $"\n❤️ średnie: {dane.AvgHeartRate} \n❤️ max: {dane.MaxHeartRate}\n❤️ min: {dane.MinHeartRate} \nIlość ruchów: {dane.MoveCount}\n💡średnie: {dane.AvgLight}" +// < 200 For an optimal sleep environment, light intensity should be below 200 lux 
                    $"\n🌡️ min : {dane.MinTemp}\n🌡️ max: {dane.MaxTemp}\n🌡️ średnia: {dane.AvgTemp}";
                adapter.Add(formattedData);
            }*/

            // Powiadom adapter o zmianach
            adapter.NotifyDataSetChanged();
        }

        private void DisplayOtherChart(ChartView chartView, Dictionary<DateTime, int> data)
        {
            var entries = new List<ChartEntry>();
            int labelInterval = data.Count < 8 ? 1 : data.Count / 8;

            int i = 0;
            var colors = new List<SKColor>
            {
                SKColor.Parse("#2196F3"),
                SKColor.Parse("#4CAF50"),
                SKColor.Parse("#FFC107"),
                // Dodaj więcej kolorów według potrzeb
            };

            foreach (var entry in data)
            {
                int colorIndex = i % colors.Count;
                entries.Add(new ChartEntry(entry.Value)
                {
                    Label = i % labelInterval == 0 ? entry.Key.ToString("HH:mm") : string.Empty,
                    ValueLabel = i % labelInterval == 0 ? entry.Value.ToString() : string.Empty,
                    Color = colors[colorIndex],
                    TextColor = colors[colorIndex],
                });

                i++;
            }

            var chart = new BarChart
            {
                Entries = entries,
                BarAreaAlpha = 200,
                LabelTextSize = 19,
            };

            chartView.Chart = chart;
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
        public int CzasPoczatku { get; set; }
        public int CzasZakonczenia { get; set; }
        public float AvgHeartRate { get; set; }
        public float MinHeartRate { get; set; }
        public float MaxHeartRate { get; set; }
        public int MoveCount { get; set; }
        public float MinTemp { get; set; }
        public float MaxTemp { get; set; }
        public float AvgTemp { get; set; }
        public float AvgLight { get; set; }
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

            List<(int, Func<BazaSnowData, string>)> textViewMappings = new List<(int, Func<BazaSnowData, string>)>
            {
                (Resource.Id.textViewData, dataItem => $"📆: {dataItem.Data.Substring(5, dataItem.Data.Length - 8)}"),
                (Resource.Id.listaOcena, dataItem => $"Ocena: {dataItem.Ocena}"),
                (Resource.Id.lista_min_heart, dataItem => $"💙 min: {Convert.ToInt32(dataItem.MinHeartRate)}"),
                (Resource.Id.lista_max_heart, dataItem => $"💚 max: {Convert.ToInt32(dataItem.MaxHeartRate)}"),
                (Resource.Id.lista_avg_heart, dataItem => $"💖 średnie: {Convert.ToInt32(dataItem.AvgHeartRate)}"),
                (Resource.Id.lista_min_temp, dataItem => $"🌡️ min: {Convert.ToInt32(dataItem.MinTemp)}"),
                (Resource.Id.lista_max_temp, dataItem => $"🌡️ max: {Convert.ToInt32(dataItem.MaxTemp)}"),
                (Resource.Id.lista_avg_temp, dataItem => $"🌡️ średnia: {Convert.ToInt32(dataItem.AvgTemp)}"),
                (Resource.Id.lista_avg_light, dataItem => $"💡średnie: {Convert.ToInt32(dataItem.AvgLight)}"),
                (Resource.Id.lista_move_count, dataItem => $"Ilość ruchów: {Convert.ToInt32(dataItem.MoveCount)}")

            };

            BazaSnowData dataItem = _data[position];

            foreach (var mapping in textViewMappings)
            {
                TextView textView = view.FindViewById<TextView>(mapping.Item1);
                textView.Text = mapping.Item2.Invoke(dataItem);
            }

            SetGradientBackground(view, position);


            return view;
        }

        private void SetGradientBackground(View view, int position)
        {
            // Definicje zestawów kolorów dla różnych pozycji
            int[][] colorSets = {
                new int[] { Color.Rgb(156, 236, 251), Color.Rgb(101, 199, 247), Color.Rgb(0, 82, 212) },
                new int[] { Color.Rgb(52, 148, 230), Color.Rgb(236, 110, 173 )},
                new int[] { Color.Rgb(103, 178, 111), Color.Rgb(76, 162, 205) }
                // Dodaj inne zestawy kolorów według potrzeb
            };

            // Ustawienie indeksu zestawu kolorów na podstawie pozycji
            int colorSetIndex = position % colorSets.Length;

            // Utwórz gradient
            GradientDrawable gradient = new GradientDrawable(
                GradientDrawable.Orientation.LeftRight, // Horyzontalny gradient
                colorSets[colorSetIndex]);

            // Ustaw zaokrąglenie narożników (opcjonalne)
            gradient.SetCornerRadius(0f);

            // Ustaw gradient jako tło widoku
            view.Background = gradient;
        }


    }



}