using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpServer.Configuration
{
    internal class AppSettingsSetup
    {
        public static AppSettings Init(string pathConfigFile)
        {
            if (!File.Exists(pathConfigFile))
            {
                Console.WriteLine("Файл appsettings.json не был найден");
                throw new Exception("Файл appsettings.json не был найден");
            }

            using var file = File.OpenRead(pathConfigFile);
            var config = JsonSerializer.Deserialize<AppSettings>(file);
            return config!;
        }
    }
}
