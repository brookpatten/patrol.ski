CREATE TABLE dbo.Events
	(
	Id int NOT NULL IDENTITY (1, 1),
	PatrolId int not null,
	CreatedByUserId int not null,
	Name nvarchar(100) NULL,
	Location nvarchar(100) NULL,
	CreatedAt datetime not null,
	StartsAt datetime not null,
	EndsAt datetime not null
	
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Events ADD CONSTRAINT
	PK_Events PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Events ADD CONSTRAINT
	FK_Events_Patrols FOREIGN KEY
	(
	PatrolId
	) REFERENCES dbo.Patrols
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO
ALTER TABLE dbo.Events ADD CONSTRAINT
	FK_Events_Users FOREIGN KEY
	(
	CreatedByUserId
	) REFERENCES dbo.Users
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO



ALTER TABLE dbo.Events SET (LOCK_ESCALATION = TABLE)
GO
CREATE NONCLUSTERED INDEX [Events-PatrolId] ON [dbo].[Events]
(
	[PatrolId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.Patrols ADD
	EnableEvents bit not null default 1
GO