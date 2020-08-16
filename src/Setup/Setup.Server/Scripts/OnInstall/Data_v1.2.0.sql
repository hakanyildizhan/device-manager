USE [DeviceManager]
GO
IF NOT EXISTS(SELECT 1 FROM [dbo].[Setting] WHERE [Name] = N'serverVersion')
BEGIN
	INSERT [dbo].[Setting] ([Name], [Value], [Description]) VALUES (N'serverVersion', N'1.2.0', NULL)
END
ELSE
BEGIN
	UPDATE [dbo].[Setting] SET [Value] = N'1.2.0' WHERE [Name] = N'serverVersion'
END