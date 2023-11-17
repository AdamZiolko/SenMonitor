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

namespace SenMonitorowanie
{
    [Activity(Label = "SenMonitor", MainLauncher = true)]
    public class MainActivity : WearableActivity//, BottomNavigationView.IOnNavigationItemSelectedListener
    {

        private SensorManager _sensorManager;
        private DatabaseManager _databaseManager;

        private TextView _accelerometerDataTextView;
        // private TextView _volumeLevelTextView;
        private TextView _heartRateTextView; // Deklaracja TextView dla tętna
        private AccelerometerHandler _accelerometerHandler;
        //private AudioRecorder _audioRecorder;
        private HeartRateSensorHandler _heartRateSensorHandler; // Dodanie obsługi czujnika tętna
        private GyroscopeSensorHandler _gyroscopeSensorHandler;


        public bool IsMonitoring = false;
        private Button mainMonitoringButton;


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
          _heartRateSensorHandler = new HeartRateSensorHandler(_sensorManager); // Inicjalizacja obsługi czujnika tętna
         _gyroscopeSensorHandler = new GyroscopeSensorHandler(_sensorManager, _databaseManager);
            //////////////// Baza danych ///////////////////////////////////////////////////////////////////////////////////////////////
            //string dataToSave = "Twoje dane do zapisania";
            //_databaseManager.InsertSensorData(dataToSave);
            //_databaseManager.InsertSensorData("Dane z bazy danych");
            //string latestData = _databaseManager.GetLatestSensorData();
            //_daneZBazy = FindViewById<TextView>(Resource.Id.daneZBazy);
            //_daneZBazy.Text = latestData;
            _databaseManager.ClearAllData();
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


            mainMonitoringButton = FindViewById<Button>(Resource.Id.startMonitoring);

            // Initialize sensor handler objects
/*
            mainMonitoringButton.Click += (sender, e) =>
            {
                Console.WriteLine("Button pressed");
                if (!isMonitoring)
                {
                    StartSleepMonitoring();
                }
                else
                {
                    StopSleepMonitoring();
                }
            };*/
        }

        public void StartSleepMonitoring()
        {
            IsMonitoring = true;

            // Start listening to sensors
            _accelerometerHandler.StartListening();
            //_audioRecorder.StartRecording();
            _heartRateSensorHandler.StartListening();
            _gyroscopeSensorHandler.StartListening();
        }

        public void StopSleepMonitoring()
        {
            IsMonitoring = false;

            // Stop listening to sensors
            _accelerometerHandler.StopListening();
            //_audioRecorder.StopRecording();
            _heartRateSensorHandler.StopListening();
            _gyroscopeSensorHandler.StopListening();
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


                    int[] location = new int[2];
                    fab.GetLocationOnScreen(location);
                    int x = location[0] - popupWindow.Width; // Wyśrodkuj w poziomie
                    int y = location[1] - popupWindow.Height; // Przesuń menu w górę
                    
                    popupWindow.ShowAtLocation(fab, GravityFlags.CenterHorizontal, 0, 0);

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


                    popupView.FindViewById(Resource.Id.navigation_home).Click += (s, args) => SwitchToMainActivityOnClick();
                    popupView.FindViewById(Resource.Id.navigation_page2).Click += (s, args) => OpenFragmentOnClick(new Page2Fragment(_databaseManager));
                    popupView.FindViewById(Resource.Id.navigation_page3).Click += (s, args) => OpenFragmentOnClick(new Page3Fragment(_databaseManager));
                    popupView.FindViewById(Resource.Id.navigation_page4).Click += (s, args) => OpenFragmentOnClick(new Page4Fragment(_databaseManager));
                    popupView.FindViewById(Resource.Id.navigation_page5).Click += (s, args) => OpenFragmentOnClick(new Page5Fragment());

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
                        fab.Animate().Alpha(1.0f).SetDuration(300);
                        isMenuOpen = false;
                    }

                    void SwitchToMainActivityOnClick()
                    {
                        Intent intent = new Intent(this, typeof(MainActivity)); // Ustaw właściwy typ Activity
                        StartActivity(intent);
                        Finish(); // Opcjonalnie zamknij bieżącą aktywność, jeśli chcesz
                    }



                }
            };


           
        }
    }


}


