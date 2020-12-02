EXEC sp_rename 'TrainingShifts', 'ScheduledShifts';  
EXEC sp_rename 'ShiftTrainers', 'ScheduledShiftAssignments';  
EXEC sp_rename 'ScheduledShiftAssignments.TrainingShiftId', 'ScheduledShiftId', 'COLUMN';
EXEC sp_rename 'ScheduledShiftAssignments.TrainerUserId', 'AssignedUserId', 'COLUMN';
EXEC sp_rename 'Trainees.ShiftTrainerId', 'ScheduledShiftAssignmentId', 'COLUMN';
GO
ALTER TABLE dbo.ScheduledShifts ADD
	ShiftId int NULL,
	GroupId int NULL;
GO
ALTER TABLE dbo.ScheduledShiftAssignments ADD
	Status varchar(10) NULL,
	ClaimedByUserId int NULL,
	OriginalAssignedUserId int NULL;
GO
update ScheduledShiftAssignments set OriginalAssignedUserId=AssignedUserId, status='Assigned';
GO
ALTER TABLE ScheduledShiftAssignments ALTER COLUMN OriginalAssignedUserId int NOT NULL;
GO
ALTER TABLE ScheduledShiftAssignments ALTER COLUMN Status varchar(10) NOT NULL;
GO
ALTER TABLE dbo.ScheduledShiftAssignments ADD CONSTRAINT
	FK_ScheduledShiftAssignments_Users FOREIGN KEY
	(
	OriginalAssignedUserId
	) REFERENCES dbo.Users
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO
ALTER TABLE dbo.ScheduledShiftAssignments WITH NOCHECK ADD CONSTRAINT
	FK_ScheduledShiftAssignments_Users1 FOREIGN KEY
	(
	ClaimedByUserId
	) REFERENCES dbo.Users
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	 NOT FOR REPLICATION

GO
ALTER TABLE dbo.ScheduledShiftAssignments
	NOCHECK CONSTRAINT FK_ScheduledShiftAssignments_Users1
GO
CREATE TABLE dbo.Shifts
	(
	Id int NOT NULL IDENTITY (1, 1),
	PatrolId int NOT NULL,
	Name varchar(25) NOT NULL,
	StartHour int NULL,
	StartMinute int NULL,
	EndHour int NULL,
	EndMinute int NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Shifts ADD CONSTRAINT
	PK_Shifts PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
GO
CREATE NONCLUSTERED INDEX [Shifts-PatrolId] ON [dbo].[Shifts]
(
	[PatrolId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.Shifts ADD CONSTRAINT
	FK_Shifts_Patrols FOREIGN KEY
	(
	PatrolId
	) REFERENCES dbo.Patrols
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION;
GO
ALTER TABLE [dbo].[ScheduledShifts]  WITH NOCHECK ADD  CONSTRAINT [FK_ScheduledShifts_Groups] FOREIGN KEY([GroupId])
REFERENCES [dbo].[Groups] ([Id])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[ScheduledShifts] NOCHECK CONSTRAINT [FK_ScheduledShifts_Groups]
GO
ALTER TABLE [dbo].[ScheduledShifts]  WITH NOCHECK ADD  CONSTRAINT [FK_ScheduledShifts_Shifts] FOREIGN KEY([ShiftId])
REFERENCES [dbo].[Shifts] ([Id])
NOT FOR REPLICATION 
GO

ALTER TABLE [dbo].[ScheduledShifts] NOCHECK CONSTRAINT [FK_ScheduledShifts_Shifts]
GO
CREATE NONCLUSTERED INDEX [ScheduledShifts-PatrolId] ON [dbo].[ScheduledShifts]
(
	[PatrolId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE dbo.Patrols ADD
	EnableScheduling bit not null default 1,
	EnableShiftSwaps bit not null default 1,
	Timezone varchar(255) not null default 'Eastern Standard Time'
GO

CREATE NONCLUSTERED INDEX [ScheduledShiftAssignments-AssignedUserId] ON [dbo].ScheduledShiftAssignments
(
	AssignedUserId ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [ScheduledShiftAssignments-ClaimedByUserId] ON [dbo].ScheduledShiftAssignments
(
	ClaimedByUserId ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [ScheduledShiftAssignments-OriginalAssignedUserId] ON [dbo].ScheduledShiftAssignments
(
	OriginalAssignedUserId ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [ScheduledShiftAssignments-ScheduledShiftId] ON [dbo].ScheduledShiftAssignments
(
	ScheduledShiftId ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [ScheduledShiftAssignments-Status] ON [dbo].ScheduledShiftAssignments
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [ScheduledShifts-ShiftId] ON [dbo].ScheduledShifts
(
	ShiftId ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [ScheduledShifts-GroupId] ON [dbo].ScheduledShifts
(
	GroupId ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [Trainees-ScheduledShiftAssignmentId] ON [dbo].Trainees
(
	ScheduledShiftAssignmentId ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [Trainees-TraineeUserId] ON [dbo].Trainees
(
	TraineeUserId ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO