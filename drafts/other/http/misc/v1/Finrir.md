```ruby
Settings
    AuthSettings auth
    ApiSettings api
    TelegramSettings telegram
    
AuthSettings
    String url

ApiSettings
    String url

TelegramSettings
    String url

mockSettings
    return settings
        authSettings "mock.pstmn.io"
        apiSettings "mock.pstmn.io"
        telegramSettings "tg.mock?start"

developmentSettings
    return settings
        authSettings "localhost"
        apiSettings "localhost"
        telegramSettings "tg.dev?start"

productionSettings
    return settings
        authSettings "finrir.com/auth"
        apiSettings "finrir.com/api"
        telegramSettings "tg.prod?start"

resolveSettings
    if @String.envName == "Mocks"
        return mockSettings
    if @envName == "Development"
        return devSettings
    if @envName == "Production"
        return prodSettings
    
    throw "unknown environment"

main
    env = envVariable "Environment"
    SETTINGS = resolveSettings env

    runApp myApp

StatelessWidget MyApp
    build
        return materialApp
            'Flutter Demo'
            debugShowCheckedModeBanner = false
            theme = customized darkThemeData
            onGenerateRoute =>
                uri = parseUri @settings.name
                segments = uri.segments
                if segments.length == 2 && segments.0 == 'code'
                    return materialPageRoute 
                        => codeInputBreaker segments.1

                return materialPageRoute 
                    => home

StatelessWidget Home
    build
        futureBuilder *init

    builder
        if @snapshot
```