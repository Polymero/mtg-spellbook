using SQLite;

namespace MyApplication.Client
{
    public static class DatabaseConstants
    {
        public const string DatabaseFilename = "TrialSQLite.db3";
        public const SQLiteOpenFlags Flags = 
            SQLiteOpenFlags.ReadWrite | 
            SQLiteOpenFlags.Create | 
            SQLiteOpenFlags.SharedCache;
        public static string DatabaseFilepath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), DatabaseFilename);
    }
}

