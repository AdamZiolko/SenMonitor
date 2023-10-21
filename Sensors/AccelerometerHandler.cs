using Android.Content;
using Android.Hardware;
using Android.Runtime;
using Android.Widget;
using System;

public class AccelerometerHandler : Java.Lang.Object, ISensorEventListener
{
    private SensorManager _sensorManager;
    private TextView _accelerometerDataTextView;

    public AccelerometerHandler(SensorManager sensorManager, TextView accelerometerDataTextView)
    {
        _sensorManager = sensorManager;
        _accelerometerDataTextView = accelerometerDataTextView;
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
            double x = e.Values[0];
            double y = e.Values[1];
            double z = e.Values[2];

            _accelerometerDataTextView.Text = $"X: {x}\nY: {y}\nZ: {z}";
        }
    }
}
