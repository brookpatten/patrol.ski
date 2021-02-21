ALTER TABLE dbo.Patrols ADD
	EnablePublicSite bit NOT NULL CONSTRAINT DF_Patrols_EnablePublicSite DEFAULT 0,
	Subdomain varchar(255) null,
	LogoImageUrl varchar(1024) null,
	BackgroundImageUrl varchar(1024) null,
	Phone varchar(128) null,
	Email varchar(255) null
GO
ALTER TABLE dbo.Users ADD
	ProfileImageUrl varchar(1024) null
GO
ALTER TABLE dbo.Announcements ADD
	IsPublic bit null,
	IsInternal bit null
GO
ALTER TABLE dbo.Events ADD
	IsPublic bit null,
	IsInternal bit null,
	EventMarkdown nvarchar(max) null,
	EventHtml nvarchar(max) null,
	SignupMode tinyint null,
	MaxSignups int null
GO
update announcements set ispublic=0,isinternal=1;
update events set ispublic=0,isinternal=1,SignupMode=0;
GO
ALTER TABLE Announcements ALTER COLUMN IsPublic bit NOT NULL;
ALTER TABLE Announcements ALTER COLUMN IsInternal bit NOT NULL;
ALTER TABLE Events ALTER COLUMN IsPublic bit NOT NULL;
ALTER TABLE Events ALTER COLUMN IsInternal bit NOT NULL;
ALTER TABLE Events ALTER COLUMN SignupMode int NOT NULL;
GO
CREATE TABLE dbo.EventSignups
	(
	Id int NOT NULL IDENTITY (1, 1),
	EventId int NOT NULL,
	UserId int NOT NULL,
	SignedUpAt datetime not null
	)  ON [PRIMARY]
GO
CREATE TABLE dbo.FileUploads
	(
	Id int NOT NULL IDENTITY (1, 1),
	PatrolId int NULL,
	UserId int NULL,
	Name varchar(1023),
	FileSize int null,
	)  ON [PRIMARY]
GO