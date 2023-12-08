using Android.Hardware;
using Android.Runtime;

public class LightSensorHandler : Java.Lang.Object, ISensorEventListener
{
    private SensorManager _sensorManager;
    private float _lightSensorData;

    public LightSensorHandler(SensorManager sensorManager)
    {
        _sensorManager = sensorManager;
        _lightSensorData = 0.0f; // Zmieniłem wartość początkową na 0.0f, bo 0.9f wydaje się być nadmiernie wysoką wartością
    }

    public void StartListening()
    {
        _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Light), SensorDelay.Normal);
    }

    public void StopListening()
    {
        _sensorManager.UnregisterListener(this);
    }

    public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
    {
        // Obsługa zmiany dokładności czujnika światła (opcjonalne)
    }

    public float GetLightSensorData()
    {
        return _lightSensorData;
    }

    public void OnSensorChanged(SensorEvent e)
    {
        if (e.Sensor.Type == SensorType.Light)
        {
            float lightIntensity = e.Values[0];

            _lightSensorData = lightIntensity;

        }
    }
}