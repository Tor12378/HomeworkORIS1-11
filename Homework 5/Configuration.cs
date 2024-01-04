using Homework_5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Homework_5;

public static class Configuration
{
    public static async Task<AppSettingsConfig> GetConfigAsync(string fileName)
    {
        try
        {
            if (File.Exists(fileName))
            {
                using (var file = new FileStream(fileName, FileMode.Open))
                {
                    return await System.Text.Json.JsonSerializer.DeserializeAsync<AppSettingsConfig>(file);
                }
            }
            else
            {
                throw new FileNotFoundException(fileName);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при чтении конфигурации: {ex.Message}");
            throw;
        }
    }

    public static void CheckExistFolderSite(AppSettingsConfig config)
    {
        if (!Directory.Exists(config.StaticPathFiles))
        {
            try
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), config.StaticPathFiles));
                Console.WriteLine("Была создана папка static: " + config.StaticPathFiles);
            }
            catch (Exception)
            {
                Console.WriteLine("Невозможно создать папку");
            }
        }
    }
}