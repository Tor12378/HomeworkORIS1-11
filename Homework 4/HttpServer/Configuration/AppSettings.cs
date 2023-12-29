using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Configuration
{
    internal class AppSettings
    {
        public string? Address { get; set; }

        public int Port { get; set; }

        public string? StaticFilesPath { get; set; }

    }
}
