using Android.Content;
using Android.Hardware;
using Android.Runtime;
using System;

namespace SenMonitor.Sensors
{
    public class GyroscopeSensorHandler : Java.Lang.Object, ISensorEventListener
    {
        private SensorManager _sensorManager;
        private DatabaseManager _databaseManager;
        private int counter = 0;

        public GyroscopeSensorHandler(SensorManager sensorManager, DatabaseManager databaseManager)
        {
            _sensorManager = sensorManager;
            _databaseManager = databaseManager;

            // Initialize gyroscope sensor
            Sensor gyroscopeSensor = _sensorManager.GetDefaultSensor(SensorType.Gyroscope);

            if (gyroscopeSensor != null)
            {
                _sensorManager.RegisterListener(this, gyroscopeSensor, SensorDelay.Fastest);
            }
            else
            {
                Console.WriteLine("Gyroscope sensor not available");
            }
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            // Do something when accuracy of the sensor changes
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type == SensorType.Gyroscope)
            {
                counter += 1;
                if (counter % 20 == 0 && counter != 0)
                {
                    float x = e.Values[0];
                    float y = e.Values[1];
                    float z = e.Values[2];

                    // Handle gyroscope data as needed
                    Console.WriteLine($"Gyroscope Data\nX: {x}\nY: {y}\nZ: {z}");

                    // You can also save data to the database if needed
                    // _databaseManager.InsertGyroscopeData(x, y, z);

                    counter = 0;
                }
            }
        }

        public void StartListening()
        {
            _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Gyroscope), SensorDelay.Normal);
        }

        public void StopListening()
        {
            _sensorManager.UnregisterListener(this);
        }
    }
}
