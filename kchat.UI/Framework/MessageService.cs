using kchat.kafka;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace kchat.UI.Framework
{
    public class MessageService : IDisposable
    {
        private readonly KafkaProducer _kafkaProducer;
        private readonly KafkaConsumer _kafkaConsumer;
        private readonly KafkaOptions _kafkaOptions;
        private bool _initialized;
        private Task _consumeTask;
        private CancellationTokenSource _ctSource;

        public event EventHandler<ChatMessage> OnNewMessage;

        public MessageService(KafkaProducer kafkaProducer, KafkaConsumer kafkaConsumer, IOptions<KafkaOptions> options)
        {
            _kafkaProducer = kafkaProducer;
            _kafkaConsumer = kafkaConsumer;
            _kafkaOptions = options.Value;
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
            _ctSource = new CancellationTokenSource();
            
            if (_initialized)
            {
                return;
            }

            _kafkaProducer.Initialize(_kafkaOptions.BootstrapServers, _kafkaOptions.Username, _kafkaOptions.Password, _kafkaOptions.Topic);
            _kafkaConsumer.Initialize(_kafkaOptions.BootstrapServers, _kafkaOptions.Username, _kafkaOptions.Password, _kafkaOptions.Topic, _kafkaOptions.GroupId);

            _consumeTask = Task.Run(Consume);

            _initialized = true;
        }

        private void Consume()
        {
            try
            {
                while (true)
                {
                    var result = _kafkaConsumer.Consume(_ctSource.Token);
                    OnNewMessage?.Invoke(this, result);
                }
            }
            catch (OperationCanceledException)
            {
                _kafkaConsumer.Close();
            }
        }

        public async Task<bool> SendMessage(string userId, string text)
        {
            return await _kafkaProducer.SendMessage(userId, text);
        }
    }
}
