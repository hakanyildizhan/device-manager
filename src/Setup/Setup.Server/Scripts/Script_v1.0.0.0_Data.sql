USE [DeviceManager]
GO
INSERT [dbo].[Setting] ([Name], [Value], [Description]) VALUES (N'lastDeviceListUpdate', N'12.04.2020 21:55:09', NULL)
INSERT [dbo].[Setting] ([Name], [Value], [Description]) VALUES (N'refreshInterval', N'60', N'Refresh interval for clients (seconds)')
INSERT [dbo].[Setting] ([Name], [Value], [Description]) VALUES (N'serverVersion', N'1.0', NULL)
INSERT [dbo].[Setting] ([Name], [Value], [Description]) VALUES (N'usagePromptInterval', N'3600', N'Interval (seconds) at which clients will be prompted to check devices back in')
