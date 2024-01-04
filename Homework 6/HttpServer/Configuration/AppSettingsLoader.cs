using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpServer.Configuration
{
    public class AppSettingsLoader
    {
        private string Path { get; set; }
        public AppSettings? Configuration { get; private set; }

        private static bool _isInitialized;

        private static AppSettingsLoader? _instance;

        public const string CurrentDirectory = "../../../";

        public AppSettingsLoader() => Path = $"{CurrentDirectory}appsettings.json";

        private AppSettingsLoader(string path, AppSettings config)
        {
            Path = path;
            Configuration = config;
        }
        public void Init()
        {
            try
            {
                if (!File.Exists(Path))
                    throw new FileNotFoundException("Файл appsettings.json не найден");

                var json = File.ReadAllText(Path);
                Configuration = JsonSerializer.Deserialize<AppSettings>(json);
                _isInitialized = true;
                _instance = new AppSettingsLoader(Path, Configuration!);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при загрузке настроек: {e.Message}");
                throw;
            }
        }

        public static AppSettingsLoader? Instance()
        {
            if (_isInitialized)
                return _instance;
            throw new InvalidOperationException("DataServer Singleton is not initialized");
        }
    }
}
