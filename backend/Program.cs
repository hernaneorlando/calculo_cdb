using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using WebApi.Infraestrutura;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<ISqlDataContext, SqlDataContext>();
builder.Services.AddDbContext<SqlDataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnectionString"));
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Calculo CDB", Version = "v1" });
});

builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Transient);
builder.Services.AddExceptionHandler<ErrorHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks().AddCheck("healthy", () => HealthCheckResult.Healthy());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Calculo CDB v1"));
app.UseExceptionHandler();

app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyHeader();
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Essa garantia da criação do banco deverá ser executada somente em ambiente de desenvolvimento.
// Em produção, esse código deverá ser colocado no bloco acima.
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<SqlDataContext>();
if (dbContext.Database.EnsureCreated())
{
    var seedInicial = File.ReadAllText("InitialDataSeed.sql");
    dbContext.Database.ExecuteSqlRaw(seedInicial);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Liveness check
app.UseHealthChecks("/healthz", new HealthCheckOptions { Predicate = r => r.Name == "healthy" });

app.Run();
