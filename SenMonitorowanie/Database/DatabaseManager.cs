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

        public void InsertDaneSensorowe(string dateTime, float accX, float accY, float accZ, float gyrX, float gyrY, float gyrZ, float heartRate)
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

        public double GetMaxHeartRate()
        {
            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                string query = "SELECT MAX(heart_rate) FROM DaneSensorowe";
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

        public double GetMinHeartRate()
        {
            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                string query = "SELECT MIN(heart_rate) FROM DaneSensorowe";
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

        public double GetAverageHeartRate()
        {
            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                string query = "SELECT AVG(heart_rate) FROM DaneSensorowe";
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

        public List<double> GetExtremeHeartRates()
        {
            List<double> extremeHeartRates = new List<double>();

            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                db.BeginTransaction();

                string query =
                    "WITH SmoothedData AS (" +
                    "   SELECT id, " +
                    "          heart_rate, " +
                    "          AVG(heart_rate) OVER (ORDER BY id ROWS BETWEEN 1 PRECEDING AND 1 FOLLOWING) AS smoothed_heart_rate " +
                    "   FROM DaneSensorowe" +
                    ")" +
                    "SELECT sd.smoothed_heart_rate " +
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
                        extremeHeartRates.Add(cursor.GetDouble(cursor.GetColumnIndex("smoothed_heart_rate")));
                    }
                }

                db.SetTransactionSuccessful();
                db.EndTransaction();
            }

            return extremeHeartRates;
        }





    }

}
