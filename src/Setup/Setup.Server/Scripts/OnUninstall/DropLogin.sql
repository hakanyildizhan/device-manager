IF EXISTS (select loginname from master.dbo.syslogins where name = N'IIS APPPOOL\DeviceManager')
BEGIN
    BEGIN TRY
        DROP LOGIN [IIS APPPOOL\DeviceManager];
    END TRY
    BEGIN CATCH
    END CATCH
END