using MqttApi.Models;
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
                string msg = Console.ReadLine();
                UidMsg message = JsonConvert.DeserializeObject<UidMsg>(msg);
                a.PublishAsync("test", message).GetAwaiter().GetResult();
            }
        }
    }
}
