CREATE TABLE [dbo].[ChatMessage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Text] [varchar](max) NOT NULL,
	[DateTimeOffSet] [datetimeoffset](7) NOT NULL,
	[UserId] [varchar](200) NOT NULL,
	[UniqueMessageId] [uniqueidentifier] NOT NULL,
	[GroupId] [varchar](200) NOT NULL,
	[Topic] [varchar](200) NOT NULL,
	[TopicPartition] [int] NOT NULL,
	[TopicPartitionOffSet] int NOT NULL
 CONSTRAINT [PK_ChatMessage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


