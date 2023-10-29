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

namespace SenMonitor
{
    [Activity(Label = "SenMonitor", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private SensorManager _sensorManager;
        private DatabaseManager _databaseManager;

        private TextView _accelerometerDataTextView;
        private TextView _volumeLevelTextView;
        private TextView _heartRateTextView; // Deklaracja TextView dla tętna
        private TextView _daneZBazy;
        private AccelerometerHandler _accelerometerHandler;
        private AudioRecorder _audioRecorder;
        private HeartRateSensorHandler _heartRateSensorHandler; // Dodanie obsługi czujnika tętna

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _sensorManager = (SensorManager)GetSystemService(SensorService);
            _databaseManager = new DatabaseManager(this);

            _accelerometerDataTextView = FindViewById<TextView>(Resource.Id.accelerometerDataTextView);
            _volumeLevelTextView = FindViewById<TextView>(Resource.Id.volumeLevelTextView);
            _heartRateTextView = FindViewById<TextView>(Resource.Id.txtHeartRate); // Inicjalizacja TextView dla tętna

            _accelerometerHandler = new AccelerometerHandler(_sensorManager, _accelerometerDataTextView, _databaseManager);
            _audioRecorder = new AudioRecorder(_volumeLevelTextView);
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


            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);

        }

        protected override void OnResume()
        {
            base.OnResume();
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
            // to jest dodane jak wyjdzie i wejdzie

            _accelerometerHandler.StartListening();
            _audioRecorder.StartRecording();
            _heartRateSensorHandler.StartListening(); // Rozpocznij nasłuchiwanie czujnika tętna
        }

        protected override void OnPause()
        {
            base.OnPause();

            _accelerometerHandler.StopListening();
            _audioRecorder.StopRecording();
            _heartRateSensorHandler.StopListening(); // Zatrzymaj nasłuchiwanie czujnika tętna
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            AndroidX.Fragment.App.Fragment selectedFragment = null;
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    SupportFragmentManager.PopBackStack(null, (int)Android.App.PopBackStackFlags.Inclusive);
                    break;
                case Resource.Id.navigation_page2:
                    selectedFragment = new Page2Fragment(); // Utwórz odpowiedni fragment
                    break;
                case Resource.Id.navigation_page3:
                    selectedFragment = new Page3Fragment(); // Utwórz odpowiedni fragment
                    break;
            }

            if (selectedFragment != null)
            {
                SupportFragmentManager.PopBackStack(); // Usuń poprzedni fragment ze stosu wstecz
                SupportFragmentManager.BeginTransaction()
                     .Replace(Resource.Id.container, selectedFragment)
                     .AddToBackStack(null)  // Dodaj ten wiersz
                     .Commit();
            }

            return true;
        }
    }


    

    public class Page2Fragment : AndroidX.Fragment.App.Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_my, container, false);
        }
    }

    public class Page3Fragment : AndroidX.Fragment.App.Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_my2, container, false);
        }
    }
}