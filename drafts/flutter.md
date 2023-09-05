```python
compile
    flutter

main
    runApp myapp

myapp widget
    materialApp
        title = 'Flutter_Demo
        debugShowCheckedModeBanner = false
        theme = theme
        home = home
        onGenerateRoute =>
            uri = parse @settings.name
            return when
                    both
                        equal pathSegments.length 2
                        equal pathSegments.0 'code
                    then =>
                        code = pathSegments.1
                        return materialPageRoute
                            builder = codeInputBreaker code
                    otherwise => 
                        return materialPageRoute
                            builder = home    
                
home widget
    futureBuilder
        future = init
        builder =>
            return when
                    notEqual connectionState CONNECTION_DONE
                    then =>
                        return when
                                equal Auth.tokens null
                                then => return unauthenticatedPage
                                otherwise => return homeGate
                    otherwise => return spinnerPage

init
    env = envVariable 'Environment
    APP_SETTINGS = resolveSettings env

    await 
        initAuth APP_SETTINGS

    API = initApi APP_SETTINGS
    if AUTH_TOKEN
        await refreshFilters

Settings
    AuthSettings auth
    ApiSettings api

    mock
        authSettings localhost
        apiSettings localhost
    
    dev
        authSettings finrir.com/auth
        apiSettings finrir.com/api
        telegramDeeplink tg.dev?start

    prod
        authSettings finrir.com/auth
        apiSettings finrir.com/api
        telegramDeeplink tg.prod?start

    resolveSettings
        return on envName
            pair 'Mocks' mock
            pair 'Dev' dev
            pair 'Prod' prod
```