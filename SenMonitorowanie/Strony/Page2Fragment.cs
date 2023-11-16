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
        private int selectedHour = 6;
        private int selectedMinute = 16;
        private DatabaseManager _databaseManager;

        public Page2Fragment(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_my, container, false);

            // Znajdź TimePicker w widoku fragmentu
            timePicker = view.FindViewById<TimePicker>(Resource.Id.timePicker1);
            timePicker2 = view.FindViewById<TimePicker>(Resource.Id.timePicker2);

            // Ustaw handler dla zdarzenia zmiany czasu
            timePicker.TimeChanged += TimePicker_TimeChanged;

            // Znajdź przycisk w widoku fragmentu
            Button displayTimeButton = view.FindViewById<Button>(Resource.Id.select_button);

            // Ustaw handler dla zdarzenia kliknięcia przycisku
            displayTimeButton.Click += DisplayTimeButton_Click;

            return view;
        }

        private void TimePicker_TimeChanged(object sender, TimePicker.TimeChangedEventArgs e)
        {
            // Zapisz wybrane wartości godziny i minuty
            selectedHour = e.HourOfDay;
            selectedMinute = e.Minute;
        }

        private void DisplayTimeButton_Click(object sender, EventArgs e)
        {
            // Pobierz wartości z drugiego TimePicker
            int selectedHour2 = timePicker2.Hour;
            int selectedMinute2 = timePicker2.Minute;
            int czasWMinutach = 0;
            // Dodaj godziny i minuty z obu TimePickers
            if (selectedHour > selectedHour2)
            {
                selectedHour = 24 - selectedHour;
                selectedMinute = 60 - selectedMinute;
                czasWMinutach = selectedHour * 60 + selectedHour2 * 60 + selectedMinute2;
            }
            else
            {
                czasWMinutach = selectedHour2 * 60 + selectedMinute2 - (selectedHour * 60 + selectedMinute);
            };

            int godziny = (int)Math.Floor((double)czasWMinutach / 60);
            int okraglyCzas = (int)Math.Round((double)czasWMinutach / 60);
            int minuty = czasWMinutach % 60;
            Console.WriteLine(selectedHour);
            Console.WriteLine(selectedHour2);

            DateTime currentDate = DateTime.Now;

            // Konwertuj datę na łańcuch tekstowy w formacie "yyyy-MM-dd"
            string formattedDate = currentDate.ToString("yyyy-MM-dd");
            int ocenaSnu = 5;
            _databaseManager.InsertDaneSnow(formattedDate, okraglyCzas, ocenaSnu);

            // Możesz wyświetlić lub użyć formattedDate w swojej aplikacji
            Console.WriteLine(_databaseManager.GetLatestDane("BazaSnow", "Data"));
            Console.WriteLine(_databaseManager.GetLatestDane("BazaSnow", "CzasTrwania"));
            Console.WriteLine(_databaseManager.GetLatestDane("BazaSnow", "Ocena"));

            // Wyświetl wybrane wartości godziny i minuty
            string timeMessage = $"Suma godzin: {godziny}, Suma minut: {minuty}";
            Toast.MakeText(Context, timeMessage, ToastLength.Short).Show();
        }
    }
}
