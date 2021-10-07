using System;
using System.Collections.Generic;
using System.Text;

namespace MqttApi.Config
{
    public class Config : IConfig
    {
        public Config()
        {
            Port = Int32.Parse(Environment.GetEnvironmentVariable("PORT"));
            ConnectionString = Environment.GetEnvironmentVariable("BROKER_CONNECTION");
            Username = Environment.GetEnvironmentVariable("BROKER_USERNAME");
            Password = Environment.GetEnvironmentVariable("BROKER_PASSWORD");
            PublisherId = Environment.GetEnvironmentVariable("PUBLISH_ID");
            SubscriberId = Environment.GetEnvironmentVariable("SUBSCRIBER_ID");
        }
        public int Port { get; }
        public string ConnectionString { get; }
        public string Username { get; }
        public string Password { get; }
        public string PublisherId { get; }
        public string SubscriberId { get; }
    }
}
