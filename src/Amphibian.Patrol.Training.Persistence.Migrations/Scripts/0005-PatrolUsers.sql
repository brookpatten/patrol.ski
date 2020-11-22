CREATE TABLE dbo.PatrolUsers
	(
	Id int NOT NULL IDENTITY (1, 1),
	UserId int NOT NULL,
	PatrolId int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.PatrolUsers ADD CONSTRAINT
	FK_PatrolUsers_Patrols FOREIGN KEY
	(
	PatrolId
	) REFERENCES dbo.Patrols
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.PatrolUsers ADD CONSTRAINT
	FK_PatrolUsers_Users FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.Users
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	