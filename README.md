# LojaTintas — CP1 + CP2 + CP3

## Integrantes

| Nome | RM |
|------|-----|
| Andrei de Paiva Gibbini | 563061 |
| Diogo Cunha Abrão de Oliveira | 563654 |

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
- **API** — entry point, DI, controllers REST, DTOs, Swagger e GlobalExceptionHandler

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
| `GET` | `/health` | Health check |
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

## Diagrama MER

![Diagrama MER](docs/mer.png)
