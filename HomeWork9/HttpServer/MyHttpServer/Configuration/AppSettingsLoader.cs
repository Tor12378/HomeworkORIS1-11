﻿using System.Text.Json;

namespace HttpServer.Configuration;

public class AppSettingsLoader
{
    private string Path { get; set; }
    public AppSettings? Configuration { get; private set; }

    private static bool _isInitialized;
    
    private static AppSettingsLoader? _instance;


    public AppSettingsLoader() => Path = "appsettings.json";

    private AppSettingsLoader(string path, AppSettings config)
    {
        Path = path;
        Configuration = config;
    }
    
    public void Init()
    {
        try
        {
            var json = File.ReadAllText(Path);
            Configuration = JsonSerializer.Deserialize<AppSettings>(json);
            _isInitialized = true;
            _instance = new AppSettingsLoader(Path, Configuration!);
        }
        
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        if (!File.Exists(Path))
            throw new ArgumentException("appsettings.json не найден");
    }
    
    public static AppSettingsLoader? Instance()
    {
        if (_isInitialized)
            return _instance;
        throw new InvalidOperationException("DataServer Singleton is not initialized");
    }
}