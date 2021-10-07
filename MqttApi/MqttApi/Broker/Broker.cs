using MQTTnet;
using MQTTnet.Server;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MqttApi.Broker
{
    public class Broker
    {
        private static int MessageCounter = 0;
        private IMqttServer _server;
        public Broker()
        {
            _server = GetAndStartBroker();
        }

        private IMqttServer GetAndStartBroker()
        {
            MqttServerOptionsBuilder options = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(707)
                .WithConnectionValidator(OnNewConnection)
                .WithApplicationMessageInterceptor(OnNewMessage);

            IMqttServer server = new MqttFactory().CreateMqttServer();
            server.StartAsync(options.Build()).GetAwaiter().GetResult();
            return server;
        }
        private static void OnNewConnection(MqttConnectionValidatorContext context)
        {
            Log.Logger.Information(
                "New Connection: ClientId = {clientId}, Endpoint = {endpoint}, CleanSession = {cleanSession}",
                context.ClientId, context.Endpoint, context.CleanSession);
        }

        private static void OnNewMessage(MqttApplicationMessageInterceptorContext context)
        {
            var payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);
            MessageCounter++;
            Log.Logger.Information(
                "MessageId: {MessageCounter} - TimeStamp: {TimeStamp} -- Message: ClientId = {clientId}, Topic = {topic}, Payload = {payload}, QoS = {qos}, Retain-Flag = {retainFlag}",
                MessageCounter,
                DateTime.Now,
                context.ClientId,
                context.ApplicationMessage?.Topic,
                payload,
                context.ApplicationMessage?.QualityOfServiceLevel,
                context.ApplicationMessage?.Retain);
        }
    }
}
