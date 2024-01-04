using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Homework_5
{
    internal class HttpServer
    {
        private HttpListener server;

        internal HttpListenerContext context;
        internal HttpListenerResponse response;
        internal HttpListenerRequest request;
        private AppSettingsConfig config;

        internal Task startServer;
        internal Task waitFinish;

        public HttpServer()
        {
            server = new HttpListener();
        }

        public void Start()
        {
            startServer = new Task(() => Run());
            waitFinish = new Task(() => Wait());
            startServer.Start();
            waitFinish.Start();

            Task.WaitAll(new Task[] { startServer, waitFinish });
        }

        private void Wait()
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (input == "stop")
                    break;
            }
        }

        private async void Run()
        {
            config = await Configuration.GetConfigAsync("appsettings.json");
            Configuration.CheckExistFolderSite(config);
            server.Prefixes.Add($"{config.Address}:{config.Port}/");
            Console.WriteLine($"Сервер запущен. Адрес: {config.Address}:{config.Port}");
            server.Start();

            while (true)
            {
                context = await server.GetContextAsync();
                request = context.Request;
                response = context.Response;

                string requestedPath = request.Url.LocalPath;
                Console.WriteLine(requestedPath);

                if (requestedPath.Contains("static"))
                    SendStaticFile(requestedPath);

                else if (requestedPath.EndsWith(".css"))
                    SendCSSFile("/styles"+requestedPath);

                else if (requestedPath.StartsWith("/images/"))
                    SendImageFile(requestedPath);

                else
                    SendHTMLFile();

                if (!(waitFinish.Status == TaskStatus.Running))
                    break;

                Console.WriteLine("Запрос обработан");
            }

            server.Close();
            ((IDisposable)server).Dispose();
            Console.WriteLine("Server has been stopped.");
        }

        private async void SendStaticFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                response.ContentType = "text/css";
                response.ContentLength64 = fileBytes.Length;
                using Stream outputStream = response.OutputStream;
                await outputStream.WriteAsync(fileBytes);
                await outputStream.FlushAsync();
            }
            else
            {
                response.StatusCode = 404;
                response.Close();
            }
        }

        private async void SendHTMLFile()
        {
            if (CheckExistFileHTML(config))
            {
                StreamReader site = new StreamReader($"{config.StaticPathFiles}/index.html");
                byte[] buffer = Encoding.UTF8.GetBytes(site.ReadToEnd());
                Console.WriteLine();
                response.ContentType = "text/html";
                response.ContentLength64 = buffer.Length;

                using Stream output = response.OutputStream;

                await output.WriteAsync(buffer);
                await output.FlushAsync();
            }
            else
            {
                StreamReader site = new StreamReader($"{config.StaticPathFiles}/404.html");
                byte[] buffer = Encoding.UTF8.GetBytes(site.ReadToEnd());
                Console.WriteLine();
                response.ContentType = "text/html";
                response.ContentLength64 = buffer.Length;

                using Stream output = response.OutputStream;

                await output.WriteAsync(buffer);
                await output.FlushAsync();
            }
        }


        private async void SendCSSFile(string filePath)
        {
            string fullPath = Path.Combine(Environment.CurrentDirectory, config.StaticPathFiles, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                byte[] fileBytes = File.ReadAllBytes(fullPath);
                response.ContentType = "text/css";
                response.ContentLength64 = fileBytes.Length;
                using Stream outputStream = response.OutputStream;
                await outputStream.WriteAsync(fileBytes);
                await outputStream.FlushAsync();
            }
            else
            {
                response.StatusCode = 404;
                response.Close();
            }
        }

        private string GetImageContentType(string imagePath)
        {
            string extension = Path.GetExtension(imagePath).ToLower();
            switch (extension)
            {
                case ".png":
                    return "image/png";
                default:
                    return "application/octet-stream";
            }
        }

        

        private async void SendImageFile(string imagePath)
        {
            string fullPath = Path.Combine(Environment.CurrentDirectory, "static", imagePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                byte[] imageBytes = File.ReadAllBytes(fullPath);
                string contentType = GetImageContentType(fullPath);
                response.ContentType = contentType;
                response.ContentLength64 = imageBytes.Length;
                using Stream outputStream = response.OutputStream;
                await outputStream.WriteAsync(imageBytes);
                await outputStream.FlushAsync();
            }
            else
            {
                response.StatusCode = 404;
                response.Close();
            }
        }

        internal bool CheckExistFileHTML(AppSettingsConfig config)
        {
            if (File.Exists($"{config.StaticPathFiles}/index.html"))
                return true;
            else
                return false;
        }
    }
}
