--at some point dommel improved it's pluralizing and broke the world

EXEC sp_rename 'TimeEntrys', 'TimeEntries';
EXEC sp_rename 'PK_TimeEntrys', 'PK_TimeEntries';
EXEC sp_rename 'FK_TimeEntrys_Patrols', 'FK_TimeEntries_Patrols';
EXEC sp_rename 'FK_TimeEntrys_Users', 'FK_TimeEntries_Users';
