using Android.Content;
using Android.Database.Sqlite;
using System.Collections.Generic;

namespace SenMonitor
{
    public class DatabaseManager
    {
        private DatabaseHelper _databaseHelper;

        public DatabaseManager(Context context)
        {
            _databaseHelper = new DatabaseHelper(context);
        }

        public void InsertSensorData(string data)
        {
            using (SQLiteDatabase db = _databaseHelper.WritableDatabase)
            {
                ContentValues values = new ContentValues();
                values.Put("Data", data);

                db.Insert("SensorData", null, values);
            }
        }

        public string GetLatestSensorData()
        {
            string data = null;
            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                string query = "SELECT Data FROM SensorData ORDER BY Id DESC LIMIT 1";
                using (var cursor = db.RawQuery(query, null))
                {
                    if (cursor.MoveToFirst())
                    {
                        data = cursor.GetString(cursor.GetColumnIndex("Data"));
                    }
                }
            }

            return data;
        }

        public List<string> GetLast60SensorData()
        {
            List<string> data = new List<string>();
            using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
            {
                string query = "SELECT Data FROM SensorData ORDER BY Id DESC LIMIT 5";
                using (var cursor = db.RawQuery(query, null))
                {
                    while (cursor.MoveToNext())
                    {
                        data.Add(cursor.GetString(cursor.GetColumnIndex("Data")));
                    }
                }
            }

            return data;
        }

        public void ClearOldData()
        {
            using (SQLiteDatabase db = _databaseHelper.WritableDatabase)
            {
                string query = "DELETE FROM SensorData WHERE Id NOT IN (SELECT Id FROM SensorData ORDER BY Id DESC LIMIT 60)";
                db.ExecSQL(query);
            }
        }

        public void ClearAllData()
        {
            using (SQLiteDatabase db = _databaseHelper.WritableDatabase)
            {
                string query = "DELETE FROM SensorData";
                db.ExecSQL(query);
            }
        }
    }
}
