using System;
using System.Collections.Generic;
using System.Text;

namespace MqttApi.Config
{
    public interface IConfig
    {
        public int Port { get; }
        public string ConnectionString { get; }
        public string Username { get; }
        public string Password { get; }
        public string PublisherId { get; }
        public string SubscriberId { get; }
    }
}
