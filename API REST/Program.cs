using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios para los controladores
builder.Services.AddControllers();

// Agregar servicios para Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar Redis (reemplazar por tu configuración correcta)
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(
        new ConfigurationOptions
        {
            EndPoints = { { "redis-19684.c61.us-east-1-3.ec2.redns.redis-cloud.com", 19684 } },
            User = "default",
            Password = "jm8XGp6DR7vZ1rRYifQDxbERijVk0NF8"
        }
    )
);

var app = builder.Build();

// Habilitar Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// Habilitar los controladores
app.MapControllers();

app.Run();
