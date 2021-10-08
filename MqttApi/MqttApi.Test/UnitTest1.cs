using MqttApi.Models;
using Newtonsoft.Json;
using System;
using Xunit;

namespace MqttApi.Test
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("{'x': 1234.56}")]
        [InlineData("{'y': 1234.56}")]
        public void UidMsgShouldRaiseExceptionOnMissingField(string json)
        {
            Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<UidMsg>(json));
        }
        [Fact]
        public void UidMsgShouldRaiseExceptionOnWrongType()
        {
            string json = "{'x': 1234.56, 'y': 'bad form'}";
            Assert.Throws<JsonReaderException>(() => JsonConvert.DeserializeObject<UidMsg>(json));
        }

        [Fact]
        public void UidMsgShouldBeCreatedOnCorrectJson()
        {
            string json = "{'x': 34.56, 'y': 12.78}";
            UidMsg msg = JsonConvert.DeserializeObject<UidMsg>(json);
            Assert.InRange(34.56 - msg.X, -1e-4, 1e-4);
            Assert.InRange(12.78 - msg.Y, -1e-4, 1e-4);
        }
    }
}
