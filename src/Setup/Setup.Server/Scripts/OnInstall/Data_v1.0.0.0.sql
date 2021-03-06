USE [DeviceManager]
GO
IF NOT EXISTS(SELECT 1 FROM [dbo].[Setting] WHERE [Name] = N'lastDeviceListUpdate')
BEGIN
	INSERT [dbo].[Setting] ([Name], [Value], [Description]) VALUES (N'lastDeviceListUpdate', N'01.01.2020 00:00:00', NULL)
END
GO
IF NOT EXISTS(SELECT 1 FROM [dbo].[Setting] WHERE [Name] = N'refreshInterval')
BEGIN
	INSERT [dbo].[Setting] ([Name], [Value], [Description]) VALUES (N'refreshInterval', N'60', N'Refresh interval for clients (seconds)')
END
GO
IF NOT EXISTS(SELECT 1 FROM [dbo].[Setting] WHERE [Name] = N'serverVersion')
BEGIN
	INSERT [dbo].[Setting] ([Name], [Value], [Description]) VALUES (N'serverVersion', N'1.0', NULL)
END
GO
IF NOT EXISTS(SELECT 1 FROM [dbo].[Setting] WHERE [Name] = N'usagePromptInterval')
BEGIN
	INSERT [dbo].[Setting] ([Name], [Value], [Description]) VALUES (N'usagePromptInterval', N'3600', N'Interval (seconds) at which clients will be prompted to check devices back in')
END
GO
IF NOT EXISTS(SELECT 1 FROM [dbo].[AspNetRoles] WHERE [Name] = N'Admin')
BEGIN
	INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (NEWID() , N'Admin')
END
