CREATE DATABASE FPF1_Login;
GO

USE FPF1_Login;
GO

CREATE TABLE Account (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL
);
GO

INSERT INTO Account (Username, Password)
VALUES
(N'admin', N'123456'),
(N'user01', N'123456'),
(N'user02', N'123456'),
(N'ngocanh', N'123456'),
(N'test', N'123456');
GO

SELECT * FROM Account;


SELECT name 
FROM sys.databases
WHERE name = 'FPF1_Login';

SELECT @@SERVERNAME AS ServerName;