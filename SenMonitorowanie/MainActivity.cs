using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Activity;
using Android.Hardware;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Android.Views.Animations;
using Android.Views;
using static Android.App.FragmentManager;
using AndroidX.AppCompat.Widget;
using Android.Content;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;


namespace SenMonitorowanie
{
    [Activity(Label = "SenMonitor", MainLauncher = true)]
    public class MainActivity : WearableActivity//, BottomNavigationView.IOnNavigationItemSelectedListener
    {

        private SensorManager _sensorManager;
        private DatabaseManager _databaseManager;
        private AccelerometerHandler _accelerometerHandler;
        private HeartRateSensorHandler _heartRateSensorHandler; 
        private AmbientTemperatureSensorHandler _ambient;
        private LightSensorHandler _light;

        System.Timers.Timer timer;

        private DateTime startTime;
        DateTime startDate = DateTime.Today;
        public bool IsMonitoring = false;
        const int BODY_SENSORS_PERMISSION_REQUEST_CODE = 2;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _sensorManager = (SensorManager)GetSystemService(SensorService);
            _databaseManager = new DatabaseManager(this);
            _accelerometerHandler = new AccelerometerHandler(_sensorManager);
            _heartRateSensorHandler = new HeartRateSensorHandler(_sensorManager); 
            _ambient = new AmbientTemperatureSensorHandler(_sensorManager);
            _light = new LightSensorHandler(_sensorManager);

            SetAmbientEnabled();

            FragmentManager fragmentManager = FragmentManager;
            // Rozpoczęcie transakcji fragmentu
            FragmentTransaction fragmentTransaction = fragmentManager.BeginTransaction();
            // Zastąpienie istniejącego fragmentu nowym fragmentem
            fragmentTransaction.Replace(Resource.Id.fragment_container, new monitoringScreen());
            // Dodanie transakcji do back stack (aby można było wrócić do poprzedniego fragmentu przyciskiem "Back")
            fragmentTransaction.AddToBackStack(null);
            // Zatwierdzenie transakcji
            fragmentTransaction.Commit();

            SetupFloatingActionButtonMenu();
        }
        private readonly object databaseLock = new object();

        Task HandleTimerAsync()
        {
            List<float> accelerometerData = _accelerometerHandler.GetAccelerometerData();
            float heartRateData = _heartRateSensorHandler.getActualData();
            float temperatura = _ambient.GetAmbientTemperatureData();
            float swiatelko = _light.GetLightSensorData();
            DateTime currentDate = DateTime.Now;
            string collectionTime = currentDate.ToString("yyyy-MM-dd H:mm:ss");


            lock (databaseLock){
                _databaseManager.InsertDaneSensorowe(
                    collectionTime, 
                    accelerometerData[0], 
                    accelerometerData[1], 
                    accelerometerData[2], 
                    heartRateData, 
                    temperatura, 
                    swiatelko
                );
            }

            return Task.CompletedTask;
        }

        private bool isTimerRunning = false;

        public void StartSleepMonitoring()
        {
            _databaseManager.ClearTable("DaneSensorowe");

            CheckAndRequestBodySensorsPermission();


            var serviceIntent = new Intent(this, typeof(MyBackgroundService));
            StartService(serviceIntent);

            _accelerometerHandler.StartListening();
            _heartRateSensorHandler.StartListening();
            _ambient.StartListening();
            _light.StartListening();

            startTime = DateTime.Now;

            if (isTimerRunning && timer != null)
            {
                timer.Stop();
                timer.Dispose(); // Zwolnienie zasobów starego timera
            }

            if (!isTimerRunning)
            {
                timer = new System.Timers.Timer();
                timer.Interval = 1000; // co jaki czas dane są zbierane w milisekundach
                timer.Elapsed += async (sender, e) => await HandleTimerAsync();
                timer.Start();
                isTimerRunning = true;
            }
        }

