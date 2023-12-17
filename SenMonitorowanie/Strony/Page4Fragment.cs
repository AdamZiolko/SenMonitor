using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private ArrayAdapter<string> adapter; 
        private DatabaseManager _databaseManager;
        List<BazaSnowData> daneList;
        Dictionary<DateTime, int> extremeSensorDataCount;
        private int kolejnyDzien = 0;
        private int iloscDanych = 8;
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
            iloscDanych = Math.Min(_databaseManager.GetDistinctIdentifiersCount(), 8);
            ChartView chartView = view.FindViewById<ChartView>(Resource.Id.chartView);
            ChartView chartView2 = view.FindViewById<ChartView>(Resource.Id.chartView2);

            extremeSensorDataCount = _databaseManager.GetIloscRuchowDataPerHour();

            Button wyborDnia = view.FindViewById<Button>(Resource.Id.wyborDnia);

            wyborDnia.Click += async (sender, e) => {
                string[] tablicaMierzen = { 
                    "Ostanie mierzenie", 
                    "Przedstanie mierzenie", 
                    "2 mierzenia temu", 
                    "3 mierzenia temu" , 
                    "4 mierzenia temu" , 
                    "5 mierzeń temu", 
                    "6 mierzeń temu", 
                    "7 mierzeń temu" 
                };
                kolejnyDzien++;
                if (iloscDanych != 0)
                {
                    wyborDnia.Text = tablicaMierzen[kolejnyDzien % iloscDanych];
                    extremeSensorDataCount = _databaseManager.GetIloscRuchowDataPerHour(kolejnyDzien % iloscDanych);

                    LoadChartDataAsync(chartView, kolejnyDzien % iloscDanych);
                    LoadOtherChartDataAsync(chartView2, kolejnyDzien % iloscDanych);
                }
            };

            UpdateListView();

            LoadChartDataAsync(chartView);
            LoadOtherChartDataAsync(chartView2);
            

            return view;
        }

        private async void LoadChartDataAsync(ChartView chartView, int i = 0)
        {
            // Load data asynchronously
            List<Tuple<DateTime, double>> extremeHeartRates = await Task.Run(() => _databaseManager.GetExtremeHeartRatesFromTable(i));
            daneList = _databaseManager.GetLastDaneSnow(60);
            long poczatekSekundy = extremeHeartRates[0].Item1.Ticks / TimeSpan.TicksPerSecond;
            DateTime poczatekCzasuUnixowego = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Obliczanie czasu względnego dla każdego elementu w liście
            extremeHeartRates = extremeHeartRates.Select(t => new Tuple<DateTime, double>(poczatekCzasuUnixowego.AddSeconds((t.Item1.Ticks / TimeSpan.TicksPerSecond) - poczatekSekundy), t.Item2)).ToList();

            DisplayChart(chartView, extremeHeartRates);
        }

        private async void LoadOtherChartDataAsync(ChartView chartView, int i = 0)
        {
            DisplayOtherChart(chartView, extremeSensorDataCount);
        }

        private void UpdateListView()
        {
            daneList = _databaseManager.GetLastDaneSnow(60);
            CustomAdapter adapter = new CustomAdapter(_databaseManager, daneList);
            listView.Adapter = adapter;

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
                (Resource.Id.listaOcena, dataItem => $"Ocena: {dataItem.Ocena}/12"),
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
            // Definicje zestawów kolorów dla różnych pozycji z przezroczystością (ostatnia wartość to przezroczystość)
            int[][] colorSets = {
            new int[] { Color.Rgb(156, 236, 251), Color.Rgb(101, 199, 247), Color.Rgb(0, 82, 212), Color.Argb(0, 0, 0, 0) },
            new int[] { Color.Rgb(52, 148, 230), Color.Rgb(236, 110, 173), Color.Argb(0, 0, 0, 0) },
            new int[] { Color.Rgb(103, 178, 111), Color.Rgb(76, 162, 205), Color.Argb(0, 0, 0, 0) }
            // Dodaj inne zestawy kolorów według potrzeb
        };

            // Ustawienie indeksu zestawu kolorów na podstawie pozycji
            int colorSetIndex = position % colorSets.Length;

            // Utwórz gradient
            GradientDrawable gradient = new GradientDrawable(
                GradientDrawable.Orientation.LeftRight, // Horyzontalny gradient
                colorSets[colorSetIndex]);

            // Ustaw zaokrąglenie narożników (opcjonalne)
            gradient.SetCornerRadius(00f);

            // Ustaw gradient jako tło widoku
            view.Background = gradient;
        }

    }
}