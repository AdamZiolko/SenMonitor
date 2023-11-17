﻿using Android.Hardware;
using System;

namespace SenMonitorowanie
{
    public class HeartRateSensorHandler
    {
        private SensorManager _sensorManager;
        private ISensorEventListener _heartRateSensorListener;

        public HeartRateSensorHandler(SensorManager sensorManager)
        {
            _sensorManager = sensorManager;

            // Pobierz czujnik tętna
            Sensor heartRateSensor = _sensorManager.GetDefaultSensor(SensorType.HeartRate);
            Console.WriteLine("HeartRateSensorHandler");
            if (heartRateSensor == null)
            {
                // Obsługa braku dostępnego czujnika tętna
                Console.WriteLine("Brak dostępnego czujnika tętna");
                return;
            }

            // Utwórz słuchacza czujnika tętna
            _heartRateSensorListener = new HeartRateSensorListener();
        }

        public void StartListening()
        {
            Console.WriteLine("StartListeningPoczatek");

            if (_heartRateSensorListener != null)
            {
                Console.WriteLine("StartListeningKoniec");

                // Rozpocznij nasłuchiwanie czujnika tętna
                _sensorManager.RegisterListener(_heartRateSensorListener, _sensorManager.GetDefaultSensor(SensorType.HeartRate), SensorDelay.Normal);
            }
        }

        public void StopListening()
        {
            Console.WriteLine("StopListeningPoczatek");

            if (_heartRateSensorListener != null)
            {
                Console.WriteLine("StopListeningKoniec");

                // Zatrzymaj nasłuchiwanie czujnika tętna
                _sensorManager.UnregisterListener(_heartRateSensorListener);
            }
        }

        // Klasa do obsługi odczytów z czujnika tętna
        private class HeartRateSensorListener : Java.Lang.Object, ISensorEventListener
        {
            public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
            {
                // Niepotrzebne dla tętna
            }

            public void OnSensorChanged(SensorEvent e)
            {
                Console.WriteLine("czujnikSercaDziała");
                if (e.Sensor.Type == SensorType.HeartRate)
                {
                    float heartRate = e.Values[0];
                    UpdateHeartRate(heartRate);
                }
            }

            private void UpdateHeartRate(float heartRate)
            {
                // Aktualizuj wartość na konsoli
                Console.WriteLine("Heart Rate: " + heartRate.ToString());
            }
        }
    }
}
