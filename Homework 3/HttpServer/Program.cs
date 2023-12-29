using System.Net;
using System.Text.Json;

namespace HttpServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const string pathConfigFile = "appsettings.json";
            HttpListener listener = new();

            try
            {
                if (!File.Exists(pathConfigFile))
                {
                    Console.WriteLine("Файл appsettings.json не был найден");
                    throw new Exception("Файл appsettings.json не был найден");
                }

                AppSettings? config;

                using (var file = File.OpenRead(pathConfigFile))
                    config = await JsonSerializer.DeserializeAsync<AppSettings>(file);

                listener.Prefixes.Add(config!.Address + ":" + config.Port + "/");
                listener.Start();
                Console.WriteLine("Сервер запущен");

                var cancellationTokenSource = new CancellationTokenSource();

                _ = Task.Run(async () =>
                {
                    while (listener.IsListening)
                    {
                        var context = await listener.GetContextAsync();
                        var response = context.Response;
                        const string filePath = "index.html";

                        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                        await fileStream.CopyToAsync(response.OutputStream);
                        response.Close();
                    }
                }, cancellationTokenSource.Token);

                Console.WriteLine("Напишите «stop», чтобы остановить сервер.");

                while (true)
                {
                    if (Console.ReadLine()?.Equals("stop", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        cancellationTokenSource.Cancel();
                        break;
                    }
                }

                listener.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.WriteLine("Работа сервера завершена");
            }
        }
    }
}