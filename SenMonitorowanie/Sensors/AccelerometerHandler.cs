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
    private TextView _accelerometerDataTextView;
    private int counter = 0;
    private DatabaseManager _databaseManager;
    public AccelerometerHandler(SensorManager sensorManager, TextView accelerometerDataTextView, DatabaseManager databaseManager)
    {
        _sensorManager = sensorManager;
        _accelerometerDataTextView = accelerometerDataTextView;
        _databaseManager = databaseManager;
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
        // Do something when accuracy of the sensor changes
    }

    public void OnSensorChanged(SensorEvent e)
    {
        if (e.Sensor.Type == SensorType.Accelerometer)
        {
            counter += 1;
            if (counter % 20 == 0 && counter != 0)
            {
                int x = ((int)((e.Values[0] + e.Values[1] + e.Values[2]) * 1000));
                _databaseManager.InsertSensorData(x.ToString());
                List<string> last60Data = _databaseManager.GetLast60SensorData();

                _accelerometerDataTextView.Text = string.Join("\n", last60Data); // Wyświetl dane w jednym TextView, każdy w nowej linii
                counter = 0;
            }
        }

    }
}
