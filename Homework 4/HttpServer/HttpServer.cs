using HttpServer.Configuration;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    internal class HttpServer
    {
        private HttpListener Listener { get; set; }
        private const string PathConfigFile = "appsettings.json";
        private AppSettings Config { get; set; }

        public HttpServer(HttpListener listener)
        {
            Listener = listener;
            Config = AppSettingsSetup.Init(PathConfigFile);
        }

        public async Task Start()
        {
            try
            {
                ConfigureListener();
                Console.WriteLine("Сервер запущен");

                _ = Task.Run(async () =>
                {
                    await HandleRequestsAsync();
                });

                Console.WriteLine("Напишите «stop», чтобы остановить сервер.");
                await WaitForStopCommand();

                Listener.Stop();
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

        private void ConfigureListener()
        {
            Listener.Prefixes.Add(Config.Address + ":" + Config.Port + "/");
            Listener.Start();
        }

        private async Task HandleRequestsAsync()
        {
            while (Listener.IsListening)
            {
                var context = await Listener.GetContextAsync();
                await ProcessRequestAsync(context);
            }
        }

        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            var response = context.Response;
            var localPath = context.Request.Url!.LocalPath;

            if (IsRootPath(localPath) || !localPath.EndsWith(".html"))
            {
                await ServeStaticContentAsync(response, localPath);
            }
            else
            {
                await ServeDynamicContentAsync(response, localPath);
            }
        }

        private bool IsRootPath(string localPath)
        {
            return string.IsNullOrEmpty(localPath) || localPath.Equals("/");
        }

        private async Task ServeStaticContentAsync(HttpListenerResponse response, string localPath)
        {
            const string fileName = "index.html";
            var filePath = Path.Combine(Config.StaticFilesPath!, fileName);

            if (File.Exists(filePath))
            {
                await SendFileAsync(response, filePath);
            }
            else
            {
                await SendNotFoundErrorPageAsync(response, $"Файл {fileName} не найден");
            }
        }

        private async Task ServeDynamicContentAsync(HttpListenerResponse response, string localPath)
        {
            var parts = localPath.Split("/");
            var directory = parts[1];
            var file = parts[2];
            var filePath = Path.Combine(directory, file);

            if (File.Exists(filePath))
            {
                await SendFileAsync(response, filePath);
            }
            else
            {
                await SendNotFoundErrorPageAsync(response, $"Файл {file} не найден");
            }
        }

        private async Task SendFileAsync(HttpListenerResponse response, string filePath)
        {
            var buffer = await File.ReadAllBytesAsync(filePath);
            await using var output = response.OutputStream;
            await output.WriteAsync(buffer);
            await output.FlushAsync();
        }

        private async Task SendNotFoundErrorPageAsync(HttpListenerResponse response, string errorMessage)
        {
            Console.WriteLine(errorMessage);
            const string error404 = "<h2>Ошибка 404</h2><h3>Файл не найден</h3>";
            response.ContentType = "text/html; charset=utf-8";
            var buffer = Encoding.UTF8.GetBytes(error404);
            await using var output = response.OutputStream;
            await output.WriteAsync(buffer);
            await output.FlushAsync();
        }

        private async Task WaitForStopCommand()
        {
            while (true)
            {
                var input = await Console.In.ReadLineAsync();
                if (input.Equals("stop", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }
        }
    }
}

