CREATE DATABASE DbProduto;
GO

USE DbProduto;
GO
CREATE TABLE DbProduto.PRODUTO (
    CO_ PRODUTO int NOT NULL primary key,
    NO_PRODUTO varchar (200) NOT NULL,
    PC_TAXA_JUROS numeric (10, 9) NOT NULL,
    NU _MINIMO_MESES smallint NOT NULL,
    NU_MAXIMO_MESES smallint NULL,
    VR_MINIMO numeric (18, 2) NOT NULL,
    VR_MAXIMO numeric (18, 2) NULL
);
GO
