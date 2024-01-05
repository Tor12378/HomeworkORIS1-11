using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Services
{
    public interface IEmailSenderService
    {
        public void SendEmail(string login, string password);
    }
}
