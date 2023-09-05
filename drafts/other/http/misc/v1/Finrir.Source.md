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

mockSettings =>
    return settings
        authSettings "mock.pstmn.io"
        apiSettings "mock.pstmn.io"
        telegramSettings "tg.mock?start"

developmentSettings =>
    return settings
        authSettings "localhost"
        apiSettings "localhost"
        telegramSettings "tg.dev?start"

productionSettings =>
    return settings
        authSettings "finrir.com/auth"
        apiSettings "finrir.com/api"
        telegramSettings "tg.prod?start"

resolveSettings =>
    if @String.envName == "Mocks"
        return mockSettings
    if @envName == "Development"
        return devSettings
    if @envName == "Production"
        return prodSettings
    
    throw "unknown environment"
```


```ruby
env = envVariable "Environment"
SETTINGS = resolveSettings env

runApp myApp

init async =>
    env = envVariable 'Environment'
    initApp env
    await initAuth App.settings.auth
    initApi App.settings.auth
    if AUTH_TOKEN
        await Filtering.refresh

StatelessWidget MyApp
    build => 
        materialApp
            'Flutter Demo'      
            debugShowCheckedModeBanner = false
            theme = customized darkThemeData
            onGenerateRoute = =>
                segments = parseUri @settings.name -> .pathSegments
                if segments.length == 2 && segments.0 == 'code'
                    return materialPageRoute 
                        => codeInputBreaker segments.1

                return materialPageRoute 
                    => home

StatelessWidget Home
    build => 
        futureBuilder
        future = *init
        builder = =>
            if @snapshot.connectionState isnt done
                return spinnerPage
            if no AUTH_TOKEN
                return unauthenticatedPage
            
            return homeGate
```

```ruby
StatelessWidget UnexpectedErrorPage
    List_Widget extraText
    List_Widget actions

    build =>
        return centeredColumnPage
            padding = HorizontalMargins.toEdgeInsets
            [
                emoji '😳'
                blocksSpace
                instructionLine 'Something strange happened on our side.'
                instructionLine 'We will investigate and fix that'
                instructionLine 'Meanshile, please try again'
                ...extraText
                blocksSpace
                ...actions
            ]

StatelessWidget Emoji
    STYLE = textStyle fontSize:60 color:black
    
    String symbol


    build =>
        return text symbol STYLE

StatelessWidget BlocksSpace
    MARGIN = edgeInsetWithOnly bottom:20

    build =>
        return container margin:MARGIN

StatelessWidget CommandInstructionLine
    String line
    
    build => 
        theme = themeOf @context
        baseStyle = theme.bodyLarge
        style = baseStyle.copyWith
            decoration = TextDecoration.underline
            fontWeight = FontWeight.w900
            color = theme.colorScheme.primary

        return text line style
```

# Version 2

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
        > "mock.pstmn.io"
        > "mock.pstmn.io"
        > "tg.mock?start"

developmentSettings
    return settings
        > "localhost"
        > "localhost"
        > "tg.dev?start"

productionSettings
    return settings
        > "finrir.com/auth"
        > "finrir.com/api"
        > "tg.prod?start"

resolveSettings
    envName = @String

    if @envName == "Mocks" return mockSettings
    if @envName == "Development" return devSettings
    if @envName == "Production" return prodSettings
    
    throw "unknown environment"
```


```ruby
runApp myApp

init async =>
    env = envVariable 'Environment'
    SETTINGS = resolveSettings env
    await initAuth SETTINGS.auth
    initApi SETTINGS.api
    if AUTH_TOKEN
        await refreshFilters

StatelessWidget MyApp
    build
        return materialApp
            'Flutter Demo'      
            debugShowCheckedModeBanner  = false
            theme = customized darkThemeData
            onGenerateRoute =>
                parseUri @settings.name
                .pathSegments
                if .length == 2 && .0 == 'code'
                    return materialPageRoute => codeInputBreaker .1

                return materialPageRoute => home

StatelessWidget Home
    build => 
        return futureBuilder
            future = *init
            builder =>
                if @snapshot.connectionState isnt done return spinnerPage
                if no AUTH_TOKEN return unauthenticatedPage
                return homeGate
```

```ruby
StatelessWidget UnexpectedErrorPage
    List_Widget extraText
    List_Widget actions

    build
        return centeredColumnPage
            padding = HorizontalMargins.toEdgeInsets
            [
                emoji '😳'
                blocksSpace
                instructionLine 'Something strange happened on our side.'
                instructionLine 'We will investigate and fix that'
                instructionLine 'Meanshile, please try again'
                ...extraText
                blocksSpace
                ...actions
            ]

StatelessWidget Emoji
    STYLE = textStyle fontSize:60 color:black
    
    String symbol

    build => text symbol STYLE

StatelessWidget BlocksSpace
    MARGIN = edgeInsetWithOnly bottom:20

    build =>
        return container margin:MARGIN

StatelessWidget CommandInstructionLine
    String line
    
    build => 
        theme = themeOf @context
        baseStyle = theme.bodyLarge
        style = baseStyle.copyWith
            decoration = TextDecoration.underline
            fontWeight = FontWeight.w900
            color = theme.colorScheme.primary

        return text line style
```