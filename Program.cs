using Drive.Models;
using Microsoft.EntityFrameworkCore;
//using Pomelo.EntityFrameworkCore.MySql;
using Scalar.AspNetCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// 1. Definir la cadena de conexión (mejor si está aquí o en appsettings.json)
var connString = "Host=localhost;Username=postgres;Password=PUTA0011;Database=users";

// 2. Configurar el DbContext correctamente
builder.Services.AddDbContextPool<DefaultDbContext>(opt =>
    opt.UseNpgsql(connString) // Usamos la variable directa para evitar el error de 'null'
);

// 3. Servicios estándar
builder.Services.AddOpenApi();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

var app = builder.Build();

// 4. Pipeline de HTTP
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();

app.UseHttpsRedirection();

app.Run();