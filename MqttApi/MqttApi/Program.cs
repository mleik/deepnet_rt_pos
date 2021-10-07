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
            Broker.Broker broker = new Broker.Broker();
            Publisher.Publisher pub = new Publisher.Publisher();
            Subscriber.Subscriber sub = new Subscriber.Subscriber();
            pub.StartClient().GetAwaiter().GetResult();
            sub.Subscribe("test").GetAwaiter().GetResult();
            sub.StartClient().GetAwaiter().GetResult();
            while (true)
            {
                LoggerService.InitLogger();
                string msg = Console.ReadLine();
                UidMsg message = JsonConvert.DeserializeObject<UidMsg>(msg);
                pub.Publish("test", message).GetAwaiter().GetResult();
            }
        }
    }
}
