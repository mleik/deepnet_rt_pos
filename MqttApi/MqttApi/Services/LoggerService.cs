using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace MqttApi.Services
{
    public class LoggerService
    {
        public static void InitLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}
