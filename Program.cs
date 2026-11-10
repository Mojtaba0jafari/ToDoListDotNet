using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using ToDoList.Api;
using ToDoList.Api.Data;
using ToDoList.Api.Services;

const string FrontendCorsPolicy = "Frontend";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    options.JsonSerializerOptions.UnmappedMemberHandling =
        JsonUnmappedMemberHandling.Disallow);
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "ToDoList.Api v1");
    });
}

app.UseHttpsRedirection();

app.UseCors(FrontendCorsPolicy);
app.MapControllers();

app.Run();

public partial class Program
{
}