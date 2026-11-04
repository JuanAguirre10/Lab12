IF OBJECT_ID('Socios', 'U') IS NOT NULL DROP TABLE Socios;
IF OBJECT_ID('Membresias', 'U') IS NOT NULL DROP TABLE Membresias;
GO

CREATE TABLE Membresias (
    Id           INT IDENTITY(1,1) CONSTRAINT PK_Membresias PRIMARY KEY,
    Nombre       NVARCHAR(50) NOT NULL CONSTRAINT UQ_Membresias_Nombre UNIQUE,
    Precio       DECIMAL(10,2) NOT NULL,
    DuracionDias INT NOT NULL
);

CREATE TABLE Socios (
    Id               INT IDENTITY(1,1) CONSTRAINT PK_Socios PRIMARY KEY,
    Nombres          NVARCHAR(100) NOT NULL,
    Dni              CHAR(8) NOT NULL CONSTRAINT UQ_Socios_Dni UNIQUE,
    Telefono         NVARCHAR(9) NULL,
    FechaInscripcion DATE NOT NULL,
    MembresiaId      INT NOT NULL,
    CONSTRAINT FK_Socios_Membresias FOREIGN KEY (MembresiaId) REFERENCES Membresias (Id)
);
GO

INSERT INTO Membresias (Nombre, Precio, DuracionDias) VALUES
    (N'Mensual',     99.90,  30),
    (N'Trimestral', 269.90,  90),
    (N'Anual',      899.00, 365);

INSERT INTO Socios (Nombres, Dni, Telefono, FechaInscripcion, MembresiaId) VALUES
    (N'Carlos Quispe Huamán',     '45872136', '987654321', '2026-01-12', 1),
    (N'María Fernanda Rojas Díaz','72451983', '956312478', '2026-02-03', 1),
    (N'Luis Alberto Mamani Cruz', '40125896', NULL,        '2026-03-18', 2),
    (N'Rosa Elena Torres Salas',  '70982341', '913457826', '2026-04-25', 3),
    (N'Jorge Paredes Vílchez',    '46319057', '991284563', '2026-06-09', 1);
GO

SELECT * FROM Membresias;
SELECT * FROM Socios;
