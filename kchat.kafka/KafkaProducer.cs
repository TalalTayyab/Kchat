using Confluent.Kafka;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace kchat.kafka
{
    public class KafkaProducer : IDisposable
    {
        private IProducer<string, string> _producer;
        private string _topic;

        public void Initialize(string bootstrapServer, string username, string password, string topic)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServer,
            };

            if (!string.IsNullOrEmpty(username))
            {
                config.SecurityProtocol = SecurityProtocol.SaslSsl;
                config.SaslMechanism = SaslMechanism.ScramSha256;
                config.SaslUsername = username;
                config.SaslPassword = password;
            }

            _topic = topic;
            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public void Close()
        {
            if (_producer == null)
            {
                return;
            }

            _producer.Flush();
            _producer.Dispose();
            _producer = null;
        }

        public async Task<bool> SendMessage(string userId, string text)
        {
            var value = new ChatMessage()
            {
                UserId = userId,
                DateTime = DateTime.Now,
                Text = text,
                UniqueMessageId = Guid.NewGuid()
            };

            var message = new Message<string, string>()
            {
                Key = userId,
                Value = JsonSerializer.Serialize<ChatMessage>(value)
            };

            var dr = await _producer.ProduceAsync(_topic, message);

            return dr.Status == PersistenceStatus.Persisted;
        }

        public void Dispose()
        {
            Close();
        }

    }
}
