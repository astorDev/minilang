```python
host csproj:'host/Template.Host
    web
        packages =
            'Astor.Logging:2.0.1
            'Nist.Logs:1.0.0
            'Swashbuckle.AspNetCore:7.0.0
        usings =
            'Template
            'Microsoft.AspNetCore.Mvc
            'Nist.Errors
            'System.Net
            'Nist.Logs
            'Astor.Logging
            'System.Reflection
        references =
            '..\protocol\Template.Protocol

go csharpProgram:'host
    f = frame @args =>
        @.services.addEndpointsApiExplorer
        @.services.addSwaggerGen

        @.setNistLogging
    
    p = pipe f =>
        @.useSwagger
        @.useSwaggerUI

        @.useHttpIOLogging
        @.useErrorBody => on @ex
            default UNKNOWN_ERROR

    a = app p =>
        @.mapGet URIS_ABOUT =>
            about
                description = 'Template
                version = getEntryAssemblyVersion
                environment = @IHostEnvironment...environmentName

    a.run

dockerfile dotnetDockerfile
    use 'host 'Template.Host.csproj
    use 'protocol 'Template.Protocol.csproj
    workdir 'host

local dotnetLaunchsettings:'host
    port '9988

URIS csharp:'protocol/About namespace:'Template
    ABOUT = about

IClient csharp:'protocol/About namespace:'Template
    getAbout Task_about

Client IClient csharp:'protocol/About namespace:'Template
    getAbout
        await =>
            http.get_About URIS_ABOUT

About csharp:'protocol/About
    description string
    version string
    environment string

URIS csharp:'protocol/Protocol
IClient csharp:'protocol/Protocol
Client IClient csharp:'protocol/Protocol
    HttpClient http

ERRORS
    UNKNOWN = error HTTP_CODE_INTERNAL_SERVER_ERROR 'Unknown

protocol csproj:'host/Template.Protocol
    lib
        packages =
            'Nist.Errors:2.0.1
            'Nist.Responses:1.0.0
            'System.Net.Http.Json:7.0.0
        usings =
            'Nist.Responses
            'System.Net
            'Nist.Errors
            'System.Net.Http.Json

gitignore dotnetGitignore

localHttpEnv httpYacEnv:'local
    template 'http://localhost:9988'

getAboutHttp httpCall:'.
    ''@template'/about

hostService dockerService:'.
    image template
    labels family:nist
    build 
        '.
        'host/Dockerfile
    map
        80 = 9998
```