﻿using Android.Hardware;
using Android.Runtime;
using System.Collections.Generic;

public class AccelerometerHandler : Java.Lang.Object, ISensorEventListener
{
    private SensorManager _sensorManager;
    private List<float> _accelerometerData; // Zmień typ na List<List<float>>
    public AccelerometerHandler(SensorManager sensorManager)
    {
        _sensorManager = sensorManager;
        _accelerometerData = new List<float>(); // Zainicjuj jako List<List<float>>
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

    public List<float> GetAccelerometerData() // Zmień zwracaną wartość
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
            _accelerometerData = accelerometerValues;
        }
    }
}
