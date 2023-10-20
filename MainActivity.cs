using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using System.Threading.Tasks;
using System;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace SenMonitor
{
    [Activity(Label = "SenMonitor", MainLauncher = true)]
    public class MainActivity : Activity, ISensorEventListener
    {
        private SensorManager _sensorManager;
        private TextView _accelerometerDataTextView;
        private AudioRecord _audioRecord;
        private bool _isRecording;
        private TextView _heartRateTextView; // Dodaj pole do wyświetlania tętna

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _sensorManager = (SensorManager)GetSystemService(SensorService);
            _accelerometerDataTextView = FindViewById<TextView>(Resource.Id.accelerometerDataTextView);
            _heartRateTextView = FindViewById<TextView>(Resource.Id.txtHeartRate); // Inicjalizacja pola do wyświetlania tętna

            if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.BodySensors) != Permission.Granted)
            {
                // Jeśli nie ma uprawnień, poproś użytkownika o ich udzielenie
                ActivityCompat.RequestPermissions(this, new string[] { Android.Manifest.Permission.BodySensors }, 1);
            }

        }

        protected override void OnResume()
        {
            base.OnResume();
            _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Normal);
            StartRecordingAudio();

            var heartRateSensor = _sensorManager.GetDefaultSensor(SensorType.HeartRate);

            if (heartRateSensor != null)
            {
                _sensorManager.RegisterListener(this, heartRateSensor, SensorDelay.Normal);
            }
            else
            {
                _heartRateTextView.Text = "Heart rate sensor not available.";
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            _sensorManager.UnregisterListener(this);
            StopRecordingAudio();
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            // Do something when accuracy of the sensor changes
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type == SensorType.Accelerometer)
            {
                double x = e.Values[0];
                double y = e.Values[1];
                double z = e.Values[2];

                _accelerometerDataTextView.Text = $"X: {x}\nY: {y}\nZ: {z}";
            }
            else if (e.Sensor.Type == SensorType.HeartRate)
            {
                float heartRate = e.Values[0];
                _heartRateTextView.Text = $"Heart Rate: {heartRate} bpm";
            }
        }



        private void DisplayVolumeLevel(double volumeLevel)
        {
            // Update UI to display the volume level (you can customize how you want to display it)
            // For example, you can set a progress bar's value or update a TextView
            // Here, we'll just update a TextView
            // Assume you have a TextView with the ID "volumeLevelTextView"
            TextView volumeLevelTextView = FindViewById<TextView>(Resource.Id.volumeLevelTextView);
            volumeLevelTextView.Text = $"Volume Level: {Math.Round(volumeLevel, 3)}";
        }

        private void StartRecordingAudio()
        {
            int minBufferSize = AudioRecord.GetMinBufferSize(44100, ChannelIn.Mono, Encoding.Pcm16bit);
            _audioRecord = new AudioRecord(AudioSource.Mic, 44100, ChannelIn.Mono, Encoding.Pcm16bit, minBufferSize);

            try
            {
                _audioRecord.StartRecording();
                _isRecording = true;

                Task.Run(() => ReadAudioData());
            }
            catch (Exception ex)
            {
                // Obsługa wyjątku
                // Tutaj możesz dodać kod obsługi wyjątku, na przykład wypisanie komunikatu o błędzie
                Console.WriteLine("Exception during audio recording: " + ex.Message);
            }
        }
        private void StopRecordingAudio()
        {
            if (_isRecording)
            {
                _isRecording = false;
                _audioRecord.Stop();
                _audioRecord.Release();
                _audioRecord = null;
            }
        }

        private void ReadAudioData()
        {
            byte[] buffer = new byte[1024];
            while (_isRecording)
            {
                int bytesRead = _audioRecord.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    // Calculate volume level based on the audio data (you may need a more sophisticated method)
                    double volumeLevel = CalculateVolumeLevel(buffer, bytesRead);
                    // Display the volume level
                    RunOnUiThread(() => DisplayVolumeLevel(Math.Round(volumeLevel)));
                }
            }
        }

        private double CalculateVolumeLevel(byte[] audioData, int length)
        {
            // Simple calculation of volume level based on the average amplitude of the audio data
            int sum = 0;
            for (int i = 0; i < length; i += 2)
            {
                short sample = (short)((audioData[i + 1] << 8) | audioData[i]);
                sum += Math.Abs(sample);
            }

            // Calculate the average amplitude
            double averageAmplitude = (double)sum / (length / 2);
            // You may want to scale the averageAmplitude to a suitable range (e.g., 0-100)
            // For simplicity, we'll just use the averageAmplitude directly
            return averageAmplitude;
        }
    }
}