using Android.Content;
using Android.Database;
using Android.Database.Sqlite;
using Android.Util;
using SenMonitorowanie;
using System.Collections.Generic;
using System;


namespace SenMonitorowanie
{
    public class DatabaseManager
    {
        private DatabaseHelper _databaseHelper;
        private const string LogTag = "SenMonitor"; // Etykieta dla logów

        public DatabaseManager(Context context)
        {
            _databaseHelper = new DatabaseHelper(context);
        }




        public string GetLatestDane(string tabela, string kolumna)
        {
            string data = "0";
            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                db.BeginTransaction();
                string query = "SELECT " + kolumna + " FROM " + tabela + " ORDER BY Id DESC LIMIT 1";
                using (var cursor = db.RawQuery(query, null))
                {
                    if (cursor.MoveToFirst())
                    {
                        data = cursor.GetString(cursor.GetColumnIndex(kolumna));
                    }
                }
                db.SetTransactionSuccessful();
                db.EndTransaction();
            }
            return data;
        }





        public void InsertDaneSnow(string data, int czasTrwania, int ocena, int czasPoczatku, int czasZakonczenia)
        {
            using (SQLiteDatabase db = _databaseHelper.WritableDatabase)
            {
                db.BeginTransaction();
                ContentValues values = new ContentValues();
                values.Put("Data", data);
                values.Put("CzasTrwania", czasTrwania);
                values.Put("Ocena", ocena);
                values.Put("CzasPoczatku", czasPoczatku); // Dodane do wstawienia CzasPoczatku
                values.Put("CzasZakonczenia", czasZakonczenia); // Dodane do wstawienia CzasZakonczenia
                db.InsertOrThrow("BazaSnow", null, values);
                db.SetTransactionSuccessful();
                db.EndTransaction();
            }
        }

        public void InsertDaneSnow(string data, int czasTrwania, int ocena, int czasPoczatku, int czasZakonczenia,
                                    float avgHeartRate, float maxHeartRate, float minHeartRate, int moveCount,
                                    float minTemp, float maxTemp, float avgTemp, float avgLight)
        {
            using (SQLiteDatabase db = _databaseHelper.WritableDatabase)
            {
                db.BeginTransaction();
                try
                {
                    ContentValues values = new ContentValues();
                    values.Put("Data", data);
                    values.Put("CzasTrwania", czasTrwania);
                    values.Put("Ocena", ocena);
                    values.Put("CzasPoczatku", czasPoczatku);
                    values.Put("CzasZakonczenia", czasZakonczenia);
                    values.Put("avg_heart_rate", avgHeartRate);
                    values.Put("max_hear_rate", maxHeartRate);
                    values.Put("min_heart_rate", minHeartRate);
                    values.Put("move_count", moveCount);
                    values.Put("min_temp", minTemp);
                    values.Put("max_temp", maxTemp);
                    values.Put("avg_temp", avgTemp);
                    values.Put("avg_light", avgLight);

                    db.InsertOrThrow("BazaSnow", null, values);

                    db.SetTransactionSuccessful();
                }
                catch (Exception ex)
                {
                    // Handle exceptions if needed
                }
                finally
                {
                    db.EndTransaction();
                }
            }
        }


        public void InsertDaneSensorowe(string dateTime, float accX, float accY, float accZ, float gyrX, float gyrY, float gyrZ, float heartRate, float temperature, float light)
        {
            using (SQLiteDatabase db = _databaseHelper.WritableDatabase)
            {
                db.BeginTransaction();
                ContentValues values = new ContentValues();
                values.Put("date_time", dateTime);
                values.Put("acc_x", accX);
                values.Put("acc_y", accY);
                values.Put("acc_z", accZ);
                values.Put("gyr_x", gyrX);
                values.Put("gyr_y", gyrY);
                values.Put("gyr_z", gyrZ);
                values.Put("heart_rate", heartRate);
                values.Put("temperature", temperature); // Add temperature column
                values.Put("light", light); // Add light column
                db.InsertOrThrow("DaneSensorowe", null, values);
                db.SetTransactionSuccessful();
                db.EndTransaction();
            }
        }


