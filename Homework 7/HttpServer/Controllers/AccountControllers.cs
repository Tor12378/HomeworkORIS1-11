using HttpServer.Attributes;
using HttpServer.Configuration;
using HttpServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Controllers
{
    [Controller("Account")]
    public class AccountControllers
    {
        [Post("SendToEmail")]
        public static void SendToEmail(string email, string password)
        {
            new EmailSenderService(AppSettingsLoader.Instance()?.Configuration).SendEmail(email, password);
            Console.WriteLine("Email was sent successfully!");
        }
    }
}
