using MqttApi.Models;
using MqttApi.Services;
using Newtonsoft.Json;
using System;

namespace MqttApi
{
    class Program
    {
        static void Main(string[] args)
        {
            Publisher.Publisher a = new Publisher.Publisher();
            while (true)
            {
                LoggerService.InitLogger();
                string msg = Console.ReadLine();
                UidMsg message = JsonConvert.DeserializeObject<UidMsg>(msg);
                a.PublishAsync("test", message).GetAwaiter().GetResult();
            }
        }
    }
}
