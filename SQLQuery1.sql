-- ������������ ��� master, �� �� ����� �� drop-���� CineMate
USE [master];
GO

-- ����������: ����������� ������ ����� ��� CineMate
ALTER DATABASE [CineMate]
SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

-- ��������� ������
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'CineMate')
    DROP DATABASE [CineMate];
GO
