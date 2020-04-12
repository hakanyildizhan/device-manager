CREATE LOGIN [IIS APPPOOL\DeviceManager] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
GO
USE [DeviceManager]
GO
/****** Object:  User [IIS APPPOOL\DeviceManager]    Script Date: 13.04.2020 01:04:01 ******/
CREATE USER [IIS APPPOOL\DeviceManager] FOR LOGIN [IIS APPPOOL\DeviceManager] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\DeviceManager]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\DeviceManager]
GO
