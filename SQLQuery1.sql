-- Превключваме към master, за да можем да drop-ваме CineMate
USE [master];
GO

-- Опционално: приключваме всички сесии към CineMate
ALTER DATABASE [CineMate]
SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

-- Изтриваме базата
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'CineMate')
    DROP DATABASE [CineMate];
GO
