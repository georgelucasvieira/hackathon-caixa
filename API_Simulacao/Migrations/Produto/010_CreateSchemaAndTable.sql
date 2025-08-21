USE db_produto;
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'db_produto')
    EXEC('CREATE SCHEMA db_produto');
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id
               WHERE t.name='PRODUTO' AND s.name='db_produto')
BEGIN
    CREATE TABLE db_produto.PRODUTO (
        CO_PRODUTO int NOT NULL PRIMARY KEY,
        NO_PRODUTO varchar(200) NOT NULL,
        PC_TAXA_JUROS numeric(10, 9) NOT NULL,
        NU_MINIMO_MESES smallint NOT NULL,
        NU_MAXIMO_MESES smallint NULL,
        VR_MINIMO numeric(18, 2) NOT NULL,
        VR_MAXIMO numeric(18, 2) NULL
    );
END
GO
