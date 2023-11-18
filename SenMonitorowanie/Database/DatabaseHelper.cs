﻿using Android.Content;
using Android.Database.Sqlite;

namespace SenMonitorowanie
{
    public class DatabaseHelper : SQLiteOpenHelper
    {
        private const string DatabaseName = "SenMonitor.db";
        private const int DatabaseVersion = 5; // Zwiększenie wersji bazy danych po dodaniu nowej tabeli

        public DatabaseHelper(Context context) : base(context, DatabaseName, null, DatabaseVersion)
        {
        }

        public override void OnCreate(SQLiteDatabase db)
        {
            // Tworzenie tabeli SensorData, jeśli nie istnieje
            db.ExecSQL("CREATE TABLE IF NOT EXISTS SensorData (Id INTEGER PRIMARY KEY, Data TEXT)");

            // Tworzenie tabeli BazaSnow, jeśli nie istnieje
            db.ExecSQL("CREATE TABLE IF NOT EXISTS BazaSnow (Id INTEGER PRIMARY KEY, Data TEXT, CzasPoczatku INTEGER, CzasZakonczenia INTEGER, CzasTrwania INTEGER, Ocena INTEGER)");

            // Tworzenie tabeli DaneSensorowe
            CreateDaneSensoroweTable(db);
        }

        private void CreateDaneSensoroweTable(SQLiteDatabase db)
        {
            // Tworzenie tabeli DaneSensorowe
            db.ExecSQL("CREATE TABLE IF NOT EXISTS DaneSensorowe (Id INTEGER PRIMARY KEY, date_time TEXT, acc_x REAL, acc_y REAL, acc_z REAL, gyr_x REAL, gyr_y REAL, gyr_z REAL, heart_rate REAL)");
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            // Obsługa aktualizacji bazy danych w przypadku zmiany schematu
            if (oldVersion < 3 && newVersion == 5)
            {
                // Dodaj nowe kolumny do tabeli BazaSnow, gdy wersja bazy danych zostanie zaktualizowana z wersji mniejszej niż 3 do 5
                db.ExecSQL("ALTER TABLE BazaSnow ADD COLUMN CzasPoczatku INTEGER");
                db.ExecSQL("ALTER TABLE BazaSnow ADD COLUMN CzasZakonczenia INTEGER");
            }

            if (oldVersion < 5 && newVersion == 5)
            {
                // Dodaj tabelę DaneSensorowe, gdy wersja bazy danych zostanie zaktualizowana z wersji mniejszej niż 5 do 5
                CreateDaneSensoroweTable(db);
            }

        }
    }
}