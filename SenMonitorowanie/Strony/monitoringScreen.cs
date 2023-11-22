using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SenMonitorowanie
{
    public class monitoringScreen : Fragment
    {
        private Button _fragmentMonitoringButton;
        private MainActivity _mainActivity;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.monitoringScreen, container, false);
            ViewHelper.SetFontForAllViews(view, Activity);

            _mainActivity = (MainActivity)Activity;

            //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();


            // Znajdź przycisk w fragmencie
            _fragmentMonitoringButton = view.FindViewById<Button>(Resource.Id.startMonitoring);
            _fragmentMonitoringButton.Text = !_mainActivity.IsMonitoring ? "Start Sleep Monitoring" : "Stop Sleep Monitoring";
            _fragmentMonitoringButton.Click += async (sender, e) =>
            {
                Console.WriteLine("Button pressed from Fragment");
                _fragmentMonitoringButton.Enabled = false;

                if (!_mainActivity.IsMonitoring)
                {
                    //stopwatch.Start();

                  

                    _mainActivity.StartSleepMonitoring();
                    _fragmentMonitoringButton.Text = "Stop Sleep Monitoring";
                    _mainActivity.IsMonitoring = true;

                }
                else
                {
                    //stopwatch.Stop();
                    //TimeSpan elapsedTime = stopwatch.Elapsed;
                   // Console.WriteLine("Elapsed Time: " + elapsedTime.ToString());

                    _mainActivity.StopSleepMonitoring();
                    _fragmentMonitoringButton.Text = "Start Sleep Monitoring";
                    _mainActivity.IsMonitoring = false;


                   // stopwatch.Reset();


                }

                await DelayAsync(1000);

                // Odblokuj przycisk po 5 sekundach
                _fragmentMonitoringButton.Enabled = true;
            };

            // Metoda asynchroniczna do opóźnienia
            
            return view;
        }
        private async Task DelayAsync(int milliseconds)
        {
            await Task.Delay(milliseconds);
        }
    }
}