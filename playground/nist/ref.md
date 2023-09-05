```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <DockerDefaultTargetOS>Linux</DockerDefaultTargetOS>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Astor.Logging" Version="2.1.0" />
    <PackageReference Include="Nist.Logs" Version="2.0.1" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\protocol\Template.Protocol.csproj" />
  </ItemGroup>

  <ItemGroup>
      <Using Include="Template" />
      <Using Include="Microsoft.AspNetCore.Mvc" />
      <Using Include="Nist.Errors" />
      <Using Include="System.Net" />
      <Using Include="Nist.Logs" />
      <Using Include="Astor.Logging" />
      <Using Include="System.Reflection" />
  </ItemGroup>

</Project>
```

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <RootNamespace>Template</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Nist.Errors" Version="2.0.1" />
    <PackageReference Include="Nist.Responses" Version="1.0.0" />
    <PackageReference Include="System.Net.Http.Json" Version="7.0.0" />
  </ItemGroup>

  <ItemGroup>
      <Using Include="Nist.Responses" />
      <Using Include="System.Net" />
      <Using Include="Nist.Errors" />
      <Using Include="System.Net.Http.Json" />
  </ItemGroup>
  
</Project>
```


```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration["Logging:LogLevel:Default"] = "Warning";
builder.Configuration["Logging:LogLevel:Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware"] = "None";
builder.Configuration["Logging:LogLevel:Microsoft.Hosting.Lifetime"] = "Information";
builder.Configuration["Logging:StateJsonConsole:LogLevel:Default"] = "None";
builder.Configuration["Logging:StateJsonConsole:LogLevel:Nist.Logs"] = "Information";

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(c => c.SingleLine = true);
builder.Logging.AddStateJsonConsole();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpIOLogging();
app.UseErrorBody(ex => ex switch {
    _ => Errors.Unknown
});

app.MapGet($"/{Uris.About}", (IHostEnvironment env) => new About(
    Description: "Template",
    Version: Assembly.GetEntryAssembly()!.GetName().Version!.ToString(),
    Environment: env.EnvironmentName
));

app.Run();

public class Uris{
    public const string About = "about";
}

public interface IClient {
    Task<About> GetAbout();
}

public class Client : IClient {
    public Task<About> GetAbout() => this.Http.GetAsync(Uris.About).Read<About>();
}

public record About(string Description, string Version, string Environment);
```