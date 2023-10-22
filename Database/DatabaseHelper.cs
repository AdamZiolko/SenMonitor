using Android.Content;
using Android.Database.Sqlite;
namespace SenMonitor
{
    public class DatabaseHelper : SQLiteOpenHelper
    {
        private new const string DatabaseName = "SenMonitor.db";
        private const int DatabaseVersion = 1;

        public DatabaseHelper(Context context) : base(context, DatabaseName, null, DatabaseVersion)
        {
        }

        public override void OnCreate(SQLiteDatabase db)
        {
            // Tworzenie tabeli, jeśli nie istnieje
            db.ExecSQL("CREATE TABLE IF NOT EXISTS SensorData (Id INTEGER PRIMARY KEY, Data TEXT)");
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            // Aktualizacja bazy danych w przypadku zmiany schematu
        }
    }
}