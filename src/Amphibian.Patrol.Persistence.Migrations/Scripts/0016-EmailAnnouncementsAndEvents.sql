ALTER TABLE dbo.Announcements ADD
	Emailed bit not null default 0;
GO
ALTER TABLE dbo.Events ADD
	Emailed bit not null default 0;
GO
ALTER TABLE dbo.Users ADD
	AllowEmailNotifications bit not null default 1;
GO