using System;

namespace kchat.kafka
{
    public class ChatMessage
    {
        public string Text { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public Guid UniqueMessageId { get; set; }
        public string UserId { get; set; }
    }
}
