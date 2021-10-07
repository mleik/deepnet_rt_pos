using MQTTnet.Extensions.ManagedClient;
using System;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using Serilog;
using MqttApi.Config;
using MQTTnet.Client.Options;
using System.Threading.Tasks;
using MqttApi.Models;
using Newtonsoft.Json;

namespace MqttApi.Publisher
{
    public class Publisher
    {
        private IManagedMqttClient _client;
        private IConfig _config;

        public Publisher()
        {
            _client = GetAndStartManagedMqttClient();
            _config = new Config.Config();
        }

        public async Task PublishAsync(string topic, UidMsg message)
        {
            string msg = JsonConvert.SerializeObject(message);
            await _client.PublishAsync(topic, msg);
        }


        private IManagedMqttClient GetAndStartManagedMqttClient()
        {

            MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder()
                                        .WithClientId(_config.PublisherId)
                                        .WithTcpServer(_config.ConnectionString, _config.Port);

            ManagedMqttClientOptions options = new ManagedMqttClientOptionsBuilder()
                                    .WithAutoReconnectDelay(TimeSpan.FromSeconds(60))
                                    .WithClientOptions(builder.Build())
                                    .Build();

            IManagedMqttClient client = new MqttFactory().CreateManagedMqttClient();

            client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(a => { Log.Logger.Information("Successfully connected."); });
            client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(a => { Log.Logger.Warning("Couldn'connect to broker"); });
            client.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(a => { Log.Logger.Information("Successfully disconnected"); });

            client.StartAsync(options).GetAwaiter().GetResult();

            return client;
        }
    }
}
