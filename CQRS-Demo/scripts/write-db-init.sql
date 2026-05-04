IF DB_ID('ProductWriteDb') IS NULL
BEGIN
    CREATE DATABASE ProductWriteDb;
END
GO

USE ProductWriteDb;
GO

IF OBJECT_ID('Products', 'U') IS NULL
BEGIN
    CREATE TABLE Products (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        Price DECIMAL(18,2) NOT NULL
    );
END
GO

IF OBJECT_ID('OutboxMessages', 'U') IS NULL
BEGIN
    CREATE TABLE OutboxMessages (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        Type NVARCHAR(200),
        Content NVARCHAR(MAX),
        Processed BIT DEFAULT 0,
        CreatedAt DATETIME DEFAULT GETUTCDATE()
    );
END
GO