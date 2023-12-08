using Android.Content;
using Android.Database.Sqlite;
using System;

namespace SenMonitorowanie
{
    public class DatabaseHelper : SQLiteOpenHelper
    {
        private const string DatabaseName = "SenMonitor.db";
        private const int DatabaseVersion = 10; // Zwiększenie wersji bazy danych po dodaniu nowej tabeli

        public DatabaseHelper(Context context) : base(context, DatabaseName, null, DatabaseVersion)
        {
        }

        public override void OnCreate(SQLiteDatabase db)
        {
            // Tworzenie tabeli SensorData, jeśli nie istnieje
            db.ExecSQL("CREATE TABLE IF NOT EXISTS SensorData (Id INTEGER PRIMARY KEY, Data TEXT)");

            // Tworzenie tabeli BazaSnow, jeśli nie istnieje
            db.ExecSQL("CREATE TABLE IF NOT EXISTS BazaSnow (Id INTEGER PRIMARY KEY, Data TEXT, CzasPoczatku INTEGER, CzasZakonczenia INTEGER, CzasTrwania INTEGER, Ocena INTEGER, avg_heart_rate REAL, max_hear_rate REAL, min_heart_rate REAL, move_count INTEGER, min_temp REAL, max_temp REAL, avg_temp REAL, avg_light REAL)");
            db.ExecSQL("CREATE TABLE IF NOT EXISTS IloscRuchow (Id INTEGER PRIMARY KEY, IloscRuchowNaGodzine INTEGER, DataPomiaru TEXT, identyfikatorPomiaru INTEGER)");
            db.ExecSQL("CREATE TABLE IF NOT EXISTS DaneSerca (ID INTEGER PRIMARY KEY, DataCzas TEXT, SmoothedHeartRate REAL, Identifikator INTEGER);");

            // Tworzenie tabeli DaneSensorowe
            CreateDaneSensoroweTable(db);


        }

        private void CreateDaneSensoroweTable(SQLiteDatabase db)
        {
            // Tworzenie tabeli DaneSensorowe
db.ExecSQL("CREATE TABLE IF NOT EXISTS DaneSensorowe (Id INTEGER PRIMARY KEY, date_time TEXT, " +
                   "acc_x REAL, acc_y REAL, acc_z REAL, " +
                   "heart_rate REAL, temperature REAL, light REAL)");
           
    }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            if (oldVersion < 8 && newVersion == 8)
            {
                // Dodaj nowe kolumny do tabeli BazaSnow, gdy wersja bazy danych zostanie zaktualizowana z wersji mniejszej niż 3 do 6
                AddColumnIfNotExists(db, "BazaSnow", "avg_heart_rate", "REAL");
                AddColumnIfNotExists(db, "BazaSnow", "max_hear_rate", "REAL");
                AddColumnIfNotExists(db, "BazaSnow", "min_heart_rate", "REAL");
                AddColumnIfNotExists(db, "BazaSnow", "move_count", "INTEGER");
                AddColumnIfNotExists(db, "DaneSensorowe", "temperature", "REAL");
                AddColumnIfNotExists(db, "DaneSensorowe", "light", "REAL");
                AddColumnIfNotExists(db, "BazaSnow", "avg_temp", "REAL");
                AddColumnIfNotExists(db, "BazaSnow", "min_temp", "REAL");
                AddColumnIfNotExists(db, "BazaSnow", "max_temp", "REAL");
                AddColumnIfNotExists(db, "BazaSnow", "avg_light", "REAL");
            }

            if (oldVersion < 10 && newVersion == 10)
            {
                // Dodaj tabelę DaneSensorowe, gdy wersja bazy danych zostanie zaktualizowana z wersji mniejszej niż 6 do 6
                db.ExecSQL("CREATE TABLE IF NOT EXISTS IloscRuchow (Id INTEGER PRIMARY KEY, IloscRuchowNaGodzine INTEGER, DataPomiaru TEXT, identyfikatorPomiaru INTEGER)");
                db.ExecSQL("CREATE TABLE IF NOT EXISTS DaneSerca (ID INTEGER PRIMARY KEY, DataCzas TEXT, SmoothedHeartRate REAL, Identifikator INTEGER);");


            }
        }

        private void AddColumnIfNotExists(SQLiteDatabase db, string tableName, string columnName, string columnType)
        {
            // Sprawdź, czy kolumna już istnieje
            var query = $"PRAGMA table_info({tableName})";
            using (var cursor = db.RawQuery(query, null))
            {
                while (cursor.MoveToNext())
                {
                    var existingColumnName = cursor.GetString(cursor.GetColumnIndex("name"));
                    if (existingColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        // Kolumna już istnieje, nie dodawaj jej ponownie
                        return;
                    }
                }
            }

            // Dodaj nową kolumnę
            db.ExecSQL($"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnType}");
        }
    }
}