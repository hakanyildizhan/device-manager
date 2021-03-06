USE [DeviceManager]
GO
IF NOT EXISTS(SELECT 1 FROM [dbo].[Setting] WHERE [Name] = N'usagePromptDuration')
BEGIN
	INSERT [dbo].[Setting] ([Name], [Value], [Description]) VALUES (N'usagePromptDuration', N'60', 'Duration for the reminder prompt to remain active on the client screen')
END
GO
IF NOT EXISTS(SELECT 1 FROM [dbo].[Setting] WHERE [Name] = N'serverVersion')
BEGIN
	INSERT [dbo].[Setting] ([Name], [Value], [Description]) VALUES (N'serverVersion', N'1.1.0', NULL)
END
ELSE
BEGIN
	UPDATE [dbo].[Setting] SET [Value] = N'1.1.0' WHERE [Name] = N'serverVersion'
END