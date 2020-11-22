﻿CREATE TABLE dbo.TrainingShifts
	(
	Id int NOT NULL IDENTITY (1, 1),
	PatrolId int NOT NULL,
	StartsAt datetime NOT NULL,
	EndsAt datetime NOT NULL
	)  ON [PRIMARY];
GO
ALTER TABLE dbo.TrainingShifts ADD CONSTRAINT
	PK_TrainingShifts PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
GO;
ALTER TABLE dbo.TrainingShifts ADD CONSTRAINT
	FK_TrainingShifts_Patrols FOREIGN KEY
	(
	PatrolId
	) REFERENCES dbo.Patrols
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

CREATE TABLE dbo.Trainers
	(
	Id int NOT NULL IDENTITY (1, 1),
	TrainingShiftId int NOT NULL,
	TrainerUserId int NOT NULL
	)  ON [PRIMARY];
GO
ALTER TABLE dbo.Trainers ADD CONSTRAINT
	PK_Trainers PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
GO;
ALTER TABLE dbo.Trainers ADD CONSTRAINT
	FK_Trainers_TrainingShifts FOREIGN KEY
	(
	TrainingShiftId
	) REFERENCES dbo.TrainingShifts
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Trainers ADD CONSTRAINT
	FK_Trainers_Users FOREIGN KEY
	(
	TrainerUserId
	) REFERENCES dbo.Users
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
CREATE TABLE dbo.Trainees
	(
	Id int NOT NULL IDENTITY (1, 1),
	TrainerId int NOT NULL,
	TraineeUserId int NOT NULL
	)  ON [PRIMARY];
GO
ALTER TABLE dbo.Trainees ADD CONSTRAINT
	PK_Trainees PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
GO;
ALTER TABLE dbo.Trainees ADD CONSTRAINT
	FK_Trainees_Trainers FOREIGN KEY
	(
	TrainerId
	) REFERENCES dbo.Trainers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Trainees ADD CONSTRAINT
	FK_Trainees_Users FOREIGN KEY
	(
	TraineeUserId
	) REFERENCES dbo.Users
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO