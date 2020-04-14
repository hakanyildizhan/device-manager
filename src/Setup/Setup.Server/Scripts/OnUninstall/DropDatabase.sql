IF DB_ID('DeviceManager') IS NOT NULL
BEGIN
    BEGIN TRANSACTION [DropDbTran]
        BEGIN TRY
	        ALTER DATABASE DeviceManager SET multi_user WITH ROLLBACK IMMEDIATE;
	        ALTER DATABASE DeviceManager SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	        DROP DATABASE DeviceManager;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION [Tran1]
        END CATCH
END