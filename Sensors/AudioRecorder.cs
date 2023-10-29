using Android.Media;
using System;
using System.Threading.Tasks;
using Android.Widget;

public class AudioRecorder
{
    private AudioRecord _audioRecord;
    private bool _isRecording;
    private TextView _volumeLevelTextView;

    public AudioRecorder(TextView volumeLevelTextView)
    {
        _volumeLevelTextView = volumeLevelTextView;
    }

    public void StartRecording()
    {
        int minBufferSize = AudioRecord.GetMinBufferSize(44100, ChannelIn.Mono, Encoding.Pcm16bit);
        _audioRecord = new AudioRecord(AudioSource.Mic, 44100, ChannelIn.Mono, Encoding.Pcm16bit, minBufferSize);

        try
        {
            _audioRecord.StartRecording();
            _isRecording = true;

            Task.Run(() => ReadAudioData());
        }
        catch (Exception ex)
        {
            // Obsługa wyjątku
            Console.WriteLine("Exception during audio recording: " + ex.Message);
        }
    }

    public void StopRecording()
    {
        if (_isRecording)
        {
            _isRecording = false;
            _audioRecord.Stop();
            _audioRecord.Release();
            _audioRecord = null;
        }
    }

    private void ReadAudioData()
    {
        byte[] buffer = new byte[1024];
        while (_isRecording)
        {
            int bytesRead = _audioRecord.Read(buffer, 0, buffer.Length);
            if (bytesRead > 0)
            {
                double volumeLevel = CalculateVolumeLevel(buffer, bytesRead);
                DisplayVolumeLevel(Math.Round(volumeLevel));
            }
        }
    }

    private double CalculateVolumeLevel(byte[] audioData, int length)
    {
        int sum = 0;
        for (int i = 0; i < length; i += 2)
        {
            short sample = (short)((audioData[i + 1] << 8) | audioData[i]);
            sum += Math.Abs(sample);
        }

        double averageAmplitude = (double)sum / (length / 2);
        return averageAmplitude;
    }

    private void DisplayVolumeLevel(double volumeLevel)
    {
        _volumeLevelTextView.Text = $"Volume Level: {volumeLevel}";
    }
}
