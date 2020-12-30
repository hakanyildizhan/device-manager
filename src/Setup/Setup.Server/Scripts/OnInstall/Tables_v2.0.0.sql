DECLARE @CurrentMigration [nvarchar](max)

IF object_id('[dbo].[__MigrationHistory]') IS NOT NULL
    SELECT @CurrentMigration =
        (SELECT TOP (1) 
        [Project1].[MigrationId] AS [MigrationId]
        FROM ( SELECT 
        [Extent1].[MigrationId] AS [MigrationId]
        FROM [dbo].[__MigrationHistory] AS [Extent1]
        WHERE [Extent1].[ContextKey] = N'DeviceManager.Entity.Migrations.Configuration'
        )  AS [Project1]
        ORDER BY [Project1].[MigrationId] DESC)

IF @CurrentMigration IS NULL
    SET @CurrentMigration = '0'

IF @CurrentMigration < '202009232305255_Initial-V2.0'
BEGIN
    CREATE TABLE [dbo].[Client] (
        [DomainUsername] [nvarchar](128) NOT NULL,
        [FriendlyName] [nvarchar](max),
        CONSTRAINT [PK_dbo.Client] PRIMARY KEY ([DomainUsername])
    )
    CREATE TABLE [dbo].[Device] (
        [Id] [int] NOT NULL IDENTITY,
        [Group] [nvarchar](50),
        [Name] [nvarchar](50) NOT NULL,
        [HardwareInfo] [nvarchar](max) NOT NULL,
        [Address] [nvarchar](max) NOT NULL,
        [Address2] [nvarchar](max),
        [ConnectedModuleInfo] [nvarchar](max),
        [IsActive] [bit] NOT NULL,
        CONSTRAINT [PK_dbo.Device] PRIMARY KEY ([Id])
    )
    CREATE TABLE [dbo].[Job] (
        [Id] [int] NOT NULL IDENTITY,
        [Type] [int] NOT NULL,
        [IsIndependent] [bit] NOT NULL,
        [Schedule] [nvarchar](max) NOT NULL,
        [IsEnabled] [bit] NOT NULL,
        CONSTRAINT [PK_dbo.Job] PRIMARY KEY ([Id])
    )
    CREATE UNIQUE INDEX [IX_Type] ON [dbo].[Job]([Type])
    CREATE TABLE [dbo].[AspNetRoles] (
        [Id] [nvarchar](128) NOT NULL,
        [Name] [nvarchar](256) NOT NULL,
        CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY ([Id])
    )
    CREATE UNIQUE INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]([Name])
    CREATE TABLE [dbo].[AspNetUserRoles] (
        [UserId] [nvarchar](128) NOT NULL,
        [RoleId] [nvarchar](128) NOT NULL,
        CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId])
    )
    CREATE INDEX [IX_UserId] ON [dbo].[AspNetUserRoles]([UserId])
    CREATE INDEX [IX_RoleId] ON [dbo].[AspNetUserRoles]([RoleId])
    CREATE TABLE [dbo].[Session] (
        [Id] [int] NOT NULL IDENTITY,
        [StartedAt] [datetime] NOT NULL,
        [FinishedAt] [datetime],
        [IsActive] [bit] NOT NULL,
        [Client_DomainUsername] [nvarchar](128) NOT NULL,
        [Device_Id] [int] NOT NULL,
        CONSTRAINT [PK_dbo.Session] PRIMARY KEY ([Id])
    )
    CREATE INDEX [IX_Client_DomainUsername] ON [dbo].[Session]([Client_DomainUsername])
    CREATE INDEX [IX_Device_Id] ON [dbo].[Session]([Device_Id])
    CREATE TABLE [dbo].[Setting] (
        [Name] [nvarchar](128) NOT NULL,
        [Value] [nvarchar](max) NOT NULL,
        [Description] [nvarchar](max),
        CONSTRAINT [PK_dbo.Setting] PRIMARY KEY ([Name])
    )
    CREATE TABLE [dbo].[UpdateJob] (
        [Id] [int] NOT NULL IDENTITY,
        [Status] [int] NOT NULL,
        [NewVersion] [nvarchar](max),
        [Uri] [nvarchar](max),
        [Info] [nvarchar](max),
        [LastUpdate] [datetime] NOT NULL,
        [Job_Id] [int] NOT NULL,
        CONSTRAINT [PK_dbo.UpdateJob] PRIMARY KEY ([Id])
    )
    CREATE INDEX [IX_Job_Id] ON [dbo].[UpdateJob]([Job_Id])
    CREATE TABLE [dbo].[AspNetUsers] (
        [Id] [nvarchar](128) NOT NULL,
        [Email] [nvarchar](256),
        [EmailConfirmed] [bit] NOT NULL,
        [PasswordHash] [nvarchar](max),
        [SecurityStamp] [nvarchar](max),
        [PhoneNumber] [nvarchar](max),
        [PhoneNumberConfirmed] [bit] NOT NULL,
        [TwoFactorEnabled] [bit] NOT NULL,
        [LockoutEndDateUtc] [datetime],
        [LockoutEnabled] [bit] NOT NULL,
        [AccessFailedCount] [int] NOT NULL,
        [UserName] [nvarchar](256) NOT NULL,
        CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY ([Id])
    )
    CREATE UNIQUE INDEX [UserNameIndex] ON [dbo].[AspNetUsers]([UserName])
    CREATE TABLE [dbo].[AspNetUserClaims] (
        [Id] [int] NOT NULL IDENTITY,
        [UserId] [nvarchar](128) NOT NULL,
        [ClaimType] [nvarchar](max),
        [ClaimValue] [nvarchar](max),
        CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY ([Id])
    )
    CREATE INDEX [IX_UserId] ON [dbo].[AspNetUserClaims]([UserId])
    CREATE TABLE [dbo].[AspNetUserLogins] (
        [LoginProvider] [nvarchar](128) NOT NULL,
        [ProviderKey] [nvarchar](128) NOT NULL,
        [UserId] [nvarchar](128) NOT NULL,
        CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey], [UserId])
    )
    CREATE INDEX [IX_UserId] ON [dbo].[AspNetUserLogins]([UserId])
    ALTER TABLE [dbo].[AspNetUserRoles] ADD CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[AspNetUserRoles] ADD CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[Session] ADD CONSTRAINT [FK_dbo.Session_dbo.Client_Client_DomainUsername] FOREIGN KEY ([Client_DomainUsername]) REFERENCES [dbo].[Client] ([DomainUsername]) ON DELETE CASCADE
    ALTER TABLE [dbo].[Session] ADD CONSTRAINT [FK_dbo.Session_dbo.Device_Device_Id] FOREIGN KEY ([Device_Id]) REFERENCES [dbo].[Device] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[UpdateJob] ADD CONSTRAINT [FK_dbo.UpdateJob_dbo.Job_Job_Id] FOREIGN KEY ([Job_Id]) REFERENCES [dbo].[Job] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[AspNetUserClaims] ADD CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[AspNetUserLogins] ADD CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    CREATE TABLE [dbo].[__MigrationHistory] (
        [MigrationId] [nvarchar](150) NOT NULL,
        [ContextKey] [nvarchar](300) NOT NULL,
        [Model] [varbinary](max) NOT NULL,
        [ProductVersion] [nvarchar](32) NOT NULL,
        CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY ([MigrationId], [ContextKey])
    )
    INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
    VALUES (N'202009232305255_Initial-V2.0', N'DeviceManager.Entity.Migrations.Configuration',  0x1F8B0800000000000400ED5D596F1CB9117E0F90FFD098A724D06A74AC0D479076A195ACDD492C59F0C88BBC195437356AB88FD93E6C09C1FEB23CE427E52F84EC83CDBBC96E4ECFC8D8B7191E1FC962B1585524ABFFF79FFF9EFEF81447DE1798E5619A9CCD0EF70F661E4CFC340893D5D9AC2C1EBE7B33FBF1873FFFE9F46D103F79BFB6E58E71395433C9CF668F45B13E99CF73FF11C620DF8F433F4BF3F4A1D8F7D3780E82747E7470F0F7F9E1E11C228819C2F2BCD30F65528431ACFEA0BF1769E2C3755182E83A0D609437E9286759A17A372086F91AF8F06C7609BF843EBC060958C16CFF2DC2299EF71140019F8A99771E8500F56909A387990792242D40817A7CF23187CB224B93D5728D124074F7BC86A8DC038872D88CE4A42B6E3AA883233CA87957B185F2CBBC48634BC0C3E3864A73BEFA205ACF0815111D6B3AE15157B43C9B5D44214C10C1F8A64E2EA20C17D3139AFCAD50F63C59D93DC22E87FB88AFF60FF6BC8B322ACA0C9E25B02C3210ED79B7E57D14FAFF84CF77E967989C256514D1DD461D47794C024ABACDD235CC8AE70FF0A119CC251A4198A039CE12F47FE6CD598C390F422014F56B02208641AB60E65D83A7773059158F687D1CBD997957E1130CDA9486833E26215A34A8529195E8EF0D1A08B88F20C9A708BC2CD20CFE0C1398810206B7A02850B367B39B34117BCEF5F32A43E40EA2E71B7D2FD14FA35ED28D9DCE3B16D1324E3DD56319A72EB375C65904F6CC82EBD4035D24C5F1D1A0C95E04B01A6ADF84FF9CA5E55A33D3AF0EAC275A6CA4879B0CDB10A8A06FF41790055F410617C943EA80952D5B3F0F820CE6F9D61A3E72BC7AC586D07A4BA08F780EEDA865E48ACCFA3617F9B95F845F0833FD94A61104490F918C25CF3FD2FBB1620741FC21737A6611B7435805E9519868759A25AF2FF24512C035DAB12A4DC386274430AC0862469E7EC92EF2B7092E1E6C88ADDB79F99046DA6DF59A287BE7F9FA0616FB6DC58685AF3204F735CD3EEFD3887B9E71BD6E0D1CE11560B2068E0FEF1F8EDFBC7A0D82E3D7DFC3E357D3AF07974ADAA86DF2E8D56B27ADDE802FE1AA9A7AAE7DAC9EA21DEB038CAADCFC315C370B949EEF4F4DB1AB2C8DF17F96BFEADC4FCBB4CCB00E77972A8BDC816C058B912C8DA1DCB3758BBAFBAC8D7B2AB2B7B4281ED09095D03631F56A68FBBBD9768D396E8994AACA501EA71F34307FE8087DBB71013254EF9C6CEB9708E62E8CEDF9E82A4CC2FC510EB5096D5327615B9F8844C4368CF1A92DD28957364710AD5CB64CACEABAD45ADB9A2EB545C42ED539CA2E35D9A324FD121645B5F2C7AEBB0A66EBEBEE6690FBA84733D8B2D3E85710955B509B2F61EE67E1BA765F6EC755F5711D206A39301B09D0D619F4056C0C4599F3E6639B6AAB71C3AF0D6137EEBAF898859B778F4CE1837907F2A26657FBAD59B90B554B48B2059175F1A92AD1ED404C86B001B1B9A3F61FACFE9EFB7E5A8E3FCBA0A05EF8329FCE0A781B83307260141BB48266EB21CC625B378C88760BF21CD984C12F207FDCF8725C42BFCC10EF200918EB1CF96E5ABB7D44FAC04D19DFC36CCAB69C4DCDDDD7F40AF868071AE4719308C3D4FF9C96C5DB24C042F063E1DBDA1804C04977907441AAF7156266185CD4224BBF2FF7EC5948606DDB2975118130967BA52879FAA92D46ED1042AEB84D88456CCDA777E92A4CFABBD7169377AFCED576AF2962DB3D0CD4DFBBA694BC7355A6B66F750967FEBC6A26DC3BF42AD8DDF7E8EDBAFEBD2D7760357DCCC9D1C60E36714BAE6CDAD1ABA15AF8EE574305BBFBABA1EA264AFE120658E3307073B78511BC5179B907BD7FCD713D9B7A3930C39CBAF16964807AB994317B685F8B85457E15815577E76EF4413E2EECD65043E34773163D237AD1F29A25F635C42A6F2B8C1EA1FFB935B72BA184A82BCC0F536591E4058822AED29148DC9A8C7AD2B68E1542DC7AE246D3B6C6DD36756F611254BC6B4659CCF77759B8425DC5BABA9CB27CA5E6248314FF5E5FFC228DD711A42BBCE9C12F91D20F03AAC2E16B7D8DDA3E20C58F8D58E33CCF533FAC265B72ADA03914661B455691D77F42DCF9F1D87B0AD768CAC3359A649482678617E5EF934B8809E5E1431A7C01F602E43E0824DE75D40F8B8EB57AACA463DD6933DBB9BF096DA2FD05F148822F0223DECF11CF8649216E4661E2876B10F55289AB69A838E2B19336F89CCBF6DE4C2F254C1A971F2EE30E9076B849E9A3D0E99CE2383D23724767AAB9569DA375F34CCE78FBA657092A619EF6406F53FC3C8050CD815EDF98F8D33D2784E2CE0429D0F698710708C5BA9D554352F8A0BB1151C743E68492FBAED9E38EDD2092E87E518E49E38BA1C845BBDA2711FF1A079042FC37CE898DC87F35952690FF6A529834AEBC873485FC9738DA4C269CF7BAED0A2372AE3E0523367E818D33224BA589199125C58B62C4DAA76A32DF9C837557D890F5EA4EAF0D2B4934310F3274D83116ACED656C5DA31AC4BE636C6AF2705130D9513F1BABBD35EB79B6C0E04B583097D7F299D7B965381557D0695880BA5F328056F5EB01409A8FAC76A510F5546D8E3884BAACCDD903C273BE0EB05B1D3DA08D622CC3226A762F4475B94D0ED15C9FEB81205AA70C84D263FB60EABBE122022DCA2CA8DC1EEB69C9DCE86416B0ED719C16B6D96139586A258AD8F4FD78AAA0FA163D2F228C5C26646484AD055163E4E1A070249CCD6F1BECC00D88C2DF681509A233DC4D4C776A00DD2AD2D04261AC533044C6391B7D2BDAD4A39759E326F6F8D0D1731638054304F4E8D173F7B6C4C16B2C6C031B9BEA332DB83483975BD5148E0C61C8C025771224A3EF319D4D8D679A0EB5D4D191406DEC2A24413B00A7546965AF9E2A323BCED4921B4C15CEF25250A51D8053AA3442574F14894D6168550C26096B0538DA31DA933DA2B892BCD3791D8FA349389D2B02779C5E83F51AAB3C5DCD26C55BD6513C2EBE5BDA07B5886B8CB9CF509957B3494B459A21159BCBC5473801BC0AB3BCB80405B807F82CF22288856252355DA1C3B44DB29AB83885AD3AD396C7BF2546017F2E27DF803BA25EA171C6D83CAA2E8F08FBA558D1C381554004B29E3818176954C6893C8FE750352A1BB582C6647344C4D3393742C16613682A58D3EC14194D60BBD13A9B40B90E613081AA8A2A5263C39626B0CCD055D76EA24DD0004D92398638C9AAC95521B09121682436C71C91447BA0C148A235CE9114483867D62149A333D0A0D202E6F8DDDB38861B48EACE2CB54AAF73B6CE24CAAAC12293D6DACC0AABAFC2D0F5EB149B9965822AB0D3CB64996376B11568B82ED5A677E4AA34DB3392BC339CA7D2D606B21EE3A9B2E7417DF5CD30A3B9A8DED214894AACF3E9227EC0E153A6865011BE7541D3A457B9A5D528EDA50E1A4575D1636B53485C1FCE66AEF5BADA4F98B2E6669617F5469E11AB5DB2850A4DBD926714682AFD1BD40E88E7DC21F7D40EF721DCA3A8B9394DB8B9E54E433449E618CC336CC69EA3337666C6297FA1B339EFCE47EC675D53776352A3BAD1CB898C2ACD626BA75E4E331C48A59BA3558FA399CD0A2758C81BC1BCB1B567E857CD340E9DBE3B3CAC383B1ACABED4E1DC0006D6D5DE0C0B378F82698026C912837A572A805179E6A8ECD35F1A93CDB158AEECFB5E66D5B25916BDA45FF1329DA43306E129282A2F6161D60AEF76191357C8B558F8E20B5E66FD8BD903B0257DE6F32C9C44E2235FC65B24665B0862F2E297371D76D872539EA03930DDEABB05E36C3705C66664A31BD38F7A6DC9380DBB644B2C89A649A7EF245F29CF201DF0557DB9641C5F2930D4D28879B1C80A23ED334B3526F30C9111F8BA67986A3C3BEEDD348FB0079852BF03B9DD62E75C20D50C5D08F87456500C649759441A599FC335807DC771D545D216D6BABBCABB9C8345A2BA3FFC49F4E0B96EEFF258CE755B6DFC5CB357770612AF01B1A3A149B75ED49C7237946C2DFDBA96B1312F219EF446D240DA610407F329BD6CB523D329DC19E18B90CDA04921FFC99D91E6BE46FF1760840B1C759199D7EE6A88F59FF302C6FBB8C0FEF2B7A815E76D01C43EE103CC8BFA75F2ECE8E0F088FB74CCEE7CC6659EE70113434CF92D1776CAB6F8E194E40BC8FC47908941076C83C34ABE77D282FF25064F7FA511C77FD3640001C7C58309898EA18BF8624934E69B21D2A9C05FF4D0D14DC4944D800672F0174094933BE4AB1E4EC08E06F09D08A6F91AC7285C3E06F27D58B8FCDAC6B7B120E8B044A1344E24BE37F17436FBB7B7C83F26E16F253CF1EE10F5BDDFDD7CF3A27F564420FE7B17E39859F886854346515F5D9882639CED357211A766155CFEA4BA928B7F55C97B12FEE183F1B9A5B9E6030BA4A3BFBF882F1798937CF1AF4F75D53DEF7D8694B913EF8023F490E967BF6760D59BBAEA88DE587FE5E0DB90CBC24704B07953B8F988800C6A331BAA64B3977B6806709614C929DB53DE06A30D92748D5464BBE38ACF25772DA68AE0EF6C476182148EDBBF25C1F4376107291C242F59C250D1E8AB264687A01FA5AF5361E6C7E9FDAE0C08315CFC6021DC7AB9ECE4485D6B13424479F3E145A9A54C8475292AA7560E0FA83E64AF9305531FC590D280E9A3102541D15DE13921A12AE8F9102C65C0F3A1FA903C00FA90AE29839F0F11CB7CE87373ADAAADB945834D72E1E0C5EEB0BB65C40951A8C739ED8448D39BD0B94CA2498FB3E55F589466675BE7AD1884D919F636F95EC55ACE62C0EE4AD8D7CEBBA58DE0B3F168AF530678D53C483275BD793B15D795BE813351CC55A67D12A54916B96923ACA37CAE6F7BFE6AC540EA775162B3C677882664A39EA8B7F4E59E8922D26E998D9441039CCB1E1BD6D15E4F9A2288A351EC5F7269688AA0BC4CCB5430363AC09A79A8E141BC227FDCE49C51742FA9C4C6D4D79E260EF6691EFE78CB013E2561F3149ACF44818EA7D67C54F7F92D0E287737A4F10E315763682A986BA2E0C5533397EA52FF0E33977198E21DE2AD6D596C5BE42C63836DEBC187C5E067FC9436B7A5D9ABA5EAA0C2F5DDDBB359708F0FA46A9F8555C0614DBC6119B6792C6255286219AA49F05CD6112260B3D9B246EA6FB8C9A3F1A91AEBD68CB2C1AE88BA51751840BE6162D308ED911C593396B19075A190E5F0967192F56192654DD8455126E2556C86CED5CF881D1B342A9F960F9A32FA66157143756D371A81B6EDA68CBE6D4574CE6D846D96C6CB94C504EFD9E92428CAE8E25B0DD3CCCA723A40A46EC09C40609E46B91B9E8338CCEC7642874FDCDEF09C055AA6441A8959A71B962008B907500E86B6D150CA424C5C4578FA3EF359B138E531E9772D72F2682230625BF140D92D115C074A1E4D029732DA2230B2F8980DA9F2A8DF615C1FD92173220F571DC469F3428456E249197CDBABB525B81EB545B8E3C96B5800B4D4C17956840FC02F5036BE7D417F4CF36D7C0F8345F2BE2CD66581860CE3FB8839EDC53689AEFD2AFA33DBE7D3F7D515C5DCC5105037432CADDE273F9561D47D0FF34A7220AA80C0C64E73D701CF6581EF3CAC9E09D24D9A180235E42336DA1D8CD71102CBDF274B802F0BDBF70DB1DF3BB802FE7377FCAD02E99F0896ECA79721586520CE1B8CAE3EFA8B7838889F7EF83F900D3134FB9F0000 , N'6.4.0')
END