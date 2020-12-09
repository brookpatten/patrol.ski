CREATE TABLE dbo.TimeEntrys
	(
	Id int NOT NULL IDENTITY (1, 1),
	PatrolId int NOT NULL,
	UserId int NOT NULL,
	ClockIn datetime NOT NULL,
	ClockOut datetime NULL,
	DurationSeconds int NULL,
	MostRecentReminderSentAt datetime NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.TimeEntrys ADD CONSTRAINT
	PK_TimeEntrys PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.TimeEntrys ADD CONSTRAINT
	FK_TimeEntrys_Patrols FOREIGN KEY
	(
	PatrolId
	) REFERENCES dbo.Patrols
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.TimeEntrys ADD CONSTRAINT
	FK_TimeEntrys_Users FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.Users
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.TimeEntrys SET (LOCK_ESCALATION = TABLE)
GO
GO
ALTER TABLE dbo.ScheduledShiftAssignments SET (LOCK_ESCALATION = TABLE)
GO
CREATE TABLE dbo.TimeEntryScheduledShiftAssignments
	(
	Id int NOT NULL IDENTITY (1, 1),
	TimeEntryId int NOT NULL,
	ScheduledShiftAssignmentId int NOT NULL,
	DurationSeconds int NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.TimeEntryScheduledShiftAssignments ADD CONSTRAINT
	PK_TimeEntryScheduledShiftAssignments PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.TimeEntryScheduledShiftAssignments ADD CONSTRAINT
	FK_TimeEntryScheduledShiftAssignments_TimeEntrys FOREIGN KEY
	(
	TimeEntryId
	) REFERENCES dbo.TimeEntrys
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.TimeEntryScheduledShiftAssignments ADD CONSTRAINT
	FK_TimeEntryScheduledShiftAssignments_ScheduledShiftAssignments FOREIGN KEY
	(
	ScheduledShiftAssignmentId
	) REFERENCES dbo.ScheduledShiftAssignments
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.TimeEntryScheduledShiftAssignments SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE dbo.Patrols ADD
	EnableTimeClock bit not null default 1
GO
ALTER TABLE dbo.ScheduledShifts ADD
	DurationSeconds int null;
GO
update dbo.scheduledshifts set durationseconds=datediff(second,startsat,endsat);
GO
ALTER TABLE ScheduledShifts ALTER COLUMN DurationSeconds INT;