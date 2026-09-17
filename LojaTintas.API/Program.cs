using LojaTintas.API.Exceptions;
using LojaTintas.API.Extensions;
using LojaTintas.Application.Interfaces.Repositories;
using LojaTintas.Application.Services;
using LojaTintas.Infrastructure.Data;
using LojaTintas.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LojaTintasDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositórios específicos por agregado (CP2) — usados quando a entidade exige
// consultas além do CRUD mínimo (ex.: busca por SKU, por CPF/CNPJ, por status).
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IEstoqueRepository, EstoqueRepository>();

// Repositório genérico (CP3) — registrado como serviço aberto, resolve
// IRepository<Categoria>, IRepository<Fabricante>, etc.
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Serviços de aplicação
builder.Services.AddScoped<IPedidoService, PedidoService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddLojaTintasSwagger();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Registrado antes de MapControllers e do Swagger, conforme exigido pelo CP3.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "LojaTintas API v1");
    });

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<LojaTintasDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck")
   .WithTags("Health");

app.Run();
