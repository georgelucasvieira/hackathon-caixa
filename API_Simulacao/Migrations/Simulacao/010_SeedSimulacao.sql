TRUNCATE TABLE parcelas, simulacao RESTART IDENTITY CASCADE;

INSERT INTO solicitacao_simulacao (id, data_criacao, prazo, valor_desejado) VALUES
  (1, NOW(), 24, 5000.00),
  (2, NOW(), 36, 6000.00);

INSERT INTO simulacao (id, tipo, data_criacao, solicitacao_id) VALUES
  (1, 'SAC',   NOW(), 1),
  (2, 'PRICE', NOW(), 1),
  (3, 'SAC',   NOW(), 2),
  (4, 'PRICE', NOW(), 2);

INSERT INTO parcelas (simulacao_id, numero, valor_amortizacao, valor_juros, valor_prestacao) VALUES
  (1, 1, 1002.12, 100.50, 1102.62),
  (1, 2, 1002.12, 100.50, 1102.62),
  (1, 3, 1002.12, 100.50, 1102.62),
  (1, 4, 1002.12, 100.50, 1102.62),

  (2, 1,  500.12,  50.01,  550.13),
  (2, 2,  500.12,  50.01,  550.13),
  (2, 3,  500.12,  50.01,  550.13),
  (2, 4,  500.12,  50.01,  550.13),

  (3, 1,  123.12,  14.32,  166.44),
  (3, 2,  123.12,  14.32,  166.44),
  (3, 3,  123.12,  14.32,  166.44),
  (3, 4,  123.12,  14.32,  166.44),

  (4, 1, 8721.12,  81.08, 8802.20),
  (4, 2, 8721.12,  81.08, 8802.20),
  (4, 3, 8721.12,  81.08, 8802.20),
  (4, 4, 8721.12,  81.08, 8802.20);

SELECT setval('solicitacao_simulacao_id_seq', (SELECT MAX(id) FROM solicitacao_simulacao));
SELECT setval('simulacao_id_seq', (SELECT MAX(id) FROM simulacao));
SELECT setval('parcelas_id_seq', (SELECT MAX(id) FROM parcelas));
