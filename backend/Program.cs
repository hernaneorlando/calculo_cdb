using FluentValidation;
using Microsoft.OpenApi.Models;
using WebApi.Infraestrutura;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Calculo CDB", Version = "v1" });
});

builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Transient);
builder.Services.AddExceptionHandler<ErrorHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Calculo CDB v1"));
app.UseExceptionHandler();

app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyHeader();
});

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
