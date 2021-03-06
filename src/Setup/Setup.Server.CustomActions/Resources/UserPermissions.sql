IF NOT EXISTS (select loginname from master.dbo.syslogins where name = N'IIS APPPOOL\DeviceManager')
BEGIN
    CREATE LOGIN [IIS APPPOOL\DeviceManager] FROM WINDOWS WITH DEFAULT_DATABASE=[master];
	USE [DeviceManager]
	CREATE USER [IIS APPPOOL\DeviceManager] FOR LOGIN [IIS APPPOOL\DeviceManager] WITH DEFAULT_SCHEMA=[dbo];
	ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\DeviceManager];
	ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\DeviceManager];
END
ELSE 
BEGIN
	IF NOT EXISTS (SELECT [name] FROM [sys].[database_principals]
       WHERE [type] = N'S' AND [name] = N'IIS APPPOOL\DeviceManager')
	BEGIN
		USE [DeviceManager]
		CREATE USER [IIS APPPOOL\DeviceManager] FOR LOGIN [IIS APPPOOL\DeviceManager] WITH DEFAULT_SCHEMA=[dbo];
		ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\DeviceManager];
		ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\DeviceManager];
	END
END