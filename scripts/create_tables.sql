-- Create ElevatorDB and tables (run in SQL Server)
IF DB_ID('ElevatorDB') IS NULL
BEGIN
    CREATE DATABASE ElevatorDB;
END
GO
USE ElevatorDB;
GO
IF OBJECT_ID('dbo.ElevatorEvents') IS NULL
BEGIN
    CREATE TABLE dbo.ElevatorEvents (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ElevatorId INT NOT NULL,
        EventType NVARCHAR(100) NOT NULL,
        Description NVARCHAR(1000) NULL,
        Timestamp DATETIME2 NOT NULL
    );
END
GO
IF OBJECT_ID('dbo.ElevatorStatuses') IS NULL
BEGIN
    CREATE TABLE dbo.ElevatorStatuses (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ElevatorId INT NOT NULL,
        CurrentFloor INT NOT NULL,
        Direction INT NOT NULL,
        OccupiedTargets INT NOT NULL,
        Timestamp DATETIME2 NOT NULL
    );
END
GO
