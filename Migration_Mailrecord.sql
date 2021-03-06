/*
   miércoles, 12 de febrero de 202012:25:39 p. m.
   User: 
   Server: localhost\SQLEXPRESS
   Database: DB_104779_evaluador
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
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

CREATE TABLE dbo.MailRecords
	(
	Id int NOT NULL IDENTITY (1, 1),
	FromDate date NOT NULL,
	ToDate date NOT NULL,
	TimeStamp datetime NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.MailRecords ADD CONSTRAINT
	PK_Table_1 PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.MailRecords SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
