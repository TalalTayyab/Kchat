using kchat.db;
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
        private readonly ChatMessageRepository _chatMessageRepository;
        private readonly LokiService _lokiService;
        private bool _initialized;
        private CancellationTokenSource _ctSource;
        private ChatMessage _result;

        public event EventHandler<ChatMessage> OnNewMessage;

        public MessageService(
            KafkaProducer kafkaProducer, 
            KafkaConsumer kafkaConsumer, 
            IOptions<KafkaOptions> options, 
            LoggingService loggingService, 
            ChatMessageRepository chatMessageRepository, 
            LokiService lokiService)
        {
            _kafkaProducer = kafkaProducer;
            _kafkaConsumer = kafkaConsumer;
            _kafkaOptions = options.Value;
            _loggingService = loggingService;
            _chatMessageRepository = chatMessageRepository;
            _lokiService = lokiService;
            _initialized = false;
        }

        public void Dispose()
        {
            _ctSource.Cancel();
            _kafkaProducer.Close();
            _kafkaConsumer.Close();
        }

        public void Initialize()
        {
            try
            {
                if (_initialized)
                {
                    _loggingService.AddLog("Services were previously initialized.");
                    return;
                }

                _loggingService.AddLog("Initializing Kafka services.");

                _ctSource = new CancellationTokenSource();

                _kafkaProducer.Initialize(_kafkaOptions.BootstrapServers, _kafkaOptions.Username, _kafkaOptions.Password, _kafkaOptions.Topic);

                _loggingService.AddLog("Kafka producer initialized.");

                _initialized = true;

                InitConsumer();
            }

            catch (Exception exp)
            {
                _loggingService.AddLog($"Exception {exp.Message}");
                throw;
            }
        }

        private void InitConsumer()
        {
            Task.Run(async () =>
            {
                int retryCount=0;

                while (true)
                {
                    try
                    {
                        _kafkaConsumer.Initialize(_kafkaOptions.BootstrapServers, _kafkaOptions.Username, _kafkaOptions.Password, _kafkaOptions.Topic, _kafkaOptions.GroupId);
                        _loggingService.AddLog("Kafka consumer initialized.");

                        await Consume();
                    }
                    catch (Exception exp)
                    {
                        _loggingService.AddLog($"Exception {exp.Message}");

                        retryCount++; 

                        if (retryCount >= 2)
                        {
                            if (_result != null)
                            {
                                _kafkaConsumer.Commit(_result.Topic, _result.Partiton, _result.Offset);
                                _loggingService.AddLog($"Reached retry count {retryCount} committing message in Kafka.");
                                retryCount = 0;
                            }
                        }

                        _kafkaConsumer.Close();
                    }
                }
            });
        }

        private async Task Consume()
        {
            _result = _kafkaConsumer.Consume(_ctSource.Token);

            if (_result != null)
            {
                _loggingService.AddLog($"New Kafka Message '{_result.Text}' [{_result.Topic}]{{{_result.Partiton}:{_result.Offset}}}");

                _lokiService.GenerateMessageException(
                    _result.Text,
                    "exception 1",
                    "After message has been read but before it has been commited in kafka or database.");

                await _chatMessageRepository.Add(ToChatMessageEntity(_result));
                _loggingService.AddLog($"Message added in db");

                _lokiService.GenerateMessageException(
                    _result.Text, 
                    "exception 2", 
                    "After message has been added in db but before it has been commited in kafka.");

                _kafkaConsumer.Commit(_result.Topic, _result.Partiton, _result.Offset);
                _loggingService.AddLog($"Message commited in Kafka");

                _lokiService.GenerateMessageException(
                    _result.Text, 
                    "exception 3", 
                    "After message has been been commited in kafka and database but before displayed to user.");

                OnNewMessage?.Invoke(this, _result);
            }
        }

        private ChatMessageEntity ToChatMessageEntity(ChatMessage result)
        {
            return new ChatMessageEntity(result.Text, result.DateTime, result.UniqueMessageId, result.UserId, result.Topic, result.Partiton, result.Offset, _kafkaOptions.GroupId);
        }

        public async Task<bool> SendMessage(string userId, string text)
        {
            try
            {
                _loggingService.AddLog($"Sending message {userId}:{text}");

                var result = await _kafkaProducer.SendMessage(userId, text);

                _loggingService.AddLog($"Message sent {result}");

                return result;
            }

            catch (Exception exp)
            {
                _loggingService.AddLog($"Exception {exp.Message} - Stack {exp.StackTrace}");
                throw;
            }
        }

    }
}
