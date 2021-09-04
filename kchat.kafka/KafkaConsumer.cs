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
                SessionTimeoutMs = 10000, //keeping it small so its easier to debug
                HeartbeatIntervalMs = 1000, //keeping it small so its easier to debug
                MaxPollIntervalMs = 20000,//keeping it small so its easier to debug
                EnableAutoCommit = false, // dont auto commit
                EnableAutoOffsetStore = false, // dont store offset
                AutoOffsetReset = AutoOffsetReset.Earliest // re-read all the messages from the beginning for this consumer group
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
            var result = _consumer.Consume(ct);

            if (result == null || result.IsPartitionEOF)
            {
                return null;
            }

            var cm = JsonSerializer.Deserialize<ChatMessage>(result.Message.Value);

            cm.Topic = result.Topic;
            cm.Partiton = result.Partition.Value;
            cm.Offset = result.Offset.Value;

            return cm;
        }

        public void Commit(string topic, int partition, long offSet)
        {
            var tp = new TopicPartition(topic, partition);
            var os = new Offset(offSet + 1); // https://github.com/confluentinc/confluent-kafka-dotnet/issues/1300
            var tpo = new TopicPartitionOffset(tp, os);

            _consumer.Commit(new[] { tpo });
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
