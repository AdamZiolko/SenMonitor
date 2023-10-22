using Android.Content;
using Android.Database.Sqlite;

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
                var cursor = db.RawQuery(query, null);

                if (cursor.MoveToFirst())
                {
                    data = cursor.GetString(cursor.GetColumnIndex("Data"));
                }
            }

            return data;
        }
    }
}