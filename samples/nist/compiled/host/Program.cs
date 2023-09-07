var builder = WebApplication.CreateBuilder(args);;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration["Logging:LogLevel:Default"] = "Warning";
builder.Configuration["Logging:LogLevel:Microsoft.AspNetCore.Diagnostic.ExceptionHandlingMiddleware"] = "None";
builder.Configuration["Logging:LogLevel:Microsoft.Hosting.Lifetime"] = "Information";
builder.Configuration["Logging:StateJsonConsole:LogLevel:Default"] = "None";
builder.Configuration["Logging:StateJsonConsole:LogLevel:Nist.Logs"] = "Information";

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole((c) => c.SingleLine = true);
builder.Logging.AddStateJsonConsole();

var app = builder.Build();;

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpIOLogging();

app.MapGet("/about", (IHostEnvironment env) => new About("Template Host", "v1", env.EnvironmentName));
app.Run();

