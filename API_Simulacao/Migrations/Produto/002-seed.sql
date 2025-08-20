USE DbProduto;
GO

INSERT INTO
    DbProduto.PRODUTO (
        CO_PRODUTO,
        NO_PRODUTO,
        PC_TAXA_JUROS,
        NU_MINIMO_MESES,
        NU_MAXIMO_MESES,
        VR_MINIMO,
        VR_MAXIMO
    )
VALUES (
        1,
        'Produto 1',
        0.017900000,
        0,
        24,
        200.00,
        10000.00
    )

INSERT INTO
    DbProduto.PRODUTO (
        CO_PRODUTO,
        NO_PRODUTO,
        PC_TAXA_JUROS,
        NU_MINIMO_MESES,
        NU_MAXIMO_MESES,
        VR_MINIMO,
        VR_MAXIMO
    )
VALUES (
        2,
        'Produto 2',
        0.017500000,
        25,
        48,
        10001.00,
        100000.00
    )

INSERT INTO
    DbProduto.PRODUTO (
        CO_PRODUTO,
        NO_PRODUTO,
        PC_TAXA_JUROS,
        NU_MINIMO_MESES,
        NU_MAXIMO_MESES,
        VR_MINIMO,
        VR_MAXIMO
    )
VALUES (
        3,
        'Produto 3',
        0.018200000,
        49,
        96,
        100000.01,
        1000000.001
    )

INSERT INTO
    DbProduto.PRODUTO (
        CO_PRODUTO,
        NO_PRODUTO,
        PC_TAXA_JUROS,
        NU_MINIMO_MESES,
        NU_MAXIMO_MESES,
        VR_MINIMO,
        VR_MAXIMO
    )
VALUES (
        4,
        'Produto 4',
        0.015100000,
        96,
        null,
        1000000.01,
        null
    )