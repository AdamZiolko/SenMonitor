using Android.Hardware;
using Android.Widget;

namespace SenMonitor
{
    public class HeartRateSensorHandler
    {
        private SensorManager _sensorManager;
        private TextView _heartRateTextView;
        private Sensor _heartRateSensor;
        private ISensorEventListener _heartRateSensorListener;

        public HeartRateSensorHandler(SensorManager sensorManager, TextView heartRateTextView)
        {
            _sensorManager = sensorManager;
            _heartRateTextView = heartRateTextView;

            // Pobierz czujnik tętna
            _heartRateSensor = _sensorManager.GetDefaultSensor(SensorType.HeartRate);

            if (_heartRateSensor == null)
            {
                // Obsługa braku dostępnego czujnika tętna
                _heartRateTextView.Text = "Brak dostępnego czujnika tętna";
            }

            // Utwórz słuchacza czujnika tętna
            _heartRateSensorListener = new HeartRateSensorListener(this);
        }

        public void StartListening()
        {
            if (_heartRateSensor != null)
            {
                // Rozpocznij nasłuchiwanie czujnika tętna
                _sensorManager.RegisterListener(_heartRateSensorListener, _heartRateSensor, SensorDelay.Normal);
            }
        }

        public void StopListening()
        {
            if (_heartRateSensor != null)
            {
                // Zatrzymaj nasłuchiwanie czujnika tętna
                _sensorManager.UnregisterListener(_heartRateSensorListener);
            }
        }

        public void UpdateHeartRate(float heartRate)
        {
            // Aktualizuj wartość TextView w interfejsie użytkownika
            _heartRateTextView.Text = "Heart Rate: " + heartRate.ToString();
        }

        // Klasa do obsługi odczytów z czujnika tętna
        private class HeartRateSensorListener : Java.Lang.Object, ISensorEventListener
        {
            private HeartRateSensorHandler _handler;

            public HeartRateSensorListener(HeartRateSensorHandler handler)
            {
                _handler = handler;
            }

            public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
            {
                // Niepotrzebne dla tętna
            }

            public void OnSensorChanged(SensorEvent e)
            {
                if (e.Sensor.Type == SensorType.HeartRate)
                {
                    float heartRate = e.Values[0];
                    _handler.UpdateHeartRate(heartRate);
                }
            }
        }
    }
}
