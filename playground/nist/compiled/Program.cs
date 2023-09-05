var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddStateJsonConsole();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpIOLogging();

app.Run();

public record About(
    String description,
    String version,
    String environment
);