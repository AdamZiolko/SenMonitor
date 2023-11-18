using Android.Content;
using Android.Hardware;
using Android.Runtime;
using Android.Widget;
using SenMonitorowanie;
using System;
using System.Collections.Generic;
using System.Reflection;
using Java.Util.Concurrent;

public class AccelerometerHandler : Java.Lang.Object, ISensorEventListener
{
    private SensorManager _sensorManager;
    private List<float> _accelerometerData; // Change the type to List<List<float>>
    private int counter = 0;
    private DatabaseManager _databaseManager;
    public AccelerometerHandler(SensorManager sensorManager, DatabaseManager databaseManager)
    {
        _sensorManager = sensorManager;
       // _accelerometerDataTextView = accelerometerDataTextView;
        _databaseManager = databaseManager;
        _accelerometerData = new List<float>(); // Initialize as a List<List<float>>

    }


    public void StartListening()
    {
        _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Normal);
    }

    public void StopListening()
    {
        _sensorManager.UnregisterListener(this);
    }

    public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
    {
        // Zrób coś, gdy zmieni się dokładność czujnika
    }

    public List<float> GetAccelerometerData() // Change the return type
    {
        return _accelerometerData;
    }

    public void OnSensorChanged(SensorEvent e)
    {
        if (e.Sensor.Type == SensorType.Accelerometer)
        {
            float x = e.Values[0];
            float y = e.Values[1];
            float z = e.Values[2];

            List<float> accelerometerValues = new List<float> { x, y, z };

            // Add the accelerometer data to the list
            _accelerometerData = accelerometerValues;

            counter += 1;
            if (counter % 20 == 0 && counter != 0)
            {
                // If you want to insert the average of x, y, and z into the database, you can do something like this:
                float average = (x + y + z) / 3;
                _databaseManager.InsertSensorData(average.ToString());

                // If you want to insert the individual values of x, y, and z into the database, you can do something like this:
                // _databaseManager.InsertSensorData(x.ToString());
                // _databaseManager.InsertSensorData(y.ToString());
                // _databaseManager.InsertSensorData(z.ToString());

                counter = 0;
            }
        }
    }
}
