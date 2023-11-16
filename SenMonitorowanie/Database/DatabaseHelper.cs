using Android.Content;
using Android.Database.Sqlite;

namespace SenMonitorowanie
{
    public class DatabaseHelper : SQLiteOpenHelper
    {
        private const string DatabaseName = "SenMonitor.db";
        private const int DatabaseVersion = 2; // Zwiększenie wersji bazy danych po dodaniu nowej tabeli

        public DatabaseHelper(Context context) : base(context, DatabaseName, null, DatabaseVersion)
        {
        }

        public override void OnCreate(SQLiteDatabase db)
        {
            // Tworzenie tabeli, jeśli nie istnieje
            db.ExecSQL("CREATE TABLE IF NOT EXISTS SensorData (Id INTEGER PRIMARY KEY, Data TEXT)");

            // Dodanie nowej tabeli BazaSnow
            db.ExecSQL("CREATE TABLE IF NOT EXISTS BazaSnow (Id INTEGER PRIMARY KEY, Data TEXT, CzasTrwania INTEGER, Ocena INTEGER)");
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            // Obsługa aktualizacji bazy danych w przypadku zmiany schematu
            if (oldVersion == 1 && newVersion == 2)
            {
                // Dodaj nową tabelę, gdy wersja bazy danych zostanie zaktualizowana z 1 do 2
                db.ExecSQL("CREATE TABLE IF NOT EXISTS BazaSnow (Id INTEGER PRIMARY KEY, Data TEXT, CzasTrwania INTEGER, Ocena INTEGER)");
            }
        }
    }
}
