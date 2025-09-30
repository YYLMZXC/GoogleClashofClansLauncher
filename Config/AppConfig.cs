using System.IO;
using System.Text.Json;

namespace GoogleClashofClansLauncher.Config;

public static class ConfigManager
{
    private const string FILE_NAME = "app_config.json";
    private static readonly string _path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "GoogleClashofClansLauncher",
        FILE_NAME);

    private static AppConfig? _cfg;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

    public static AppConfig GetConfig()
    {
        if (_cfg == null) Load();
        return _cfg ??= new AppConfig();
    }

    public static void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            File.WriteAllText(_path, JsonSerializer.Serialize(_cfg, _jsonSerializerOptions));
        }
        catch { /* silent */ }
    }

    private static void Load()
    {
        try
        {
            if (File.Exists(_path))
                _cfg = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(_path), _jsonSerializerOptions);
        }
        catch { /* silent */ }
    }
}

public class AppConfig
{
    public GeneralConfig General { get; set; } = new();
    public GameConfig Game { get; set; } = new();
    public APIConfig API { get; set; } = new();
}

public class GeneralConfig
{
    public bool AutoStart { get; set; }
    public bool CheckForUpdates { get; set; } = true;
    public string Language { get; set; } = "zh-CN";
    public string Theme { get; set; } = "Light";
}

public class GameConfig
{
    public string GameProcessName { get; set; } = "clashofclans";
    public string GameWindowTitle { get; set; } = "Clash of Clans";
    public string GameResolution { get; set; } = "1280x720";
    public int GameInteractionDelay { get; set; } = 1000;
}

public class APIConfig
{
    public string ApiKey { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = "https://api.example.com ";
    public int Timeout { get; set; } = 30000;
    public bool EnableLogging { get; set; }
}