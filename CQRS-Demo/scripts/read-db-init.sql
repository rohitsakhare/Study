IF DB_ID('ProductReadDb') IS NULL
BEGIN
    CREATE DATABASE ProductReadDb;
END
GO

USE ProductReadDb;
GO

IF OBJECT_ID('ProductView', 'U') IS NULL
BEGIN
    CREATE TABLE ProductView (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        Name NVARCHAR(200),
        Price DECIMAL(18,2)
    );
END
GO