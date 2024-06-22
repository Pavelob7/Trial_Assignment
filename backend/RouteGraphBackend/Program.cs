using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using RouteGraphBackend.Data;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:5000");

// ��������� ��������� Swagger
builder.Services.AddEndpointsApiExplorer(); // ��������� ��������� ��������� ������������ OpenAPI
builder.Services.AddSwaggerGen(); // ��������� ��������� Swagger ���������

// ��������� ������� � �����������
builder.Services.AddDbContext<RouteGraphBackend.Data.RouteContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); // ��������� �������� ���� ������ � �������������� PostgreSQL

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; // ���������� ����� � �������� ��� ������������ JSON
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // ���������� camelCase ��� ���������� ������� JSON
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull; // ���������� �������� �� ��������� null ��� ������������
    });

builder.Services.AddLogging(); // ��������� �����������

var app = builder.Build();

// ��������� Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // �������� ��������� Swagger JSON
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); // ��������� ���� � Swagger JSON � �������� API
    });
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // ��������� ������������� ������������
});

app.Run(); // ��������� ����������
