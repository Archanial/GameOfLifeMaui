using GameOfLifeMaui.Entities;
using SQLite;

namespace GameOfLifeMaui;

public sealed class GoLDatabase : IDatabase
{
    private SQLiteAsyncConnection _database;

    private async Task Init()
    {
        if (_database is not null)
            return;

        _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        await _database.CreateTableAsync<SettingsEntity>();
        await _database.CreateTableAsync<ColorAgeEntity>();
    }

    public async Task<List<SettingsEntity>> GetSettingsAsync()
    {
        await Init();
        return await _database.Table<SettingsEntity>().ToListAsync();
    }
    
    public async Task SaveSettingAsync(SettingsEntity setting)
    {
        await Init();
        await _database.Table<SettingsEntity>().DeleteAsync(x => x.FieldName == setting.FieldName);
        await _database.InsertAsync(setting);
    }
    
    public async Task<List<ColorAgeEntity>> GetColorsAsync()
    {
        await Init();
        return await _database.Table<ColorAgeEntity>().OrderBy(x => x.Age).ToListAsync();
    }
    
    public async Task SaveColorAsync(ColorAgeEntity setting)
    {
        await Init();
        await _database.Table<ColorAgeEntity>().DeleteAsync(x => x.Age == setting.Age);
        await _database.InsertAsync(setting);
    }

    public async Task RemoveColorAsync(int age)
    {
        await Init();
        await _database.Table<ColorAgeEntity>().DeleteAsync(x => x.Age == age);
    }
}