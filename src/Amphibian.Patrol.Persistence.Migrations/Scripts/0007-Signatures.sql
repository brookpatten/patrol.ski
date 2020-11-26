CREATE TABLE dbo.Signatures
	(
	Id int NOT NULL IDENTITY (1, 1),
	AssignmentId int NOT NULL,
	SectionSkillId int NOT NULL,
	SectionLevelId int NOT NULL,
	SignedByUserId int NOT NULL,
	SignedAt datetime NOT NULL
	)  ON [PRIMARY];
GO
ALTER TABLE dbo.Signatures ADD CONSTRAINT
	PK_Signatures PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
GO;
ALTER TABLE dbo.Signatures ADD CONSTRAINT
	FK_Signatures_SectionSkills FOREIGN KEY
	(
	SectionSkillId
	) REFERENCES dbo.SectionSkills
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION;
GO;
ALTER TABLE dbo.Signatures ADD CONSTRAINT
	FK_Signatures_SectionLevels FOREIGN KEY
	(
	SectionLevelId
	) REFERENCES dbo.SectionLevels
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION;
GO;
ALTER TABLE dbo.Signatures ADD CONSTRAINT
	FK_Signatures_Users FOREIGN KEY
	(
	SignedByUserId
	) REFERENCES dbo.Users
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION;
GO;
ALTER TABLE dbo.Signatures ADD CONSTRAINT
	FK_Signatures_Assignments FOREIGN KEY
	(
	AssignmentId
	) REFERENCES dbo.Assignments
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION;
GO;