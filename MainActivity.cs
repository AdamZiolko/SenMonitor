using Android.App;
using Android.Content.PM;
using Android.Hardware;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using Google.Android.Material.BottomNavigation;
using System;
using AndroidX.AppCompat.App;
using Android.Content.Res;
using Google.Android.Material.FloatingActionButton;
using Android.Views.Animations;
using System.Text;
using Android.Hardware.Usb;
using AndroidX.Core.Content;
using System.Collections.Generic;

namespace SenMonitor
{
    [Activity(Label = "SenMonitor", MainLauncher = true)]
    public class MainActivity : AppCompatActivity//, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        //public static int ImageResource { get; set; } = Resource.Drawable.domyslny_obraz;

        private SensorManager _sensorManager;
        private DatabaseManager _databaseManager;

        private TextView _accelerometerDataTextView;
       // private TextView _volumeLevelTextView;
        private TextView _heartRateTextView; // Deklaracja TextView dla tętna
        private TextView _daneZBazy;
        private AccelerometerHandler _accelerometerHandler;
        //private AudioRecorder _audioRecorder;
        private HeartRateSensorHandler _heartRateSensorHandler; // Dodanie obsługi czujnika tętna




        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _sensorManager = (SensorManager)GetSystemService(SensorService);
            _databaseManager = new DatabaseManager(this);

            _accelerometerDataTextView = FindViewById<TextView>(Resource.Id.accelerometerDataTextView);
            // _volumeLevelTextView = FindViewById<TextView>(Resource.Id.volumeLevelTextView);
            _heartRateTextView = FindViewById<TextView>(Resource.Id.txtHeartRate); // Inicjalizacja TextView dla tętna

            _accelerometerHandler = new AccelerometerHandler(_sensorManager, _accelerometerDataTextView, _databaseManager);
            // _audioRecorder = new AudioRecorder(_volumeLevelTextView);
            _heartRateSensorHandler = new HeartRateSensorHandler(_sensorManager, _heartRateTextView); // Inicjalizacja obsługi czujnika tętna

            //////////////// Baza danych ///////////////////////////////////////////////////////////////////////////////////////////////
            //string dataToSave = "Twoje dane do zapisania";
            //_databaseManager.InsertSensorData(dataToSave);
            //_databaseManager.InsertSensorData("Dane z bazy danych");
            //string latestData = _databaseManager.GetLatestSensorData();
            //_daneZBazy = FindViewById<TextView>(Resource.Id.daneZBazy);
            //_daneZBazy.Text = latestData;
            _databaseManager.ClearAllData();

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //////////////// Menu Zakładkowe /////////////////////////////////////////////////////


            //BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            //navigation.SetOnNavigationItemSelectedListener(this);



            SetupFloatingActionButtonMenu();

        }

        private void SetupFloatingActionButtonMenu()
        {
            var fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            bool isMenuOpen = false; // Zmienna śledząca stan menu


            fab.Click += (sender, e) =>
            {
                Console.WriteLine(_heartRateTextView.Text);

                fab.Animate().Alpha(0.0f).SetDuration(300);
                if (!isMenuOpen) // Jeśli menu jest zamknięte
                { // Ładuj niestandardowy widok z menu XML
                    View popupView = LayoutInflater.Inflate(Resource.Layout.custom_menu_item, null);
                    PopupWindow popupWindow = new PopupWindow(popupView, ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);


                    int[] location = new int[2];
                    fab.GetLocationOnScreen(location);
                    int x = location[0] - popupWindow.Width; // Wyśrodkuj w poziomie
                    int y = location[1] - popupWindow.Height; // Przesuń menu w górę
                    Console.WriteLine(location[0]);
                    popupWindow.ShowAtLocation(fab, GravityFlags.CenterHorizontal, 0, -60);

                    // Przygotowanie popupView do animacji początkowej
                    popupView.Alpha = 0.0f;
                    popupView.ScaleX = 0.0f; // Skalowanie X na zero
                    popupView.ScaleY = 0.0f; // Skalowanie Y na zero
                    popupView.TranslationX = x;
                    popupView.TranslationY = -y; // Przesunięcie na początek bliżej środka ekranu

                    // Ustawienie punktu odniesienia (pivot point) w środku popupView
                    popupView.PivotX = 0;
                    popupView.PivotY = popupView.Height / 2;

                    // Rozpoczęcie animacji
                    var animator = popupView.Animate();
                    animator.Alpha(1.0f)
                            .ScaleX(1.0f)
                            .ScaleY(1.0f)
                            .TranslationX(0)
                            .TranslationY(0)
                            .SetDuration(500); // Wydłuż czas trwania animacji na 1 sekundę

                    // Dodanie efektu spowolnienia (jedno odbicie)
                    animator.SetInterpolator(new DecelerateInterpolator());

                    // Obsługa kliknięcia opcji
                    popupView.FindViewById(Resource.Id.navigation_home).Click += (s, args) =>
                    {
                        SupportFragmentManager.PopBackStack(null, (int)Android.App.PopBackStackFlags.Inclusive);
                        popupWindow.Dismiss();
                        fab.Animate().Alpha(1.0f).SetDuration(300);
                        isMenuOpen = false;
                    };

                    // Obsługa innych opcji podobnie jak powyżej
                    popupView.FindViewById(Resource.Id.navigation_page2).Click += (s, args) =>
                    {
                        ShowFragment(new Page2Fragment(_databaseManager));
                        popupWindow.Dismiss();
                        fab.Animate().Alpha(1.0f).SetDuration(300);
                        isMenuOpen = false;



                    };

                    popupView.FindViewById(Resource.Id.navigation_page3).Click += (s, args) =>
                    {
                        ShowFragment(new Page3Fragment());
                        popupWindow.Dismiss();
                        fab.Animate().Alpha(1.0f).SetDuration(300);
                        isMenuOpen = false;
                    };

                    popupView.FindViewById(Resource.Id.navigation_page4).Click += (s, args) =>
                    {
                        ShowFragment(new Page4Fragment());
                        popupWindow.Dismiss();
                        fab.Animate().Alpha(1.0f).SetDuration(300);
                        isMenuOpen = false;
                    };

                    popupView.FindViewById(Resource.Id.navigation_page5).Click += (s, args) =>
                    {
                        ShowFragment(new Page5Fragment());
                        popupWindow.Dismiss();
                        fab.Animate().Alpha(1.0f).SetDuration(300);
                        isMenuOpen = false;
                    };

                    // Obsługa zamykania PopupWindow po kliknięciu w inne miejsce
                    Window.DecorView.RootView.Click += (s, args) =>
                    {
                        popupWindow.Dismiss();
                    };
                    isMenuOpen = true;
                }
            };

            void ShowFragment(AndroidX.Fragment.App.Fragment fragment)
            {
                if (fragment != null)
                {
                    SupportFragmentManager.PopBackStack(); // Usuń poprzedni fragment ze stosu wstecz
                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.container, fragment)
                        .AddToBackStack(null)
                        .Commit();
                }
            }
        }


        protected override void OnResume()
        {
            base.OnResume();
            //BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            //navigation.SetOnNavigationItemSelectedListener(this);
            // to jest dodane jak wyjdzie i wejdzie

            _accelerometerHandler.StartListening();
           // _audioRecorder.StartRecording();
            _heartRateSensorHandler.StartListening(); // Rozpocznij nasłuchiwanie czujnika tętna
        }

        protected override void OnPause()
        {
            base.OnPause();

            _accelerometerHandler.StopListening();
           // _audioRecorder.StopRecording();
            _heartRateSensorHandler.StopListening(); // Zatrzymaj nasłuchiwanie czujnika tętna
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        
    }




    public class Page2Fragment : AndroidX.Fragment.App.Fragment
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
                selectedHour = 24  - selectedHour;
                selectedMinute = 60 - selectedMinute;
                czasWMinutach = selectedHour * 60 + selectedHour2*60 + selectedMinute2;
            } else {
                czasWMinutach = selectedHour2*60 + selectedMinute2 - (selectedHour * 60 + selectedMinute);
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
            Console.WriteLine(_databaseManager.GetLatestDane("BazaSnow","Data"));
            Console.WriteLine(_databaseManager.GetLatestDane("BazaSnow", "CzasTrwania"));
            Console.WriteLine(_databaseManager.GetLatestDane("BazaSnow", "Ocena"));


            // Wyświetl wybrane wartości godziny i minuty
            string timeMessage = $"Suma godzin: {godziny}, Suma minut: {minuty}";
            Toast.MakeText(Context, timeMessage, ToastLength.Short).Show();
        }
    }



    public class Page3Fragment : AndroidX.Fragment.App.Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_my2, container, false);
        }
    }

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
            foreach (var dane in daneList)
            {
                string formattedData = $"{dane.Data} - {dane.CzasTrwania} - {dane.Ocena}";
                adapter.Add(formattedData);
            }

            // Powiadom adapter o zmianach
            adapter.NotifyDataSetChanged();
        }
    }

    public class Page5Fragment : AndroidX.Fragment.App.Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_my4, container, false);

            // Znajdź przycisk w fragmencie
            Button ustawienia = view.FindViewById<Button>(Resource.Id.Ustawione);

            // Dodaj akcję do przycisku
            ustawienia.Click += (sender, e) => {
              
              
                Toast.MakeText(Context, "Przycisk został kliknięty", ToastLength.Short).Show();
            };

            return view;
        }
    }

    public class BazaSnowData
    {
        public string Data { get; set; }
        public int CzasTrwania { get; set; }
        public int Ocena { get; set; }
    }





}