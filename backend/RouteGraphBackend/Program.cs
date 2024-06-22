using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using RouteGraphBackend.Data;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:5000");

// Добавляем поддержку Swagger
builder.Services.AddEndpointsApiExplorer(); // Добавляем поддержку генерации спецификаций OpenAPI
builder.Services.AddSwaggerGen(); // Добавляем поддержку Swagger генерации

// Добавляем сервисы и контроллеры
builder.Services.AddDbContext<RouteGraphBackend.Data.RouteContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); // Добавляем контекст базы данных с использованием PostgreSQL

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; // Игнорируем циклы в объектах при сериализации JSON
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // Используем camelCase для именования свойств JSON
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull; // Игнорируем свойства со значением null при сериализации
    });

builder.Services.AddLogging(); // Добавляем логирование

var app = builder.Build();

// Настройка Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Включаем генерацию Swagger JSON
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); // Указываем путь к Swagger JSON и название API
    });
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Разрешаем использование контроллеров
});

app.Run(); // Запускаем приложение
