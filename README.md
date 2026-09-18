# LojaTintas — CP1 + CP2 + CP3 + CP4

## Integrantes

| Nome | RM |
|------|-----|
| Andrei de Paiva Gibbini | 563061 |

---

## Domínio

Sistema de gerenciamento para uma loja de tintas: cadastro de produtos, controle de estoque, pedidos de venda e relacionamento com fornecedores e fabricantes.

## Entidades

| Entidade | PK |
|---|---|
| `Categoria` | `int Id` |
| `Fabricante` | `int Id` |
| `Produto` | `Guid Id` |
| `Estoque` | `int Id` |
| `Fornecedor` | `int Id` |
| `ProdutoFornecedor` | `(Guid ProdutoId, int FornecedorId)` |
| `Cliente` | `Guid Id` |
| `Pedido` | `Guid Id` |
| `ItemPedido` | `int Id` |

## Relacionamentos

| Relacionamento | Cardinalidade |
|---|---|
| Categoria → Produto | 1:N |
| Fabricante → Produto | 1:N |
| Produto → Estoque | 1:1 |
| Produto ↔ Fornecedor | N:N via `ProdutoFornecedor` |
| Cliente → Pedido | 1:N |
| Pedido → ItemPedido | 1:N |
| Produto → ItemPedido | 1:N |

## Arquitetura

Clean Architecture em 4 projetos:

- **Domain** — entidades e enums, sem dependências externas
- **Application** — interfaces dos repositórios
- **Infrastructure** — EF Core, DbContext, Fluent API, implementações
- **API** — entry point, DI, controllers REST, DTOs, Swagger, GlobalExceptionHandler, health checks e logs
- **Domain.Tests** / **Application.Tests** — testes xUnit (unidade de domínio e de aplicação)

## Banco de Dados

SQLite — arquivo `loja_tintas.db` criado automaticamente na raiz da API.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=loja_tintas.db"
  }
}
```

## Como executar

```bash
dotnet restore

dotnet ef database update --project LojaTintas.Infrastructure --startup-project LojaTintas.API

dotnet run --project LojaTintas.API
```

## Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/health` | Health check (self + banco + dependência externa), JSON detalhado |
| `GET` | `/api/categorias` | Lista categorias |
| `GET` | `/api/categorias/{id}` | Busca categoria por id |
| `POST` | `/api/categorias` | Cria categoria |
| `PUT` | `/api/categorias/{id}` | Atualiza categoria |
| `DELETE` | `/api/categorias/{id}` | Remove categoria |
| `GET` | `/api/produtos` | Lista produtos |
| `GET` | `/api/produtos/{id}` | Busca produto por id |
| `GET` | `/api/produtos/por-categoria/{categoriaId}` | Lista produtos de uma categoria |
| `POST` | `/api/produtos` | Cadastra produto |
| `GET` | `/api/clientes` | Lista clientes |
| `GET` | `/api/clientes/{id}` | Busca cliente por id |
| `POST` | `/api/clientes` | Cadastra cliente |
| `GET` | `/api/pedidos` | Lista pedidos |
| `GET` | `/api/pedidos/{id}` | Busca pedido com itens |
| `POST` | `/api/pedidos` | Cria pedido |

## Swagger

Com a API em execução (ambiente Development), a documentação interativa fica em:

```
http://localhost:5169/swagger
```

Exemplos de chamada prontos em `LojaTintas.API/LojaTintas.API.http`.

## Repositório genérico

`IRepository<T>` (Application) + `Repository<T>` (Infrastructure, EF Core) — registrado na DI como serviço aberto:

```csharp
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```

Usado diretamente pelo `CategoriasController` (CRUD completo). Convive com os repositórios específicos por agregado do CP2 (`IProdutoRepository`, `IClienteRepository`, `IPedidoRepository`, `IEstoqueRepository`), que seguem sendo usados onde há consultas além do CRUD mínimo (ex.: busca por SKU, por CPF/CNPJ, por status).

## Tratamento global de erros

`GlobalExceptionHandler` (API), implementando `IExceptionHandler`, converte exceções em `ProblemDetails` (`application/problem+json`):

