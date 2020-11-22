﻿/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Patrols
	(
	Id int NOT NULL IDENTITY (1, 1),
	Name varchar(256) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Patrols ADD CONSTRAINT
	PK_Patrols PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Patrols SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Sections
	(
	Id int NOT NULL IDENTITY (1, 1),
	Name varchar(256) NOT NULL,
	PatrolId int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Sections ADD CONSTRAINT
	PK_Sections PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Sections ADD CONSTRAINT
	FK_Sections_Patrols FOREIGN KEY
	(
	PatrolId
	) REFERENCES dbo.Patrols
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Sections SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Plans
	(
	Id int NOT NULL IDENTITY (1, 1),
	Name varchar(256) NOT NULL,
	PatrolId int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Plans ADD CONSTRAINT
	PK_Plans PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Plans ADD CONSTRAINT
	FK_Plans_Patrols FOREIGN KEY
	(
	PatrolId
	) REFERENCES dbo.Patrols
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Plans SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.PlanSections
	(
	Id int NOT NULL IDENTITY (1, 1),
	PlanId int NOT NULL,
	SectionId int NOT NULL,
	PatrolId int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.PlanSections ADD CONSTRAINT
	PK_PlanSections PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.PlanSections ADD CONSTRAINT
	FK_PlanSections_Plans FOREIGN KEY
	(
	PlanId
	) REFERENCES dbo.Plans
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.PlanSections ADD CONSTRAINT
	FK_PlanSections_Sections FOREIGN KEY
	(
	SectionId
	) REFERENCES dbo.Sections
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.PlanSections ADD CONSTRAINT
	FK_PlanSections_Patrols FOREIGN KEY
	(
	PatrolId
	) REFERENCES dbo.Patrols
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.PlanSections SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Levels
	(
	Id int NOT NULL IDENTITY (1, 1),
	Name varchar(256) NOT NULL,
	Description varchar(256) NULL,
	PatrolId int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Levels ADD CONSTRAINT
	PK_Levels PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Levels ADD CONSTRAINT
	FK_Levels_Patrols FOREIGN KEY
	(
	PatrolId
	) REFERENCES dbo.Patrols
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Levels SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.SectionLevel
	(
	Id int NOT NULL IDENTITY (1, 1),
	SectionId int NOT NULL,
	LevelId int NOT NULL,
	[Order] int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.SectionLevel ADD CONSTRAINT
	PK_SectionLevel PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.SectionLevel ADD CONSTRAINT
	FK_SectionLevel_Sections FOREIGN KEY
	(
	SectionId
	) REFERENCES dbo.Sections
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.SectionLevel ADD CONSTRAINT
	FK_SectionLevel_Levels FOREIGN KEY
	(
	LevelId
	) REFERENCES dbo.Levels
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.SectionLevel SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Skills SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.SectionSkill
	(
	Id int NOT NULL IDENTITY (1, 1),
	SectionId int NOT NULL,
	SkillId int NOT NULL,
	[Order] int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.SectionSkill ADD CONSTRAINT
	FK_SectionSkill_Skills FOREIGN KEY
	(
	SkillId
	) REFERENCES dbo.Skills
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.SectionSkill ADD CONSTRAINT
	FK_SectionSkill_Sections FOREIGN KEY
	(
	SectionId
	) REFERENCES dbo.Sections
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.SectionSkill SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE Skills
ADD PatrolId int not null;
GO
ALTER TABLE dbo.Skills ADD CONSTRAINT
	FK_Skills_Patrols FOREIGN KEY
	(
	PatrolId
	) REFERENCES dbo.Patrols
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO
COMMIT