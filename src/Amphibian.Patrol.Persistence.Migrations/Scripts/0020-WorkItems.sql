﻿ALTER TABLE dbo.Groups SET (LOCK_ESCALATION = TABLE)
GO
GO
ALTER TABLE dbo.Shifts SET (LOCK_ESCALATION = TABLE)
GO
GO
ALTER TABLE dbo.Users SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Patrols ADD
	EnableWorkItems bit NOT NULL CONSTRAINT DF_Patrols_EnableWorkItems DEFAULT 1
GO
ALTER TABLE dbo.Patrols SET (LOCK_ESCALATION = TABLE)
GO
CREATE TABLE dbo.RecurringWorkItems
	(
	Id int NOT NULL IDENTITY (1, 1),
	PatrolId int NOT NULL,
	Name varchar(255) NOT NULL,
	DescriptionMarkup varchar(MAX) NULL,
	Location varchar(255) NULL,
	CreatedAt datetime NOT NULL,
	CreatedByUserId int NOT NULL,
	CompletionMode tinyint NOT NULL,
	MaximumRandomCount tinyint NULL,
	AdminGroupId int NULL,
	RecurStart datetime NULL,
	RecurEnd datetime NULL,
	RecurIntervalSeconds int NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.RecurringWorkItems ADD CONSTRAINT
	PK_RecurringWorkItems PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_RecurringWorkItem_PatrolId ON dbo.RecurringWorkItems
	(
	PatrolId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.RecurringWorkItems ADD CONSTRAINT
	FK_RecurringWorkItems_Patrols FOREIGN KEY
	(
	PatrolId
	) REFERENCES dbo.Patrols
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.RecurringWorkItems WITH NOCHECK ADD CONSTRAINT
	FK_RecurringWorkItems_Groups FOREIGN KEY
	(
	AdminGroupId
	) REFERENCES dbo.Groups
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.RecurringWorkItems
	NOCHECK CONSTRAINT FK_RecurringWorkItems_Groups
GO
ALTER TABLE dbo.RecurringWorkItems SET (LOCK_ESCALATION = TABLE)
GO
CREATE TABLE dbo.ShiftRecurringWorkItems
	(
	Id int NOT NULL IDENTITY (1, 1),
	ShiftId int NOT NULL,
	RecurringWorkItemId int NOT NULL,
	ShiftAssignmentMode tinyint NULL,
	ScheduledAtHour tinyint NULL,
	ScheduledAtMinute tinyint NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.ShiftRecurringWorkItems ADD CONSTRAINT
	PK_ShiftRecurringWorkItems PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_ShiftRecurringWorkItems_ShiftId ON dbo.ShiftRecurringWorkItems
	(
	ShiftId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_ShiftRecurringWorkItems_RecurringWorkItemId ON dbo.ShiftRecurringWorkItems
	(
	RecurringWorkItemId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.ShiftRecurringWorkItems ADD CONSTRAINT
	FK_ShiftRecurringWorkItems_Shifts FOREIGN KEY
	(
	ShiftId
	) REFERENCES dbo.Shifts
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.ShiftRecurringWorkItems ADD CONSTRAINT
	FK_ShiftRecurringWorkItems_RecurringWorkItems FOREIGN KEY
	(
	RecurringWorkItemId
	) REFERENCES dbo.RecurringWorkItems
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.ShiftRecurringWorkItems SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.ScheduledShifts SET (LOCK_ESCALATION = TABLE)
GO
CREATE TABLE dbo.WorkItems
	(
	Id int NOT NULL IDENTITY (1, 1),
	PatrolId int NOT NULL,
	RecurringWorkItemId int NULL,
	ScheduledShiftId int NULL,
	Name varchar(255) NOT NULL,
	DescriptionMarkup varchar(MAX) NULL,
	Location varchar(255) NULL,
	ScheduledAt datetime NOT NULL,
	CreatedAt datetime NOT NULL,
	CreatedByUserId int NOT NULL,
	CanceledAt datetime NULL,
	CanceledByUserId int NULL,
	CompletedAt datetime NULL,
	CompletionMode tinyint NOT NULL,
	AdminGroupId int NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.WorkItems ADD CONSTRAINT
	PK_WorkItems PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_WorkItems_PatrolId ON dbo.WorkItems
	(
	PatrolId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_WorkItems_RecurringWorkItemId ON dbo.WorkItems
	(
	RecurringWorkItemId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_WorkItems_ScheduledShiftId ON dbo.WorkItems
	(
	ScheduledShiftId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_WorkItems_AdminGroupId ON dbo.WorkItems
	(
	AdminGroupId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_WorkItems_ScheduledAt ON dbo.WorkItems
	(
	ScheduledAt
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_WorkItems_CanceledAt ON dbo.WorkItems
	(
	CanceledAt
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_WorkItems_CompletedAt ON dbo.WorkItems
	(
	CompletedAt
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_WorkItems_CompletionMode ON dbo.WorkItems
	(
	CompletionMode
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.WorkItems ADD CONSTRAINT
	FK_WorkItems_Patrols FOREIGN KEY
	(
	PatrolId
	) REFERENCES dbo.Patrols
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.WorkItems WITH NOCHECK ADD CONSTRAINT
	FK_WorkItems_RecurringWorkItems FOREIGN KEY
	(
	RecurringWorkItemId
	) REFERENCES dbo.RecurringWorkItems
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.WorkItems
	NOCHECK CONSTRAINT FK_WorkItems_RecurringWorkItems
GO
ALTER TABLE dbo.WorkItems WITH NOCHECK ADD CONSTRAINT
	FK_WorkItems_ScheduledShifts FOREIGN KEY
	(
	ScheduledShiftId
	) REFERENCES dbo.ScheduledShifts
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.WorkItems
	NOCHECK CONSTRAINT FK_WorkItems_ScheduledShifts
GO
ALTER TABLE dbo.WorkItems ADD CONSTRAINT
	FK_WorkItems_Users FOREIGN KEY
	(
	CreatedByUserId
	) REFERENCES dbo.Users
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.WorkItems ADD CONSTRAINT
	FK_WorkItems_Users2 FOREIGN KEY
	(
	CanceledByUserId
	) REFERENCES dbo.Users
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.WorkItems WITH NOCHECK ADD CONSTRAINT
	FK_WorkItems_Groups FOREIGN KEY
	(
	AdminGroupId
	) REFERENCES dbo.Groups
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.WorkItems
	NOCHECK CONSTRAINT FK_WorkItems_Groups
GO
ALTER TABLE dbo.WorkItems SET (LOCK_ESCALATION = TABLE)
GO
CREATE TABLE dbo.WorkItemAssignments
	(
	Id int NOT NULL IDENTITY (1, 1),
	WorkItemId int NOT NULL,
	UserId int NOT NULL,
	CompletedAt datetime NULL,
	WorkNotes varchar(MAX) NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.WorkItemAssignments ADD CONSTRAINT
	PK_WorkItemAssignments PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_WorkItemAssignments_WorkItemId ON dbo.WorkItemAssignments
	(
	WorkItemId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_WorkItemAssignments_UserId ON dbo.WorkItemAssignments
	(
	UserId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_WorkItemAssignments_CompletedAt ON dbo.WorkItemAssignments
	(
	CompletedAt
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.WorkItemAssignments ADD CONSTRAINT
	FK_WorkItemAssignments_WorkItems FOREIGN KEY
	(
	WorkItemId
	) REFERENCES dbo.WorkItems
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.WorkItemAssignments ADD CONSTRAINT
	FK_WorkItemAssignments_Users FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.Users
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.WorkItemAssignments SET (LOCK_ESCALATION = TABLE)
GO