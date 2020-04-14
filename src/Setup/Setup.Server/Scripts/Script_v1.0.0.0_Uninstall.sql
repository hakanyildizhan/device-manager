IF EXISTS (select loginname from master.dbo.syslogins where name = N'IIS APPPOOL\DeviceManager')
BEGIN
	DROP LOGIN [IIS APPPOOL\DeviceManager];
	IF DB_ID('DeviceManager') IS NOT NULL
	BEGIN
		USE [DeviceManager]
		DROP USER [IIS APPPOOL\DeviceManager];
	END
END

IF DB_ID('DeviceManager') IS NOT NULL
BEGIN
	ALTER DATABASE DeviceManager SET multi_user WITH ROLLBACK IMMEDIATE;
	ALTER DATABASE DeviceManager SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE DeviceManager;
END