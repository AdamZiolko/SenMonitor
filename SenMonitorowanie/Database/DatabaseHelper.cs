using Android.Content;
using Android.Database.Sqlite;

namespace SenMonitorowanie
{
    public class DatabaseHelper : SQLiteOpenHelper
    {
        private const string DatabaseName = "SenMonitor.db";
        private const int DatabaseVersion = 3; // Zwiększenie wersji bazy danych po dodaniu nowej tabeli

        public DatabaseHelper(Context context) : base(context, DatabaseName, null, DatabaseVersion)
        {
        }

        public override void OnCreate(SQLiteDatabase db)
        {
            // Tworzenie tabeli SensorData, jeśli nie istnieje
            db.ExecSQL("CREATE TABLE IF NOT EXISTS SensorData (Id INTEGER PRIMARY KEY, Data TEXT)");

            // Tworzenie tabeli BazaSnow, jeśli nie istnieje
            db.ExecSQL("CREATE TABLE IF NOT EXISTS BazaSnow (Id INTEGER PRIMARY KEY, Data TEXT, CzasPoczatku INTEGER, CzasZakonczenia INTEGER, CzasTrwania INTEGER, Ocena INTEGER)");
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            // Obsługa aktualizacji bazy danych w przypadku zmiany schematu
            if (oldVersion < 3 && newVersion == 3)
            {
                // Dodaj nowe kolumny, gdy wersja bazy danych zostanie zaktualizowana z wersji mniejszej niż 3 do 3
                db.ExecSQL("ALTER TABLE BazaSnow ADD COLUMN CzasPoczatku INTEGER");
                db.ExecSQL("ALTER TABLE BazaSnow ADD COLUMN CzasZakonczenia INTEGER");
            }
        }
    }
}
