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

namespace SenMonitorowanie
{
    [Activity(Label = "SenMonitor", MainLauncher = true)]
    public class MainActivity : WearableActivity//, BottomNavigationView.IOnNavigationItemSelectedListener
    {

        private SensorManager _sensorManager;
        private DatabaseManager _databaseManager;

       // private TextView _accelerometerDataTextView;
        // private TextView _volumeLevelTextView;
        //private TextView _heartRateTextView; // Deklaracja TextView dla tętna
        private AccelerometerHandler _accelerometerHandler;
        //private AudioRecorder _audioRecorder;
        private HeartRateSensorHandler _heartRateSensorHandler; // Dodanie obsługi czujnika tętna
        private GyroscopeSensorHandler _gyroscopeSensorHandler;
        private AmbientTemperatureSensorHandler _ambient;
        private LightSensorHandler _light;

        System.Timers.Timer timer;

        private DateTime startTime;
        DateTime startDate = DateTime.Today;

        public bool IsMonitoring = false;
        //private Button mainMonitoringButton;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _sensorManager = (SensorManager)GetSystemService(SensorService);
            _databaseManager = new DatabaseManager(this);

            //_accelerometerDataTextView = FindViewById<TextView>(Resource.Id.accelerometerDataTextView);
            // _volumeLevelTextView = FindViewById<TextView>(Resource.Id.volumeLevelTextView);
            //_heartRateTextView = FindViewById<TextView>(Resource.Id.txtHeartRate); // Inicjalizacja TextView dla tętna

          _accelerometerHandler = new AccelerometerHandler(_sensorManager, _databaseManager);
           //_audioRecorder = new AudioRecorder(_volumeLevelTextView);
          _heartRateSensorHandler = new HeartRateSensorHandler(_sensorManager, _databaseManager); // Inicjalizacja obsługi czujnika tętna
         _gyroscopeSensorHandler = new GyroscopeSensorHandler(_sensorManager, _databaseManager);
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
            Console.WriteLine("Interval Called");

            List<float> accelerometerData = _accelerometerHandler.GetAccelerometerData();
            float heartRateData = _heartRateSensorHandler.getActualData();
            List<float> gyroscopeData = _gyroscopeSensorHandler.GetGyroscopeData();
            float temperatura = _ambient.GetAmbientTemperatureData();
            float swiatelko = _light.GetLightSensorData();

            DateTime currentDate = DateTime.Now;
            string collectionTime = currentDate.ToString("yyyy-MM-dd H:mm:ss");
            // Console.WriteLine(_databaseManager.GetLatestDane("DaneSensorowe", "heart_rate"));

            //foreach (List<float> data in accelerometerData)
            //{
            lock (databaseLock){
                Console.WriteLine($"Data pobrania : {collectionTime} AX: {accelerometerData[0]}, AY: {accelerometerData[1]}, AZ: {accelerometerData[2]}, heart: {heartRateData}, Gx: {gyroscopeData[0]}, Gy: {gyroscopeData[1]}, Gz: {gyroscopeData[2]},");
                Console.WriteLine($"Poziom temperatury: {temperatura} Poziom światła: {swiatelko}");
                _databaseManager.InsertDaneSensorowe(collectionTime, accelerometerData[0], accelerometerData[1], accelerometerData[2], gyroscopeData[0], gyroscopeData[1], gyroscopeData[2], heartRateData, temperatura, swiatelko);
            }
            //_databaseManager.InsertDaneSensorowe();
            //}
            // Your async logic here.
            //Console.WriteLine($"MAX heartate: {_databaseManager.GetMaxHeartRate()} MIN heartate: {_databaseManager.GetMinHeartRate()} AVG heartate: {_databaseManager.GetAverageHeartRate()}");

            return Task.CompletedTask;
        }

        private bool isTimerRunning = false;

