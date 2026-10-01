-- 0001_init.sql
-- Schemas and the schema version table. Idempotent: safe to run more than once.
-- Setup-Db.ps1 records this script in cfg.SchemaVersion after it succeeds.
-- cfg.SchemaVersion is a system table and is the one table without CompanyID or audit columns.
SET NOCOUNT ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'sec') EXEC (N'CREATE SCHEMA sec');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'mst') EXEC (N'CREATE SCHEMA mst');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'att') EXEC (N'CREATE SCHEMA att');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'pay') EXEC (N'CREATE SCHEMA pay');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'stat') EXEC (N'CREATE SCHEMA stat');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'car') EXEC (N'CREATE SCHEMA car');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'aud') EXEC (N'CREATE SCHEMA aud');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'cfg') EXEC (N'CREATE SCHEMA cfg');
GO

IF OBJECT_ID(N'cfg.SchemaVersion', N'U') IS NULL
BEGIN
    CREATE TABLE cfg.SchemaVersion
    (
        Version   NVARCHAR(100) NOT NULL CONSTRAINT PK_SchemaVersion PRIMARY KEY,
        AppliedOn DATETIME2(0)  NOT NULL CONSTRAINT DF_SchemaVersion_AppliedOn DEFAULT SYSUTCDATETIME()
    );
END
GO
