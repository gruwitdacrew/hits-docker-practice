IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220421180447_initial'
)
BEGIN
    CREATE TABLE [Roles] (
        [Id] uniqueidentifier NOT NULL,
        [Type] int NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220421180447_initial'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [BirthDate] datetime2 NOT NULL,
        [Email] nvarchar(256) NOT NULL,
        [Phone] nvarchar(max) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220421180447_initial'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220421180447_initial'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220421180447_initial'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220421180447_initial'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] uniqueidentifier NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220421180447_initial'
)
BEGIN
    CREATE TABLE [UserRoles] (
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220421180447_initial'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220421180447_initial'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220421180447_initial'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220421180447_initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [Roles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220421180447_initial'
)
BEGIN
    CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220421180447_initial'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [Users] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220421180447_initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [Users] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220421180447_initial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220421180447_initial', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220423091412_AddedAddresses'
)
BEGIN
    CREATE TABLE [Addresses] (
        [Id] uniqueidentifier NOT NULL,
        [StreetName] nvarchar(max) NOT NULL,
        [HouseNumber] nvarchar(max) NOT NULL,
        [EntranceNumber] nvarchar(max) NOT NULL,
        [FlatNumber] nvarchar(max) NOT NULL,
        [Note] nvarchar(max) NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [IsMainAddress] bit NOT NULL,
        [userId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Addresses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Addresses_Users_userId] FOREIGN KEY ([userId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220423091412_AddedAddresses'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName', N'Type') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] ON;
    EXEC(N'INSERT INTO [Roles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName], [Type])
    VALUES (''004f29dc-0a2d-4950-ac67-35161a1f1ba4'', N''14e7f47f-740d-47bd-88b2-df8a829b6cef'', N''Пользователь'', N''ПОЛЬЗОВАТЕЛЬ'', 1)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName', N'Type') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220423091412_AddedAddresses'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName', N'Type') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] ON;
    EXEC(N'INSERT INTO [Roles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName], [Type])
    VALUES (''dea06712-e644-4e53-97d8-3a0810bce815'', N''fe23aff2-2920-412a-9a2f-4616bb4e070e'', N''Администратор'', N''АДМИНИСТРАТОР'', 0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName', N'Type') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220423091412_AddedAddresses'
)
BEGIN
    CREATE INDEX [IX_Addresses_userId] ON [Addresses] ([userId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220423091412_AddedAddresses'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220423091412_AddedAddresses', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220427001752_nullableUserDataEdit'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Addresses]') AND [c].[name] = N'Note');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Addresses] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Addresses] ALTER COLUMN [Note] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220427001752_nullableUserDataEdit'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Addresses]') AND [c].[name] = N'EntranceNumber');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Addresses] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Addresses] ALTER COLUMN [EntranceNumber] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220427001752_nullableUserDataEdit'
)
BEGIN
    EXEC(N'UPDATE [Roles] SET [ConcurrencyStamp] = N''294a24a0-072a-4833-9dd7-20eb6e5dc5ed''
    WHERE [Id] = ''004f29dc-0a2d-4950-ac67-35161a1f1ba4'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220427001752_nullableUserDataEdit'
)
BEGIN
    EXEC(N'UPDATE [Roles] SET [ConcurrencyStamp] = N''8c8de9c9-b05e-4cea-8c31-7810ff00b645''
    WHERE [Id] = ''dea06712-e644-4e53-97d8-3a0810bce815'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220427001752_nullableUserDataEdit'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220427001752_nullableUserDataEdit', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220503055245_kappa123'
)
BEGIN
    EXEC(N'DELETE FROM [Roles]
    WHERE [Id] = ''004f29dc-0a2d-4950-ac67-35161a1f1ba4'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220503055245_kappa123'
)
BEGIN
    EXEC(N'DELETE FROM [Roles]
    WHERE [Id] = ''dea06712-e644-4e53-97d8-3a0810bce815'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220503055245_kappa123'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220503055245_kappa123', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220503060639_addressFieldNamingFix'
)
BEGIN
    ALTER TABLE [Addresses] DROP CONSTRAINT [FK_Addresses_Users_userId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220503060639_addressFieldNamingFix'
)
BEGIN
    EXEC sp_rename N'[Addresses].[userId]', N'UserId', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220503060639_addressFieldNamingFix'
)
BEGIN
    EXEC sp_rename N'[Addresses].[IX_Addresses_userId]', N'IX_Addresses_UserId', N'INDEX';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220503060639_addressFieldNamingFix'
)
BEGIN
    ALTER TABLE [Addresses] ADD CONSTRAINT [FK_Addresses_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220503060639_addressFieldNamingFix'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220503060639_addressFieldNamingFix', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510075851_Orders'
)
BEGIN
    CREATE TABLE [MenuItems] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Price] real NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [Category] int NOT NULL,
        [IsVegan] bit NOT NULL,
        [PhotoPath] nvarchar(max) NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_MenuItems] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510075851_Orders'
)
BEGIN
    CREATE TABLE [Orders] (
        [Id] int NOT NULL IDENTITY,
        [CreationTime] datetime2 NOT NULL,
        [DeliveryTime] datetime2 NOT NULL,
        [Cost] real NOT NULL,
        [Discount] real NOT NULL,
        [Status] int NOT NULL,
        [AddressId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Orders_Addresses_AddressId] FOREIGN KEY ([AddressId]) REFERENCES [Addresses] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Orders_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510075851_Orders'
)
BEGIN
    CREATE TABLE [MenuItemOrder] (
        [ItemsId] uniqueidentifier NOT NULL,
        [OrdersId] int NOT NULL,
        CONSTRAINT [PK_MenuItemOrder] PRIMARY KEY ([ItemsId], [OrdersId]),
        CONSTRAINT [FK_MenuItemOrder_MenuItems_ItemsId] FOREIGN KEY ([ItemsId]) REFERENCES [MenuItems] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_MenuItemOrder_Orders_OrdersId] FOREIGN KEY ([OrdersId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510075851_Orders'
)
BEGIN
    CREATE INDEX [IX_MenuItemOrder_OrdersId] ON [MenuItemOrder] ([OrdersId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510075851_Orders'
)
BEGIN
    CREATE INDEX [IX_Orders_AddressId] ON [Orders] ([AddressId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510075851_Orders'
)
BEGIN
    CREATE INDEX [IX_Orders_UserId] ON [Orders] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510075851_Orders'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220510075851_Orders', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510095516_OrdersRemoveAddressFK'
)
BEGIN
    ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_Addresses_AddressId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510095516_OrdersRemoveAddressFK'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'AddressId');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Orders] ALTER COLUMN [AddressId] uniqueidentifier NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510095516_OrdersRemoveAddressFK'
)
BEGIN
    ALTER TABLE [Orders] ADD [Address] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510095516_OrdersRemoveAddressFK'
)
BEGIN
    ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Addresses_AddressId] FOREIGN KEY ([AddressId]) REFERENCES [Addresses] ([Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510095516_OrdersRemoveAddressFK'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220510095516_OrdersRemoveAddressFK', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510095600_OrdersRemoveAddressFK2'
)
BEGIN
    ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_Addresses_AddressId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510095600_OrdersRemoveAddressFK2'
)
BEGIN
    DROP INDEX [IX_Orders_AddressId] ON [Orders];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510095600_OrdersRemoveAddressFK2'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'AddressId');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Orders] DROP COLUMN [AddressId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510095600_OrdersRemoveAddressFK2'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220510095600_OrdersRemoveAddressFK2', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510131024_OrderItemsMtoMRelFix1'
)
BEGIN
    DROP TABLE [MenuItemOrder];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510131024_OrderItemsMtoMRelFix1'
)
BEGIN
    CREATE TABLE [OrderMenuItems] (
        [OrderId] int NOT NULL,
        [ItemId] uniqueidentifier NOT NULL,
        [Amount] int NOT NULL,
        CONSTRAINT [PK_OrderMenuItems] PRIMARY KEY ([OrderId], [ItemId]),
        CONSTRAINT [FK_OrderMenuItems_MenuItems_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [MenuItems] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderMenuItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510131024_OrderItemsMtoMRelFix1'
)
BEGIN
    CREATE INDEX [IX_OrderMenuItems_ItemId] ON [OrderMenuItems] ([ItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20220510131024_OrderItemsMtoMRelFix1'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220510131024_OrderItemsMtoMRelFix1', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216055411_AddProductViewTable'
)
BEGIN
    CREATE TABLE [ProductViews] (
        [Id] uniqueidentifier NOT NULL,
        [MenuItemId] uniqueidentifier NOT NULL,
        [ViewDate] datetime2 NOT NULL,
        [UserId] nvarchar(max) NULL,
        [SessionId] nvarchar(max) NULL,
        [IpAddress] nvarchar(max) NULL,
        CONSTRAINT [PK_ProductViews] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProductViews_MenuItems_MenuItemId] FOREIGN KEY ([MenuItemId]) REFERENCES [MenuItems] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216055411_AddProductViewTable'
)
BEGIN
    CREATE INDEX [IX_ProductViews_MenuItemId] ON [ProductViews] ([MenuItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216055411_AddProductViewTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251216055411_AddProductViewTable', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216061005_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251216061005_InitialCreate', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216073138_AddCartAdditionsTable'
)
BEGIN
    DROP TABLE [ProductViews];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216073138_AddCartAdditionsTable'
)
BEGIN
    CREATE TABLE [CartAdditions] (
        [Id] uniqueidentifier NOT NULL,
        [MenuItemId] uniqueidentifier NOT NULL,
        [AdditionDate] datetime2 NOT NULL,
        [UserId] nvarchar(max) NULL,
        [SessionId] nvarchar(max) NULL,
        [IpAddress] nvarchar(max) NULL,
        CONSTRAINT [PK_CartAdditions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CartAdditions_MenuItems_MenuItemId] FOREIGN KEY ([MenuItemId]) REFERENCES [MenuItems] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216073138_AddCartAdditionsTable'
)
BEGIN
    CREATE INDEX [IX_CartAdditions_MenuItemId] ON [CartAdditions] ([MenuItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251216073138_AddCartAdditionsTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251216073138_AddCartAdditionsTable', N'8.0.0');
END;
GO

COMMIT;
GO

