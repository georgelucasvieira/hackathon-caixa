# API_Simulacao

API construída para simular produtos e gerar relatórios de telemetria, com foco em **performance e simplicidade**.

---

## Como executar

A forma mais simples de rodar a aplicação é usando Docker Compose:

```bash
docker compose up --build
```

Isso irá:

* Construir as imagens do backend e do banco de dados.
* Iniciar os containers com as configurações corretas.
* Rodar migrations automaticamente no PostgreSQL.

Após isso, a API estará disponível em `http://localhost:8080`.

Swagger disponível em: `http://localhost:8080/swagger/index.html`

---

## Racional por trás de algumas decisões

Esta API foi escrita com ênfase maior em **performance**, e algumas decisões refletem esse foco:

1. **PostgreSQL como banco local**

   * A imagem Docker do PostgreSQL é mais leve e otimizada para múltiplos ambientes, comparada ao SQL Server que é mais otimizado para Windows Server.
   * Permite testes consistentes em todos os servidores.

2. **Queries nativas e customizadas com Dapper MicroORM**

   * Em vez de depender de EF Core e operações LINQ, usei Queries nativas com o **MicroORM Dapper**.
   * As queries retornam dados **diretamente no formato desejado** para a API, evitando múltiplas serializações/deserializações.
   * Isso melhora o desempenho e reduz overhead de memória.

3. **Cache para a tabela de Produtos**

   * A tabela de produtos muda com pouca frequência. Usar **MemoryCache** evita consultas repetidas e melhora o tempo de resposta.

4. **Operações assíncronas (`async/await`)**

   * Todos os acessos a banco e serviços externos são **assíncronos**, garantindo que a API não bloqueie threads e escale melhor sob carga.

5. **Minimal APIs do .NET**

   * Usei **endpoints minimalistas** em vez de controllers tradicionais, para reduzir boilerplate e manter a aplicação enxuta.

6. **Validações com Data Annotations**

   * Os DTOs utilizam **Data Annotations** (`[Required]`, `[Range]`, etc.) para validação automática, garantindo consistência das requisições sem precisar escrever lógica manual em todos os endpoints.

7. **Formatação de datas e métricas**

   * Campos como `dataReferencia` e `percentualSucesso` são tratados para **retornar JSON no formato ideal** (`YYYY-MM-DD` para datas e 2 casas decimais para percentuais).
---

## Endpoints principais

| Endpoint                | Método | Descrição                                                    |
| ----------------------- | ------ | ------------------------------------------------------------ |
| `/simulacoes`           | POST   | Realiza uma simulação de produto.                            |
| `/simulacoes`           | GET    | Lista simulações paginadas.                                  |
| `/simulacoes/relatorio` | GET    | Retorna relatório diário de uma API específica.              |
| `/telemetria/metricas`  | GET    | Retorna métricas agregadas de telemetria para todas as APIs. |

> Todos os endpoints retornam **JSON** pronto para consumo, com validação automática e respostas consistentes.

## Regras de negócio interpretativas?
Algumas regras ficaram a cargo da nossa interpretação. Os exemplos maiores são os endpoints `GET /simulacoes` e `GET /simulacoes/relatorio`. A fim de manter o JSON como solicitado, precisamos fazer uma escolha se vamos efetuar os cálculos com a simulação PRICE ou SAC. Se tentarmos fazermos com as duas, o retorno pode não haver sentido e/ou precisarmos alterar os campos do JSON de saída.

Nesse sentido, **optei por utilizar a simulação PRICE** por ter a prestação fixa e assim ser mais consistente com o que foi pedido.