CREATE TABLE dbo.Announcements
	(
	Id int NOT NULL IDENTITY (1, 1),
	PatrolId int not null,
	CreatedByUserId int not null,
	Subject nvarchar(100) NULL,
	AnnouncementMarkdown nvarchar(MAX) NULL,
	AnnouncementHtml nvarchar(MAX) NULL,
	CreatedAt datetime not null,
	PostAt datetime not null,
	ExpireAt datetime null
	
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Announcements ADD CONSTRAINT
	PK_Announcements PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Announcements ADD CONSTRAINT
	FK_Announcements_Patrols FOREIGN KEY
	(
	PatrolId
	) REFERENCES dbo.Patrols
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO
ALTER TABLE dbo.Announcements ADD CONSTRAINT
	FK_Announcements_Users FOREIGN KEY
	(
	CreatedByUserId
	) REFERENCES dbo.Users
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO



ALTER TABLE dbo.Announcements SET (LOCK_ESCALATION = TABLE)
GO
CREATE NONCLUSTERED INDEX [Announcements-PatrolId] ON [dbo].[Announcements]
(
	[PatrolId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.Patrols ADD
	EnableTraining bit not null default 1,
	EnableAnnouncements bit not null default 1
GO