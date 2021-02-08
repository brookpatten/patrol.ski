CREATE TABLE dbo.ApiLogs
	(
	Id int NOT NULL IDENTITY (1, 1),
	UserId int NULL,
	Route varchar(255) NOT NULL,
	Verb varchar(10) NOT NULL,
	QueryString varchar(1024) not null,
	StartedAt datetime not null,
	DurationMs int null,
	ResponseCode int null,
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.ApiLogs ADD CONSTRAINT
	PK_ApiLogs PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO