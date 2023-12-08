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

namespace SenMonitorowanie
{
    public class OcenaSnu
    {
        public static int EvaluateSleep(int selectedHour, int okraglyCzas, int poprzedniaOcena, int iloscRuchow = 0, double avgTemp = 0.0, double avgLight = 0.0,float minHeartRate = 0.0f, float maxHeartRate = 0.0f)
        {
            int ocenaSnu = 0;

            // Kryteria dotyczące ogólnej jakości snu
            if (selectedHour >= 22 && selectedHour <= 20) ocenaSnu += 2; // Odpowiednia pora snu (22:00 - 7:00)
            if (okraglyCzas >= 7 && okraglyCzas <= 8) ocenaSnu += 2; // Odpowiedni czas snu (7-8 godzin)
            if (poprzedniaOcena >= ocenaSnu) ocenaSnu += 1; // Punkty za utrzymanie oceny snu z dnia poprzedniego

            // Dodatkowe kryteria oceny snu
            if (iloscRuchow <= 55 && iloscRuchow >= 35) ocenaSnu += 2; // The average sleeper moves about 40 to 50 times a night andthis number changes in certain situations. For example, sleep deprivationresults in a sleep with fewer movements.1 kwi 2006
            if (avgTemp >= 15 && avgTemp <= 24) ocenaSnu += 2; // Optymalna średnia temperatura podczas snu (19-21 stopni Celsiusza)
            if (minHeartRate >= 40 && maxHeartRate <= 100) ocenaSnu += 1; // Optymalne tetno podczas snu
            if (avgLight <= 80) ocenaSnu += 2; // Bardzo niskie natężenie światła 

            return ocenaSnu;
        }
    }
}
