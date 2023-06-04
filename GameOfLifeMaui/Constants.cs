namespace GameOfLifeMaui;

public static class Constants
{
    public const string DatabaseFilename = "GoLSQLite.db3";

    public const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLite.SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLite.SQLiteOpenFlags.SharedCache;

    public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

    public const string FieldNameBArg = "BArg";
    
    public const string FieldNameSArg = "SArg";
    
    public const string FieldNameTappedAge = "TappedAge";
    
    public const string FieldNameCellSize = "CurrentCellSize";
    
    public const string FieldMiscSettings = "MiscSettings";
    
    public const string FieldNextButtonFrames = "NextButtonFrames";
}