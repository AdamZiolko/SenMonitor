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
using Android.Graphics;
using Android.Util;

namespace SenMonitorowanie
{
    [Service]
    public class MyBackgroundService : Service
    {
        private const int ServiceNotificationId = 1; // Unikalne ID powiadomienia dla Foreground Service

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            // Rozpocznij usługę w pierwszym planie
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                StartForegroundOreo();
            }
            else
            {
                StartForeground(ServiceNotificationId, CreateNotification("Zbieranie danych z czujników trwa", "Możesz wyłączyć to w aplikacji"));
            }

            // Zwróć jeden z kodów wyniku StartCommandResult, na przykład:
            return StartCommandResult.Sticky;
        }

        private void StartForegroundOreo()
        {
            // Ustawienie kanału powiadomień dla Androida 8.0 i nowszych
            NotificationChannel channel = new NotificationChannel("channel_id", "Channel Name", NotificationImportance.Default);
            NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);

            // Utworzenie powiadomienia dla Foreground Service
            Notification notification = new Notification.Builder(this, "channel_id")
                .SetContentTitle("Zbieranie danych z czujników trwa")
                .SetContentText("Możesz wyłączyć to w aplikacji")
                .SetSmallIcon(Resource.Drawable.abc_btn_radio_material) // Zastąp "my_icon" odpowiednią nazwą swojego pliku graficznego
                .Build();

            // Rozpocznij usługę w pierwszym planie
            StartForeground(ServiceNotificationId, notification);
        }

        public override void OnDestroy()
        {
            // Zatrzymaj usługę w pierwszym planie, gdy usługa jest zatrzymywana
            StopForeground(true);
            base.OnDestroy();
        }


        private Notification CreateNotification(string title, string content)
        {
            // Utwórz powiadomienie dla Foreground Service
            Notification.Builder builder;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                builder = new Notification.Builder(this, "channel_id");
            }
            else
            {
                builder = new Notification.Builder(this);
            }

            builder.SetContentTitle(title)
                   .SetContentText(content)
                   .SetSmallIcon(Resource.Drawable.icon) // Zastąp "my_icon" odpowiednią nazwą swojego pliku graficznego
                   .Build();

            return builder.Build();
        }
    }
}