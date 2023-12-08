using Android.Hardware;
using System;

namespace SenMonitorowanie
{
    public class HeartRateSensorHandler
    {
        private SensorManager _sensorManager;
        private HeartRateSensorListener _heartRateSensorListener;

        public HeartRateSensorHandler(SensorManager sensorManager)
        {
            _sensorManager = sensorManager;

            Sensor heartRateSensor = _sensorManager.GetDefaultSensor(SensorType.HeartRate);
            if (heartRateSensor == null)
            {
                Console.WriteLine("Brak dostępnego czujnika tętna");
                return;
            }

            // Utwórz słuchacza czujnika tętna
            _heartRateSensorListener = new HeartRateSensorListener();
        }

        public void StartListening()
        {
            if (_heartRateSensorListener != null)
            {
                // Rozpocznij nasłuchiwanie czujnika tętna
                _sensorManager.RegisterListener(_heartRateSensorListener, _sensorManager.GetDefaultSensor(SensorType.HeartRate), SensorDelay.Normal);
            }
        }

        public void StopListening()
        {
            if (_heartRateSensorListener != null)
            {
                _sensorManager.UnregisterListener(_heartRateSensorListener);
            }
        }

        // Klasa do obsługi odczytów z czujnika tętna
        private class HeartRateSensorListener : Java.Lang.Object, ISensorEventListener
        {
            private float heartRate = 0;
            public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
            {
            }

            public void OnSensorChanged(SensorEvent e)
            {
                if (e.Sensor.Type == SensorType.HeartRate)
                {
                    heartRate = e.Values[0];
                }
            }

            public float getActualData()
            {
                return heartRate;
            }
        }

        public float getActualData()
        {
            return _heartRateSensorListener.getActualData();
        }
    }
}
