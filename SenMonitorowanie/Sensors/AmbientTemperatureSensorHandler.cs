using Android.Hardware;
using Android.Runtime;

public class AmbientTemperatureSensorHandler : Java.Lang.Object, ISensorEventListener
{
    private SensorManager _sensorManager;
    private float _ambientTemperatureData;

    public AmbientTemperatureSensorHandler(SensorManager sensorManager)
    {
        _sensorManager = sensorManager;
        _ambientTemperatureData = 0.0f;
    }

    public void StartListening()
    {
        _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.AmbientTemperature), SensorDelay.Normal);
    }

    public void StopListening()
    {
        _sensorManager.UnregisterListener(this);
    }

    public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
    {
        // Handling changes in the accuracy of the ambient temperature sensor (optional)
    }

    public float GetAmbientTemperatureData()
    {
        return _ambientTemperatureData;
    }

    public void OnSensorChanged(SensorEvent e)
    {
        if (e.Sensor.Type == SensorType.AmbientTemperature)
        {
            float ambientTemperature = e.Values[0];
            _ambientTemperatureData = ambientTemperature;
        }
    }
}
