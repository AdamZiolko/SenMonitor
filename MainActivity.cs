using Android.App;
using Android.Content.PM;
using Android.Hardware;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using System;

namespace SenMonitor
{
    [Activity(Label = "SenMonitor", MainLauncher = true)]
    public class MainActivity : Activity
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
            SetContentView(Resource.Layout.activity_main);

            _sensorManager = (SensorManager)GetSystemService(SensorService);
            _databaseManager = new DatabaseManager(this);

            _accelerometerDataTextView = FindViewById<TextView>(Resource.Id.accelerometerDataTextView);
            _volumeLevelTextView = FindViewById<TextView>(Resource.Id.volumeLevelTextView);
            _heartRateTextView = FindViewById<TextView>(Resource.Id.txtHeartRate); // Inicjalizacja TextView dla tętna

            _accelerometerHandler = new AccelerometerHandler(_sensorManager, _accelerometerDataTextView);
            _audioRecorder = new AudioRecorder(_volumeLevelTextView);
            _heartRateSensorHandler = new HeartRateSensorHandler(_sensorManager, _heartRateTextView); // Inicjalizacja obsługi czujnika tętna

            //////////////// Baza danych ///////////////////////////////////////////////////////////////////////////////////////////////
            string dataToSave = "Twoje dane do zapisania";
            _databaseManager.InsertSensorData(dataToSave);
            _databaseManager.InsertSensorData("Dane z bazy danych");
            string latestData = _databaseManager.GetLatestSensorData();
            _daneZBazy = FindViewById<TextView>(Resource.Id.daneZBazy);
            _daneZBazy.Text = latestData;
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }

        protected override void OnResume()
        {
            base.OnResume();

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
    }
}
