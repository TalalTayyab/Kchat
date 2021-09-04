using System;

namespace kchat.kafka
{
    public record ChatMessage
    {
        public string Text { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public Guid UniqueMessageId { get; set; }
        public string UserId { get; set; }
        public string Topic { get; set; }
        public int Partiton { get; set; }
        public long Offset { get; set; }
    }
}
