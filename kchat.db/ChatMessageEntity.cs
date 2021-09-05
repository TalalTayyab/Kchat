using System;

namespace kchat.db
{
    public record ChatMessageEntity
    {
        public string Text { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public Guid UniqueMessageId { get; set; }
        public string UserId { get; set; }
        public string Topic { get; set; }
        public int TopicPartition { get; set; }
        public long TopicPartitionOffSet { get; set; }
        public string GroupId { get; set; }

        public ChatMessageEntity()
        {

        }

        public ChatMessageEntity(string text, DateTimeOffset dateTimeOffset,Guid uniqueMessageId , string userId, string topic, int partition, long offSet, string groupId)
        {
            Text = text;
            DateTime = dateTimeOffset;
            UniqueMessageId = uniqueMessageId;
            UserId = userId;
            Topic = topic;
            TopicPartition = partition;
            TopicPartitionOffSet = offSet;
            GroupId = groupId;
        }
    }
}