| Exceção | Status HTTP |
|---|---|
| `ResourceNotFoundException` | 404 |
| `ConflictException` | 409 |
| `ArgumentException` | 400 |
| `DomainException` (demais) | 400 |
| Payload inválido (validação do `[ApiController]`) | 400 |
| Qualquer outra exceção | 500 (mensagem genérica fora de Development) |

## Health checks

`GET /health` é o único endpoint de health check, com relatório completo em JSON (status geral, duração total, `traceId` e a lista de checks nomeados). Status HTTP alinhado ao runtime: **Healthy/Degraded → 200**, **Unhealthy → 503**.

Checks registrados (`AddLojaTintasHealthChecks`, em `LojaTintas.API/Extensions/HealthCheckServiceExtensions.cs`):

| Check | O que verifica | Abordagem |
|---|---|---|
| `self` | Processo está no ar | `HealthCheckResult.Healthy` direto |
| `database` | Conexão com o SQLite via `LojaTintasDbContext` | `AddDbContextCheck<TContext>` (opção A do enunciado) |
| `fiap-website` | Dependência externa de exemplo (item recomendado) | `IHealthCheck` customizado (`FiapWebsiteHealthCheck`), faz um `GET` em `https://www.fiap.com.br` |

Exemplo de resposta **Healthy**:

```json
{
  "status": "Healthy",
  "totalDurationMs": 42.1,
  "traceId": "0HN...",
  "checks": [
    { "name": "self", "status": "Healthy", "durationMs": 0.01, "description": "API em execução." },
    { "name": "database", "status": "Healthy", "durationMs": 12.3, "description": null },
    { "name": "fiap-website", "status": "Healthy", "durationMs": 29.7, "description": "FIAP respondeu 200." }
  ]
}
```

**Para simular falha do banco (→ 503):** com a API rodando, pare-a, adicione um bloco `ConnectionStrings` com caminho inválido em `LojaTintas.API/appsettings.Development.json` (esse arquivo não tem `ConnectionStrings` hoje — a connection string vem do `appsettings.json`; adicionando aqui ela sobrepõe só em Development):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=/caminho/que/nao/existe/x.db"
  }
}
```

Suba a API de novo (`dotnet run`) — o check `database` vira `Unhealthy` e o `/health` responde **503**. Reverta o arquivo depois do teste (não commitar).

*Observação:* o check `fiap-website` depende de acesso real à internet da máquina onde a API roda. Se essa URL não responder, o `/health` inteiro fica `Unhealthy` (503) mesmo com banco e processo OK — comportamento esperado, um check `Unhealthy` derruba o status agregado.

## Observabilidade (logs)

- `GlobalExceptionHandler` loga toda exceção não tratada em nível **Error**, com `traceId` (`HttpContext.TraceIdentifier`) e o path/método da requisição. Em Development, o mesmo `traceId` também vai na resposta (`ProblemDetails.Extensions["traceId"]`) para facilitar correlacionar log ↔ resposta; em Production a resposta não expõe detalhe interno, só o log.
- `PedidosController.Criar` (fluxo de escrita) loga **início** (`ClienteId`, quantidade de itens, `traceId`) e **sucesso** (`PedidoId`, `NumeroPedido`, `ValorTotal`, `traceId`) com propriedades nomeadas (`ILogger` estruturado, sem concatenar string).

## Testes (xUnit)

```bash
dotnet test
```

Dois projetos de teste, na mesma solution:

- **`LojaTintas.Domain.Tests`** — referencia só o `Domain` (sem Infrastructure/API). Testa `Pedido.AdicionarItem`, uma regra de negócio real (invariantes de quantidade > 0 e desconto entre 0–100%, cálculo do valor total): `[Fact]` no caminho feliz + `[Theory]`/`[InlineData]` nos caminhos de erro.
- **`LojaTintas.Application.Tests`** — referencia o `Application`, mocka `IPedidoRepository`/`IClienteRepository`/`IProdutoRepository` com **Moq**. Testa `PedidoService.CriarPedidoAsync`: cliente ou produto inexistente → lança `ResourceNotFoundException` e **não** chama `AdicionarAsync` (`Times.Never`); caminho feliz → persiste uma vez (`Times.Once`).

## Diagrama MER

![Diagrama MER](docs/mer.png)