        public void StartSleepMonitoring()
        {
            _databaseManager.ClearAllDaneSensoroweData();

            var serviceIntent = new Intent(this, typeof(MyBackgroundService));
            StartService(serviceIntent);
            // Start listening to sensors
            _accelerometerHandler.StartListening();
            //_audioRecorder.StartRecording();
            _heartRateSensorHandler.StartListening();
            _gyroscopeSensorHandler.StartListening();
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



                string currentDateAsString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                TimeSpan duration = DateTime.Now - startTime;
                int seconds = (int)duration.TotalSeconds;
                int ocenkaSnu = 5;
                int startTimeInSeconds = (int)(startTime - startDate).TotalSeconds;
                int currentTimeInSeconds = (int)(DateTime.Now - startDate).TotalSeconds;


                Console.WriteLine($"MAX heartate: {_databaseManager.GetMax("heart_rate")} MIN heartate: {_databaseManager.GetMin("heart_rate")} AVG heartate: {_databaseManager.GetAverage("heart_rate")}");
                Console.WriteLine($"MAX temperature : {_databaseManager.GetMax("temperature ")} MIN temperature : {_databaseManager.GetMin("temperature ")} AVG temperature : {_databaseManager.GetAverage("temperature ")}");
                Console.WriteLine($"AVG light : {_databaseManager.GetAverage("light")}");
                Dictionary<DateTime, int> extremeSensorDataCount = _databaseManager.GetExtremeSensorDataCountPerHour();
                int iloscRuchow = 0;

                foreach (var entry in extremeSensorDataCount)
                {
                    Console.WriteLine($"Hour: {entry.Key}, Count: {entry.Value}"); /// dzielić przez 2 ilośc ruchów?????????
                    iloscRuchow += entry.Value;    // ilość wszystkich ruchów
                }

                _databaseManager.InsertDaneSnow(currentDateAsString, seconds, ocenkaSnu, startTimeInSeconds, currentTimeInSeconds,
                    (float)_databaseManager.GetAverage("heart_rate"), (float)_databaseManager.GetMax("heart_rate"),
                    (float)_databaseManager.GetMin("heart_rate"), iloscRuchow, (float)_databaseManager.GetMin("temperature "),
                    (float)_databaseManager.GetMax("temperature "), (float)_databaseManager.GetAverage("temperature"), (float)_databaseManager.GetAverage("light")
                );
                // List<Tuple<System.DateTime, double>> extremeHeartRates = _databaseManager.GetExtremeHeartRatesWithDate();
                // _databaseManager.SaveExtremeHeartRatesToDatabase(extremeHeartRates);
                /*
                 foreach (var data in extremeHeartRates)
                 {
                     System.DateTime id = data.Item1;
                     double smoothedHeartRate = data.Item2;
                     Console.WriteLine($"data wystąpienia to: {data.Item1}, a dane serca to {smoothedHeartRate}");

                 }*/




                _accelerometerHandler.StopListening();
                _heartRateSensorHandler.StopListening();
                _gyroscopeSensorHandler.StopListening();
                _ambient.StopListening();
                _light.StopListening();

                var serviceIntent = new Intent(this, typeof(MyBackgroundService));
                StopService(serviceIntent);
            }
        }



        protected override void OnResume()
        {
            base.OnResume();

            
            //_accelerometerHandler.StartListening();
            // _audioRecorder.StartRecording();
           // _heartRateSensorHandler.StartListening(); // Rozpocznij nasłuchiwanie czujnika tętna
        }

        protected override void OnPause()
        {
            base.OnPause();

           //_accelerometerHandler.StopListening();
            // _audioRecorder.StopRecording();
           // _heartRateSensorHandler.StopListening(); // Zatrzymaj nasłuchiwanie czujnika tętna
            //_gyroscopeSensorHandler.StopListening();

        }

        protected override void OnDestroy()
        {
            // Stop the background service
            

            base.OnDestroy();
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        private void SetupFloatingActionButtonMenu()
        {
            var fab = FindViewById<AppCompatImageButton>(Resource.Id.fab);

            bool isMenuOpen = false; // Zmienna śledząca stan menu


            fab.Click += (sender, e) =>
            {
                //Console.WriteLine(_heartRateTextView.Text);

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
                            .SetDuration(500); // Wydłuż czas trwania animacji na 1 sekundę

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


