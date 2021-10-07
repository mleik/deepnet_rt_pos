using MqttApi.Models;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MqttApi.Subscriber
{
    public class Subscriber
    {
        private IManagedMqttClient _client;
        public Subscriber()
        {
            _client = GetSubscriber();
        }

        private IManagedMqttClient GetSubscriber()
        {
            IManagedMqttClient client = new MqttFactory().CreateManagedMqttClient();

            client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(a => { Log.Logger.Information("Successfully connected."); });
            client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(a => { Log.Logger.Warning("Couldn't connect to broker."); });
            client.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(a => { Log.Logger.Information("Successfully disconnected."); });

            client.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(a => {
                JsonConvert.DeserializeObject<UidMsg>(Encoding.UTF8.GetString(a.ApplicationMessage.Payload));
                Log.Logger.Information("Message recieved: {payload}", Encoding.UTF8.GetString(a.ApplicationMessage.Payload));
            });

            
            return client;
        }

        public async Task StartClient()
        {
            MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder()
                                        .WithClientId("sub")
                                        .WithTcpServer("localhost", 707);

            ManagedMqttClientOptions options = new ManagedMqttClientOptionsBuilder()
                                    .WithAutoReconnectDelay(TimeSpan.FromSeconds(60))
                                    .WithClientOptions(builder.Build())
                                    .Build();

            await _client.StartAsync(options);
        }

        public async Task<bool> Subscribe(string topic)
        {
            if (_client.IsStarted)
            {
                return false;
            }
            await _client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic)
                .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)1)
                .Build());
            return true;
        }
    }
}
