using System.Net;
using HttpServer;
namespace HttpServer;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await new HttpServer(new HttpListener()).Start();
    }
}