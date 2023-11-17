using Android.Content;
using Android.Database;
using Android.Database.Sqlite;
using Android.Util;
using SenMonitorowanie;
using System.Collections.Generic;

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

        public void InsertSensorData(string data)
        {
            using (SQLiteDatabase db = _databaseHelper.WritableDatabase)
            {
                db.BeginTransaction();
                ContentValues values = new ContentValues();
                values.Put("Data", data);
                db.InsertOrThrow("SensorData", null, values);
                db.SetTransactionSuccessful();
                db.EndTransaction();
            }
        }

        public string GetLatestSensorData()
        {
            string data = null;
            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                db.BeginTransaction();
                string query = "SELECT Data FROM SensorData ORDER BY Id DESC LIMIT 1";
                using (var cursor = db.RawQuery(query, null))
                {
                    if (cursor.MoveToFirst())
                    {
                        data = cursor.GetString(cursor.GetColumnIndex("Data"));
                    }
                }
                db.SetTransactionSuccessful();
                db.EndTransaction();
            }
            return data;
        }

        public string GetLatestDane(string tabela, string kolumna)
        {
            string data = null;
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

        public List<string> GetLast60SensorData()
        {
            List<string> data = new List<string>();
            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                db.BeginTransaction();
                string query = "SELECT Data FROM SensorData ORDER BY Id DESC LIMIT 5";
                using (var cursor = db.RawQuery(query, null))
                {
                    while (cursor.MoveToNext())
                    {
                        data.Add(cursor.GetString(cursor.GetColumnIndex("Data")));
                    }
                }
                db.SetTransactionSuccessful();
                db.EndTransaction();
            }
            return data;
        }

        public void ClearOldData()
        {
            using (SQLiteDatabase db = _databaseHelper.WritableDatabase)
            {
                db.BeginTransaction();
                string query = "DELETE FROM SensorData WHERE Id NOT IN (SELECT Id FROM SensorData ORDER BY Id DESC LIMIT 6)";
                db.ExecSQL(query);
                db.SetTransactionSuccessful();
                db.EndTransaction();
            }
        }

        public void ClearAllData()
        {
            using (SQLiteDatabase db = _databaseHelper.WritableDatabase)
            {
                db.BeginTransaction();
                db.Delete("SensorData", null, null);
                db.SetTransactionSuccessful();
                db.EndTransaction();
            }
        }

        public void InsertDaneSnow(string data, int czasTrwania, int ocena)
        {
            using (SQLiteDatabase db = _databaseHelper.WritableDatabase)
            {
                db.BeginTransaction();
                ContentValues values = new ContentValues();
                values.Put("Data", data);
                values.Put("CzasTrwania", czasTrwania);
                values.Put("Ocena", ocena);
                db.InsertOrThrow("BazaSnow", null, values);  // Zmiana na "BazaSnow" zamiast "SensorData"
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
                string query = "SELECT Data, CzasTrwania, Ocena FROM BazaSnow ORDER BY Id DESC LIMIT 60";
                using (var cursor = db.RawQuery(query, null))
                {
                    while (cursor.MoveToNext())
                    {
                        BazaSnowData dane = new BazaSnowData
                        {
                            Data = cursor.GetString(cursor.GetColumnIndex("Data")),
                            CzasTrwania = cursor.GetInt(cursor.GetColumnIndex("CzasTrwania")),
                            Ocena = cursor.GetInt(cursor.GetColumnIndex("Ocena"))
                        };

                        data.Add(dane);
                    }
                }
                db.SetTransactionSuccessful();
                db.EndTransaction();
            }
            return data;
        }
    }
}
