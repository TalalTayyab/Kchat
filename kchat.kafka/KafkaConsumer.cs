using Confluent.Kafka;
using System;
using System.Text.Json;
using System.Threading;

namespace kchat.kafka
{
    public class KafkaConsumer : IDisposable
    {
        private IConsumer<string, string> _consumer;

        public void Initialize(string bootstrapServer, string username, string password, string topic, string groupId)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServer,
                GroupId = groupId,
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetReset.Latest
            };

            if (!string.IsNullOrEmpty(username))
            {
                config.SecurityProtocol = SecurityProtocol.SaslSsl;
                config.SaslMechanism = SaslMechanism.ScramSha256;
                config.SaslUsername = username;
                config.SaslPassword = password;
            }


            _consumer = new ConsumerBuilder<string, string>(config).Build();

            _consumer.Subscribe(topic);
        }

        public ChatMessage Consume(CancellationToken ct)
        {
            var cr =  _consumer.Consume(ct);
            var cm = JsonSerializer.Deserialize<ChatMessage>(cr.Message.Value);
            cm.UserId = cm.UserId ?? cr.Message.Key;
            return cm;
        }

        public void Close()
        {
            if (_consumer == null)
            {
                return;
            }

            _consumer.Close();
            _consumer.Dispose();
            _consumer = null;
        }


        public void Dispose()
        {
            Close();
        }
    }
}
