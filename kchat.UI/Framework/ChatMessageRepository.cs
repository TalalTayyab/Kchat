using System;
using System.Threading.Tasks;

namespace kchat.db
{
    public class ChatMessageRepository
    {
        private readonly DatabaseService databaseService;

        public ChatMessageRepository(DatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }
        public async Task<int> Add(ChatMessageEntity chatMessageEntity)
        {
            var sql = @"INSERT INTO [ChatMessage] ([Text],[DateTimeOffset],[UserId],[UniqueMessageId],[Topic],[TopicPartition],[TopicPartitionOffSet],[GroupId]) " +
                "values (@Text,@DateTime,@UserId,@UniqueMessageId,@Topic,@TopicPartition,@TopicPartitionOffSet,@GroupId)";

            return await databaseService.ExecuteSqlAsync(sql, chatMessageEntity);
        }

        public async Task<ChatMessageEntity> Get(string groupId, Guid uniqueMessageId)
        {
            var sql = "SELECT * from [ChatMessage] where [GroupId] = @groupId and UniqueMessageId = @uniqueMessageId";
            return await databaseService.GetTAsync<ChatMessageEntity>(sql, new { groupId = groupId, uniqueMessageId = uniqueMessageId });
        }
    }
}
