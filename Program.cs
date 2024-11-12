using Microsoft.AspNetCore.Mvc;
using Serilog;

var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
        .Build();

var log = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
        options.AddPolicy(name: "all",
                policy  =>
                {
                        policy.AllowAnyOrigin();
                        policy.AllowAnyHeader();
                        policy.AllowAnyMethod();
                });
});

var app = builder.Build();

app.UseCors("all");

app.MapGet("/", () => "Hello World!");

app.MapGet("/health", () => "I'm up");

string CurrentColor = "";

app.MapGet("/color", () => CurrentColor);

app.MapPost("/color", ([FromBody] string color) => {
        log.Information($"Changed color to {color}");
        CurrentColor = color;
});

app.UseSwagger();
app.UseSwaggerUI();

log.Information("Starting app");

app.Run();