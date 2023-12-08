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
using Android.Preferences;

namespace SenMonitorowanie
{
    public class monitoringScreen : Fragment
    {
        private Button _fragmentMonitoringButton;
        private MainActivity _mainActivity;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.monitoringScreen, container, false);
            var preferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            var savedFontPath = preferences.GetString("CurrentFontPath", "");
            if (!string.IsNullOrEmpty(savedFontPath))
            {
                AppSettings.CurrentFontPath = savedFontPath;
                ViewHelper.SetFontForAllViews(view, Activity);
            }
            _mainActivity = (MainActivity)Activity;

            _fragmentMonitoringButton = view.FindViewById<Button>(Resource.Id.startMonitoring);
            _fragmentMonitoringButton.Text = !_mainActivity.IsMonitoring ? "Rozpocznij śledzenie snu" : "Zakończ śledzenie snu";
            _fragmentMonitoringButton.Click += async (sender, e) =>
            {
                _fragmentMonitoringButton.Enabled = false;

                if (!_mainActivity.IsMonitoring)
                {
                    _mainActivity.StartSleepMonitoring();
                    _fragmentMonitoringButton.Text = "Zakończ śledzenie snu";
                    _mainActivity.IsMonitoring = true;
                }
                else
                {
                    _mainActivity.StopSleepMonitoring();
                    _fragmentMonitoringButton.Text = "Rozpocznij śledzenie snu";
                    _mainActivity.IsMonitoring = false;
                }

                await DelayAsync(1000);
                // Odblokuj przycisk po czasie z DelayAsync
                _fragmentMonitoringButton.Enabled = true;
            };            
            return view;
        }
        private async Task DelayAsync(int milliseconds)
        {
            await Task.Delay(milliseconds);
        }
    }
}