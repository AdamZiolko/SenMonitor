using Android.Content;
using Android.Hardware;
using Android.Runtime;
using System;
using System.Collections.Generic;

namespace SenMonitorowanie
{
    public class GyroscopeSensorHandler : Java.Lang.Object, ISensorEventListener
    {
        private SensorManager _sensorManager;
        private DatabaseManager _databaseManager;
        private List<float> _gyroscopeData; // Change the type to List<List<float>>

        private int counter = 0;

        public GyroscopeSensorHandler(SensorManager sensorManager, DatabaseManager databaseManager)
        {
            _sensorManager = sensorManager;
            _databaseManager = databaseManager;
            _gyroscopeData = new List<float>(); // Initialize as a List<List<float>>

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

        public List<float> GetGyroscopeData() // Change the return type
        {
            return _gyroscopeData;
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type == SensorType.Gyroscope)
            {
                float x = e.Values[0];
                float y = e.Values[1];
                float z = e.Values[2];

                List<float> gyroscopeValues = new List<float> { x, y, z };

                // Add the gyroscope data to the list
                _gyroscopeData = gyroscopeValues;

                counter += 1;
                if (counter % 20 == 0 && counter != 0)
                {
                    // If you want to insert the average of x, y, and z into the database, you can do something like this:
                   // float average = (x + y + z) / 3;
                  //  _databaseManager.InsertSensorData(average.ToString());

                    // If you want to insert the individual values of x, y, and z into the database, you can do something like this:
                    // _databaseManager.InsertSensorData(x.ToString());
                    // _databaseManager.InsertSensorData(y.ToString());
                    // _databaseManager.InsertSensorData(z.ToString());

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
