using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MqttApi.Models
{
    public class UidMsg
    {
        [JsonProperty("x", Required = Required.Always)]
        public float X { get; set; }
        [JsonProperty("y", Required = Required.Always)]
        public float Y { get; set; }
    }
}
