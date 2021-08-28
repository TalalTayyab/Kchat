using kchat.kafka;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace kchat.UI.Framework
{
    public class MessageService : IDisposable
    {
        private readonly KafkaProducer _kafkaProducer;
        private readonly KafkaConsumer _kafkaConsumer;
        private readonly KafkaOptions _kafkaOptions;
        private readonly LoggingService _loggingService;
        private bool _initialized;
        private Task _consumeTask;
        private CancellationTokenSource _ctSource;

        public event EventHandler<ChatMessage> OnNewMessage;

        public MessageService(KafkaProducer kafkaProducer, KafkaConsumer kafkaConsumer, IOptions<KafkaOptions> options, LoggingService loggingService)
        {
            _kafkaProducer = kafkaProducer;
            _kafkaConsumer = kafkaConsumer;
            _kafkaOptions = options.Value;
            _loggingService = loggingService;
            _initialized = false;
        }

        public void Dispose()
        {
            _ctSource.Cancel();
            _kafkaProducer.Close();
            _kafkaConsumer.Close();
            _consumeTask.Dispose();
        }

        public void Initialize()
        {
            if (_initialized)
            {
                _loggingService.AddLog("Services were previously initialized.");
                return;
            }

            _loggingService.AddLog("Initializing Kafka services.");

            _ctSource = new CancellationTokenSource();
            
            _kafkaProducer.Initialize(_kafkaOptions.BootstrapServers, _kafkaOptions.Username, _kafkaOptions.Password, _kafkaOptions.Topic);
            _kafkaConsumer.Initialize(_kafkaOptions.BootstrapServers, _kafkaOptions.Username, _kafkaOptions.Password, _kafkaOptions.Topic, _kafkaOptions.GroupId);

            _loggingService.AddLog("Kafka services initialized.");

            _consumeTask = Task.Run(Consume);

            _initialized = true;
        }

        private void Consume()
        {
            _loggingService.AddLog("Running thread to consume Kafka messages.");

            try
            {
                while (true)
                {
                    var result = _kafkaConsumer.Consume(_ctSource.Token);

                    _loggingService.AddLog($"New Kafka Message {JsonSerializer.Serialize(result)}");

                    OnNewMessage?.Invoke(this, result);
                }
            }
            catch (OperationCanceledException)
            {
                _loggingService.AddLog("OperationCancelledException - closing Kafka consume thread");
                _kafkaConsumer.Close();
            }
        }

        public async Task<bool> SendMessage(string userId, string text)
        {
            _loggingService.AddLog($"Sending message {userId}:{text}");
            
            var result = await _kafkaProducer.SendMessage(userId, text);
            
            _loggingService.AddLog($"Message sent {result}");

            return result;
        }
    }
}