        public void StopSleepMonitoring()
        {
            if (isTimerRunning)
            {
                timer.Stop();
                timer.Dispose();
                isTimerRunning = false;

                int identyfikatorMierzenia = _databaseManager.GetMaxIdentifikator();
                _databaseManager.InsertExtremeSensorDataToTable(identyfikatorMierzenia+1);
                _databaseManager.InsertExtremeHeartRatesToTable(identyfikatorMierzenia+1);
                _databaseManager.ClearWykresTable();
                _databaseManager.ClearWykresTable("DaneSerca", "Identifikator");

                string currentDateAsString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                TimeSpan duration = DateTime.Now - startTime;
                int seconds = (int)duration.TotalSeconds;
                int ocenkaSnu = 5;
                int startTimeInSeconds = (int)(startTime - startDate).TotalSeconds;
                int currentTimeInSeconds = (int)(DateTime.Now - startDate).TotalSeconds;

                Dictionary<DateTime, int> extremeSensorDataCount = _databaseManager.GetExtremeSensorDataCountPerHour();
                int iloscRuchow = 0;

                foreach (var entry in extremeSensorDataCount)
                {
                    iloscRuchow += entry.Value;    // ilość wszystkich ruchów
                }

                _databaseManager.InsertDaneSnow(
                    currentDateAsString, 
                    seconds, 
                    ocenkaSnu, 
                    startTimeInSeconds, 
                    currentTimeInSeconds,
                    (float)_databaseManager.GetAverage("heart_rate"), 
                    (float)_databaseManager.GetMax("heart_rate"),
                    (float)_databaseManager.GetMin("heart_rate"), 
                    iloscRuchow, 
                    (float)_databaseManager.GetMin("temperature "),
                    (float)_databaseManager.GetMax("temperature "), 
                    (float)_databaseManager.GetAverage("temperature"), 
                    (float)_databaseManager.GetAverage("light")
                );


                _accelerometerHandler.StopListening();
                _heartRateSensorHandler.StopListening();
                _ambient.StopListening();
                _light.StopListening();

                var serviceIntent = new Intent(this, typeof(MyBackgroundService));
                StopService(serviceIntent);

                _databaseManager.ClearTable("DaneSensorowe");
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnDestroy()
        {
            StopBackgroundService();
            base.OnDestroy();
        }

        private void StopBackgroundService()
        {
            Intent serviceIntent = new Intent(this, typeof(MyBackgroundService));
            StopService(serviceIntent);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        async void CheckAndRequestBodySensorsPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Sensors>();

            if (status != PermissionStatus.Granted)
            {
                var result = await Permissions.RequestAsync<Permissions.Sensors>();

                if (result != PermissionStatus.Granted)
                {
                    // Użytkownik nie udzielił zgody na dostęp do czujników na ciele
                    // Tutaj możesz obsłużyć sytuację, gdy użytkownik nie udzielił zgody
                }
            }
        }


        private void SetupFloatingActionButtonMenu()
        {
            var fab = FindViewById<AppCompatImageButton>(Resource.Id.fab);
            bool isMenuOpen = false; // Zmienna śledząca stan menu

            fab.Click += (sender, e) =>
            {
                fab.Animate().Alpha(0.0f).SetDuration(300);
                if (!isMenuOpen) // Jeśli menu jest zamknięte
                { // Ładuj niestandardowy widok z menu XML
                    View popupView = LayoutInflater.Inflate(Resource.Layout.custom_menu_item, null);
                    PopupWindow popupWindow = new PopupWindow(popupView, ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                    ViewHelper.SetFontForAllViews(popupView, this);

                    popupWindow.ShowAtLocation(fab, GravityFlags.CenterHorizontal, 0, 0);

                    // Przygotowanie popupView do animacji początkowej
                    popupView.Alpha = 0.0f;
                    popupView.ScaleX = 0.0f; // Skalowanie X na zero
                    popupView.ScaleY = 0.0f; // Skalowanie Y na zero
                    popupView.TranslationX = 0;
                    popupView.TranslationY = 100; // Przesunięcie na początek bliżej środka ekranu

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
                            .SetDuration(300); // Wydłuż czas trwania animacji

                    // Dodanie efektu spowolnienia (jedno odbicie)
                    animator.SetInterpolator(new DecelerateInterpolator());


                    popupView.FindViewById(Resource.Id.navigation_home).Click += (s, args) => OpenFragmentOnClick(new monitoringScreen());
                    popupView.FindViewById(Resource.Id.navigation_page2).Click += (s, args) => OpenFragmentOnClick(new Page2Fragment(_databaseManager));
                    popupView.FindViewById(Resource.Id.navigation_page3).Click += (s, args) => OpenFragmentOnClick(new Page3Fragment(_databaseManager));
                    popupView.FindViewById(Resource.Id.navigation_page4).Click += (s, args) => OpenFragmentOnClick(new Page4Fragment(_databaseManager));
                    popupView.FindViewById(Resource.Id.navigation_page5).Click += (s, args) => OpenFragmentOnClick(new Page5Fragment(_databaseManager));

                    Window.DecorView.RootView.Click += (s, args) => {popupWindow.Dismiss();};
                    isMenuOpen = true;

                    void OpenFragmentOnClick(Fragment fragment)
                    {
                        // Pobieranie menedżera fragmentów
                        FragmentManager fragmentManager = FragmentManager;
                        // Rozpoczęcie transakcji fragmentu
                        FragmentTransaction fragmentTransaction = fragmentManager.BeginTransaction();
                        // Zastąpienie istniejącego fragmentu nowym fragmentem
                        fragmentTransaction.Replace(Resource.Id.fragment_container, fragment);
                        // Dodanie transakcji do back stack (aby można było wrócić do poprzedniego fragmentu przyciskiem "Back")
                        fragmentTransaction.AddToBackStack(null);
                        // Zatwierdzenie transakcji
                        fragmentTransaction.Commit();
                        // Możesz dodać tutaj dodatkowe operacje związane z zamknięciem menu kontekstowego i animacjami, jeśli są potrzebne
                        popupWindow.Dismiss();
                        fab.Animate().Alpha(1.0f).SetDuration(00);
                        isMenuOpen = false;
                    }
                }
            };
        }
    }
}