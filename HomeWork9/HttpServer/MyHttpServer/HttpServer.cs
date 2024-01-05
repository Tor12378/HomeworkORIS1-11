using System.Net;
using HttpServer.Configuration;
using HttpServer.Handlers;

namespace HttpServer;

public class HttpServer
{
    private HttpListener Listener { get; }

    private readonly string _prefix;
    
    private readonly StaticFilesHandler _staticFileHandler = new();
    
    private readonly ControllersHandler? _controllersHandler = new();
    
    private AppSettings Config { get; }

    public HttpServer(HttpListener listener)
    {
        Listener = listener;
        var appSettingLoader = new AppSettingsLoader();
        appSettingLoader.Init();
        Config = appSettingLoader.Configuration!;
        _prefix = Config.Address + ":" + Config.Port + "/";
    }

    public async Task Start()
    {
        try
        {
            
            Listener.Prefixes.Add(_prefix);
            Listener.Start();
            Console.WriteLine($"Server started.{Config.Address}:{Config.Port}");

            _ = Task.Run(async () =>
            {
                while (Listener.IsListening)
                {
                    var context = await Listener.GetContextAsync();
                    _staticFileHandler.Successor = _controllersHandler;
                    _staticFileHandler.HandleRequest(context);
                }
            });

            Console.WriteLine("Напишите stop, чтобы остановить сервер.");

            await Task.Run(() =>
            {
                while (true)
                    if (Console.ReadLine()!.Equals("stop"))
                        break;
            });
        
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
}