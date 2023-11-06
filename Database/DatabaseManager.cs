using Android.Content;
using Android.Database.Sqlite;
using Android.Util;
using System;
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
            try
            {
                using (SQLiteDatabase db = _databaseHelper.WritableDatabase)
                {
                    try
                    {
                        db.BeginTransaction();
                        ContentValues values = new ContentValues();
                        values.Put("Data", data);
                        db.InsertOrThrow("SensorData", null, values);
                        db.SetTransactionSuccessful();
                    }
                    catch (SQLiteException ex)
                    {
                        Log.Error(LogTag, "Błąd podczas wstawiania danych: " + ex.Message);
                    }
                    finally
                    {
                        db.EndTransaction();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Log.Error(LogTag, "Błąd SQLite: " + ex.Message);
            }
        }

        public string GetLatestSensorData()
        {
            string data = null;
            try
            {
                using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
                {
                    try
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
                    }
                    catch (SQLiteException ex)
                    {
                        Log.Error(LogTag, "Błąd podczas pobierania danych: " + ex.Message);
                    }
                    finally
                    {
                        db.EndTransaction();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Log.Error(LogTag, "Błąd SQLite: " + ex.Message);
            }
            return data;
        }

        public List<string> GetLast60SensorData()
        {
            List<string> data = new List<string>();
            try
            {
                using (SQLiteDatabase db = _databaseHelper.ReadableDatabase)
                {
                    try
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
                    }
                    catch (SQLiteException ex)
                    {
                        Log.Error(LogTag, "Błąd podczas pobierania danych: " + ex.Message);
                    }
                    finally
                    {
                        db.EndTransaction();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Log.Error(LogTag, "Błąd SQLite: " + ex.Message);
            }
            return data;
        }

        public void ClearOldData()
        {
            try
            {
                using (SQLiteDatabase db = _databaseHelper.WritableDatabase)
                {
                    try
                    {
                        db.BeginTransaction();
                        string query = "DELETE FROM SensorData WHERE Id NOT IN (SELECT Id FROM SensorData ORDER BY Id DESC LIMIT 6)";
                        db.ExecSQL(query);
                        db.SetTransactionSuccessful();
                    }
                    catch (SQLiteException ex)
                    {
                        Log.Error(LogTag, "Błąd podczas usuwania starych danych: " + ex.Message);
                    }
                    finally
                    {
                        db.EndTransaction();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Log.Error(LogTag, "Błąd SQLite: " + ex.Message);
            }
        }

        public void ClearAllData()
        {
            try
            {
                using (SQLiteDatabase db = _databaseHelper.WritableDatabase)
                {
                    try
                    {
                        db.BeginTransaction();
                        db.Delete("SensorData", null, null);
                        db.SetTransactionSuccessful();
                    }
                    catch (SQLiteException ex)
                    {
                        Log.Error(LogTag, "Błąd podczas usuwania wszystkich danych: " + ex.Message);
                    }
                    finally
                    {
                        db.EndTransaction();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Log.Error(LogTag, "Błąd SQLite: " + ex.Message);
            }
        }
    }
}
