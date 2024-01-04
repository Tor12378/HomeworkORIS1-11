using System.Net;

namespace HttpServer
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            await new HttpServer(new HttpListener()).Start();
        }
    }
}