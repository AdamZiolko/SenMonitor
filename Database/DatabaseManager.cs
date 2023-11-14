using Android.Content;
using Android.Database.Sqlite;
using Android.Util;
using System.Collections.Generic;

namespace SenMonitor
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
    }
}