        public double GetAverageRating()
        {
            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                string query = "SELECT AVG(Ocena) FROM BazaSnow";
                using (var cursor = db.RawQuery(query, null))
                {
                    if (cursor.MoveToFirst())
                    {
                        return cursor.GetDouble(0);
                    }
                }
            }
            return 0; // Zwracamy 0, gdy nie ma rekordów do obliczenia średniej
        }

        public double GetAverageRecordsPerDate()
        {
            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                string query = "SELECT AVG(Cnt) FROM (SELECT COUNT(*) AS Cnt FROM BazaSnow GROUP BY Data)";

                using (var cursor = db.RawQuery(query, null))
                {
                    if (cursor.MoveToFirst())
                    {
                        return cursor.GetDouble(0);
                    }
                }
            }
            return 0; // Zwracamy 0, gdy nie ma rekordów
        }

        public int GetRecordCount()
        {
            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                return (int)DatabaseUtils.QueryNumEntries(db, "BazaSnow");
            }
        }

        public double GetAverageDuration()
        {
            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                string query = "SELECT AVG(CzasTrwania) FROM BazaSnow";
                using (var cursor = db.RawQuery(query, null))
                {
                    if (cursor.MoveToFirst())
                    {
                        return cursor.GetDouble(0);
                    }
                }
            }
            return 0; // Zwracamy 0, gdy nie ma rekordów do obliczenia średniej
        }


        public List<BazaSnowData> GetLast60DaneSnow()
        {
            List<BazaSnowData> data = new List<BazaSnowData>();
            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                db.BeginTransaction();
                string query = "SELECT Data, CzasTrwania, Ocena, CzasPoczatku, CzasZakonczenia FROM BazaSnow ORDER BY Id DESC LIMIT 60";
                using (var cursor = db.RawQuery(query, null))
                {
                    while (cursor.MoveToNext())
                    {
                        BazaSnowData dane = new BazaSnowData
                        {
                            Data = cursor.GetString(cursor.GetColumnIndex("Data")),
                            CzasTrwania = cursor.GetInt(cursor.GetColumnIndex("CzasTrwania")),
                            Ocena = cursor.GetInt(cursor.GetColumnIndex("Ocena")),
                            CzasPoczatku = cursor.GetInt(cursor.GetColumnIndex("CzasPoczatku")),
                            CzasZakonczenia = cursor.GetInt(cursor.GetColumnIndex("CzasZakonczenia"))
                        };

                        data.Add(dane);
                    }
                }
                db.SetTransactionSuccessful();
                db.EndTransaction();
            }
            return data;
        }

        public void ClearAllBazaSnowData()
        {
            using (SQLiteDatabase db = _databaseHelper.WritableDatabase)
            {
                db.BeginTransaction();
                try
                {
                    string clearQuery = "DELETE FROM BazaSnow";
                    db.ExecSQL(clearQuery);
                    db.SetTransactionSuccessful();
                }
                catch (Exception ex)
                {
                    // Obsługa wyjątku, jeśli to konieczne
                    Console.WriteLine($"Error clearing BazaSnow table: {ex.Message}");
                }
                finally
                {
                    db.EndTransaction();
                }
            }
        }


        public void ClearAllDaneSensoroweData()
        {
            using (SQLiteDatabase db = _databaseHelper.WritableDatabase)
            {
                db.BeginTransaction();
                try
                {
                    // Usuwanie wszystkich rekordów z tabeli "DaneSensorowe"
                    db.Delete("DaneSensorowe", null, null);

                    // Ustawianie transakcji jako udanej
                    db.SetTransactionSuccessful();
                }
                catch (Exception ex)
                {
                    // Obsługa błędów, np. logowanie błędu
                    Console.WriteLine("Error clearing DaneSensorowe data: " + ex.Message);
                }
                finally
                {
                    // Zakończenie transakcji
                    db.EndTransaction();
                }
            }
        }

        public double GetMax(string zmienna)
        {
            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                string query = $"SELECT MAX({zmienna}) FROM DaneSensorowe";
                using (var cursor = db.RawQuery(query, null))
                {
                    if (cursor.MoveToFirst())
                    {
                        return cursor.GetDouble(0);
                    }
                }
            }
            return 0; // Return 0 if there are no records
        }

        public double GetMin(string zmienna)
        {
            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                string query = $"SELECT MIN({zmienna}) FROM DaneSensorowe";
                using (var cursor = db.RawQuery(query, null))
                {
                    if (cursor.MoveToFirst())
                    {
                        return cursor.GetDouble(0);
                    }
                }
            }
            return 0; // Return 0 if there are no records
        }

        public double GetAverage(string zmienna)
        {
            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                string query = $"SELECT AVG({zmienna}) FROM DaneSensorowe";
                using (var cursor = db.RawQuery(query, null))
                {
                    if (cursor.MoveToFirst())
                    {
                        return cursor.GetDouble(0);
                    }
                }
            }
            return 0; // Return 0 if there are no records
        }

        public List<Tuple<DateTime, double>> GetExtremeHeartRatesWithDate()
        {
            List<Tuple<DateTime, double>> extremeHeartRates = new List<Tuple<DateTime, double>>();

            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                db.BeginTransaction();

                string query =
                    "WITH SmoothedData AS (" +
                    "   SELECT date_time,id, " +
                    "          heart_rate, " +
                    "          AVG(heart_rate) OVER (ORDER BY id ROWS BETWEEN 1 PRECEDING AND 1 FOLLOWING) AS smoothed_heart_rate " +
                    "   FROM DaneSensorowe" +
                    ")" +
                    "SELECT sd.date_time, sd.smoothed_heart_rate, sd.id " +
                    "FROM SmoothedData AS sd " +
                    "WHERE " +
                    "    (sd.id = (SELECT MIN(id) FROM SmoothedData)) OR " +
                    "    (sd.id = (SELECT MAX(id) FROM SmoothedData)) OR " +
                    "    ((sd.smoothed_heart_rate >= (SELECT smoothed_heart_rate FROM SmoothedData WHERE id = sd.id - 1)) AND " +
                    "     (sd.smoothed_heart_rate > (SELECT smoothed_heart_rate FROM SmoothedData WHERE id = sd.id + 1))) " +
                    "    OR " +
                    "    ((sd.smoothed_heart_rate <= (SELECT smoothed_heart_rate FROM SmoothedData WHERE id = sd.id - 1)) AND " +
                    "     (sd.smoothed_heart_rate < (SELECT smoothed_heart_rate FROM SmoothedData WHERE id = sd.id + 1))) " +
                    "ORDER BY sd.id";

                using (var cursor = db.RawQuery(query, null))
                {
                    while (cursor.MoveToNext())
                    {
                        DateTime dateTime = DateTime.Parse(cursor.GetString(cursor.GetColumnIndex("date_time")));
                        double smoothedHeartRate = cursor.GetDouble(cursor.GetColumnIndex("smoothed_heart_rate"));
                        extremeHeartRates.Add(new Tuple<DateTime, double>(dateTime, smoothedHeartRate));
                    }
                }

                db.SetTransactionSuccessful();
                db.EndTransaction();
            }

            return extremeHeartRates;
        }

        public Dictionary<DateTime, int> GetExtremeSensorDataCountPerHour()
        {
            Dictionary<DateTime, int> extremeSensorDataCount = new Dictionary<DateTime, int>();

            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                db.BeginTransaction();

                string query =
                            "WITH SmoothedSensorData AS (" +
                    "    SELECT " +
                    "        date_time, " +
                    "        id, " +
                    "        acc_x, " +
                    "        AVG(acc_x) OVER (ORDER BY id ROWS BETWEEN 1 PRECEDING AND 1 FOLLOWING) AS smoothed_acc_x, " +
                    "        acc_y, " +
                    "        AVG(acc_y) OVER (ORDER BY id ROWS BETWEEN 1 PRECEDING AND 1 FOLLOWING) AS smoothed_acc_y, " +
                    "        acc_z, " +
                    "        AVG(acc_z) OVER (ORDER BY id ROWS BETWEEN 1 PRECEDING AND 1 FOLLOWING) AS smoothed_acc_z, " +
                    "        gyr_x, " +
                    "        AVG(gyr_x) OVER (ORDER BY id ROWS BETWEEN 1 PRECEDING AND 1 FOLLOWING) AS smoothed_gyr_x, " +
                    "        gyr_y, " +
                    "        AVG(gyr_y) OVER (ORDER BY id ROWS BETWEEN 1 PRECEDING AND 1 FOLLOWING) AS smoothed_gyr_y, " +
                    "        gyr_z, " +
                    "        AVG(gyr_z) OVER (ORDER BY id ROWS BETWEEN 1 PRECEDING AND 1 FOLLOWING) AS smoothed_gyr_z " +
                    "    FROM DaneSensorowe " +
                    ") " +
                    "SELECT " +
                    "    ssd.date_time, " +
                    "    ssd.id " +
                    "FROM SmoothedSensorData AS ssd " +
                    "WHERE " +
                    "    ( " +
                    "        ((ssd.smoothed_acc_x >= ((SELECT smoothed_acc_x FROM SmoothedSensorData WHERE id = ssd.id - 1)) + 2 ) AND " +
                    "         (ssd.smoothed_acc_x > ((SELECT smoothed_acc_x FROM SmoothedSensorData WHERE id = ssd.id + 1))) + 2 ) OR " +
                    "        ((ssd.smoothed_acc_y >= ((SELECT smoothed_acc_y FROM SmoothedSensorData WHERE id = ssd.id - 1)) + 2 ) AND " +
                    "         (ssd.smoothed_acc_y > ((SELECT smoothed_acc_y FROM SmoothedSensorData WHERE id = ssd.id + 1))) + 2 ) OR " +
                    "        ((ssd.smoothed_acc_z >= ((SELECT smoothed_acc_z FROM SmoothedSensorData WHERE id = ssd.id - 1)) + 2 ) AND " +
                    "         (ssd.smoothed_acc_z > ((SELECT smoothed_acc_z FROM SmoothedSensorData WHERE id = ssd.id + 1))) + 2 ) OR " +
                    "        ((ssd.smoothed_gyr_x >= ((SELECT smoothed_gyr_x FROM SmoothedSensorData WHERE id = ssd.id - 1)) + 2 ) AND " +
                    "         (ssd.smoothed_gyr_x > ((SELECT smoothed_gyr_x FROM SmoothedSensorData WHERE id = ssd.id + 1))) + 2 ) OR " +
                    "        ((ssd.smoothed_gyr_y >= ((SELECT smoothed_gyr_y FROM SmoothedSensorData WHERE id = ssd.id - 1)) + 2 ) AND " +
                    "         (ssd.smoothed_gyr_y > ((SELECT smoothed_gyr_y FROM SmoothedSensorData WHERE id = ssd.id + 1))) + 2 ) OR " +
                    "        ((ssd.smoothed_gyr_z >= ((SELECT smoothed_gyr_z FROM SmoothedSensorData WHERE id = ssd.id - 1)) + 2 ) AND " +
                    "         (ssd.smoothed_gyr_z > ((SELECT smoothed_gyr_z FROM SmoothedSensorData WHERE id = ssd.id + 1))) + 2 ) " +
                    "    ) OR " +
                    "    ( " +
                    "        (((ssd.smoothed_acc_x <= (SELECT smoothed_acc_x FROM SmoothedSensorData WHERE id = ssd.id - 1)) - 2) AND " +
                    "         ((ssd.smoothed_acc_x < (SELECT smoothed_acc_x FROM SmoothedSensorData WHERE id = ssd.id + 1))) - 2) OR " +
                    "        (((ssd.smoothed_acc_y <= (SELECT smoothed_acc_y FROM SmoothedSensorData WHERE id = ssd.id - 1)) - 2) AND " +
                    "         ((ssd.smoothed_acc_y < (SELECT smoothed_acc_y FROM SmoothedSensorData WHERE id = ssd.id + 1))) - 2) OR " +
                    "        (((ssd.smoothed_acc_z <= (SELECT smoothed_acc_z FROM SmoothedSensorData WHERE id = ssd.id - 1)) - 2) AND " +
                    "         ((ssd.smoothed_acc_z < (SELECT smoothed_acc_z FROM SmoothedSensorData WHERE id = ssd.id + 1))) - 2) OR " +
                    "        (((ssd.smoothed_gyr_x <= (SELECT smoothed_gyr_x FROM SmoothedSensorData WHERE id = ssd.id - 1)) - 2) AND " +
                    "         ((ssd.smoothed_gyr_x < (SELECT smoothed_gyr_x FROM SmoothedSensorData WHERE id = ssd.id + 1))) - 2) OR " +
                    "        (((ssd.smoothed_gyr_y <= (SELECT smoothed_gyr_y FROM SmoothedSensorData WHERE id = ssd.id - 1)) - 2) AND " +
                    "         ((ssd.smoothed_gyr_y < (SELECT smoothed_gyr_y FROM SmoothedSensorData WHERE id = ssd.id + 1))) - 2) OR " +
                    "        (((ssd.smoothed_gyr_z <= (SELECT smoothed_gyr_z FROM SmoothedSensorData WHERE id = ssd.id - 1)) - 2) AND " +
                    "         ((ssd.smoothed_gyr_z < (SELECT smoothed_gyr_z FROM SmoothedSensorData WHERE id = ssd.id + 1)) - 2) " +
                    "    )) " +
                    "ORDER BY ssd.id";

                using (var cursor = db.RawQuery(query, null))
                {
                    while (cursor.MoveToNext())
                    {
                        DateTime dateTime = DateTime.Parse(cursor.GetString(cursor.GetColumnIndex("date_time")));
                        int id = cursor.GetInt(cursor.GetColumnIndex("id"));

                        // Extract the hour part from the DateTime
                        DateTime hourDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);

                        // Check if the hour is already in the dictionary, if not, add it with a count of 1
                        if (!extremeSensorDataCount.ContainsKey(hourDateTime))
                        {
                            extremeSensorDataCount[hourDateTime] = 1;
                        }
                        else
                        {
                            // Increment the count for the existing hour
                            extremeSensorDataCount[hourDateTime]++;
                        }
                    }
                }

                db.SetTransactionSuccessful();
                db.EndTransaction();
            }

            return extremeSensorDataCount;
        }


    }

    }
