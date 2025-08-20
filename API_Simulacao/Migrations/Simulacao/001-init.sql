CREATE DATABASE DbSimulacao;
GO

USE DbSimulacao;
GO

CREATE TABLE Simulacao (
    Id INT IDENTITY PRIMARY KEY,
    Descricao NVARCHAR(200) NOT NULL,
    DataCriacao DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE Simulacao (
    Id INT PRIMARY KEY,
    Tipo VARCHAR(100),
    ValorTotal DECIMAL(18,2)
);
GO

CREATE TABLE Parcelas (
    Id INT PRIMARY KEY,
    SimulacaoId INT,
    NumeroParcela INT,
    ValorParcela DECIMAL(18,2),
    FOREIGN KEY (SimulacaoId) REFERENCES Simulacao(Id)
);
GO
