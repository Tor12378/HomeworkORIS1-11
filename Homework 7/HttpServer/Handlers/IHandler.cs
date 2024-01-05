using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Handlers
{
    public interface IHandler
    {
        void HandleRequest(HttpListenerContext context);
    }
}
