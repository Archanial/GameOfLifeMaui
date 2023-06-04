using GameOfLifeMaui.Entities;

namespace GameOfLifeMaui.Database;

public interface IDatabase
{
    public Task<List<SettingsEntity>> GetSettingsAsync();
    
    public Task SaveSettingAsync(SettingsEntity setting);

    public Task<List<ColorAgeEntity>> GetColorsAsync();

    public Task SaveColorAsync(ColorAgeEntity setting);
    
    public Task RemoveColorAsync(int age);
}