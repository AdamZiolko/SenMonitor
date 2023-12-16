using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using Android.App;

namespace SenMonitorowanie
{
    public class Page2Fragment : Fragment
    {
        private TimePicker timePicker;
        private TimePicker timePicker2;
        private int selectedHour = 0;
        private int selectedMinute = 0;
        private DatabaseManager _databaseManager;
        private Button displayTimeButton;
        public Page2Fragment(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_my, container, false);
            ViewHelper.SetFontForAllViews(view, Activity);

            timePicker = view.FindViewById<TimePicker>(Resource.Id.timePicker1);
            timePicker2 = view.FindViewById<TimePicker>(Resource.Id.timePicker2);

            // Ustaw handler dla zdarzenia zmiany czasu
            timePicker.TimeChanged += TimePicker_TimeChanged;

            displayTimeButton = view.FindViewById<Button>(Resource.Id.select_button);

            // Ustaw handler dla zdarzenia kliknięcia przycisku
            displayTimeButton.Click += DisplayTimeButton_Click;

            return view;
        }

        private void TimePicker_TimeChanged(object sender, TimePicker.TimeChangedEventArgs e)
        {
            selectedHour = e.HourOfDay;
            selectedMinute = e.Minute;
        }

        private void DisplayTimeButton_Click(object sender, EventArgs e)
        {
            int selectedHour2 = timePicker2.Hour;
            int selectedMinute2 = timePicker2.Minute;
            int czasWMinutach = 0;
            int czasPoczatku = (selectedHour * 60 + selectedMinute) * 60;
            int czasZakonczenia = (selectedHour2 * 60 + selectedMinute2) * 60;

            if (selectedHour * 60 + selectedMinute >= selectedHour2 * 60 + selectedMinute2 )
            {
                czasWMinutach = (24 * 60 - (selectedHour * 60 + selectedMinute))  + selectedHour2 * 60 + selectedMinute2;
            } else {
                czasWMinutach = selectedHour2 * 60 + selectedMinute2 - (selectedHour * 60 + selectedMinute);
            };

            int okraglyCzas = (int)Math.Round((double)czasWMinutach / 60);
            int czaswSekundach = czasWMinutach * 60;


            DateTime currentDate = DateTime.Now;
            string formattedDate = currentDate.ToString("yyyy-MM-dd H:mm:ss");
            int ocenaSnu = OcenaSnu.EvaluateSleep(
                selectedHour, 
                okraglyCzas, 
                Int32.Parse(_databaseManager.GetLatestDane("BazaSnow", "Ocena"))
            );

            _databaseManager.InsertDaneSnow(formattedDate, czaswSekundach, ocenaSnu, czasPoczatku, czasZakonczenia);
            _databaseManager.KeepLatestRecords();

            TimeSpan czasTrwania = TimeSpan.FromSeconds(czaswSekundach);
            string koncowyCzasTrwania = $"{(int)czasTrwania.TotalHours}:{czasTrwania.Minutes:D2}";
            string timeMessage = $"Suma godzin: {koncowyCzasTrwania}";
            Toast.MakeText(Context, timeMessage, ToastLength.Short).Show();

        }
    }
}
