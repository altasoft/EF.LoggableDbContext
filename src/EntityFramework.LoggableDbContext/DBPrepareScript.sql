CREATE SCHEMA log
GO



CREATE TABLE log.DatabaseLog
(
	Id int IDENTITY(1,1) NOT NULL,
	Timestamp datetime NOT NULL,
	UserId int NOT NULL,
	CONSTRAINT [PK_log.DatabaseLog] PRIMARY KEY CLUSTERED (Id ASC)
)
GO



CREATE TABLE log.EntityLog
(
	Id int IDENTITY(1,1) NOT NULL,
	TypeName varchar(200) NOT NULL,
	TypeFullName varchar(500) NOT NULL,
	HasMultipleKey bit NOT NULL,
	EntityKey nvarchar(200) NOT NULL,
	EntityKeyValue nvarchar(200) NULL,
	DatabaseLogId int NOT NULL,
	CONSTRAINT [PK_log.EntityLog] PRIMARY KEY CLUSTERED (Id ASC)
)
GO

ALTER TABLE log.EntityLog  WITH CHECK ADD  CONSTRAINT [FK_log.EntityLog_log.DatabaseLog_DatabaseLogId] FOREIGN KEY(DatabaseLogId)
REFERENCES log.DatabaseLog (Id)
ON DELETE CASCADE
GO

ALTER TABLE log.EntityLog CHECK CONSTRAINT [FK_log.EntityLog_log.DatabaseLog_DatabaseLogId]
GO



CREATE TABLE log.EntityPropertiesLog
(
	Id int IDENTITY(1,1) NOT NULL,
	PropertyName varchar(200) NOT NULL,
	OldValue nvarchar(max) NULL,
	NewValue nvarchar(max) NULL,
	EntityLogId int NOT NULL,
	CONSTRAINT [PK_log.EntityPropertiesLog] PRIMARY KEY CLUSTERED (Id ASC)
)
GO

ALTER TABLE log.EntityPropertiesLog  WITH CHECK ADD  CONSTRAINT [FK_log.EntityPropertiesLog_log.EntityLog_EntityLogId] FOREIGN KEY(EntityLogId)
REFERENCES log.EntityLog (Id)
ON DELETE CASCADE
GO

ALTER TABLE log.EntityPropertiesLog CHECK CONSTRAINT [FK_log.EntityPropertiesLog_log.EntityLog_EntityLogId]
GO

