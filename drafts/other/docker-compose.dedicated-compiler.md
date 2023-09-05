```python
compile
    docker

main
    dockerCompose
        mssql password:'yourStrong(!)Password' on:1433
        redis on:6379

mssql
    return dockerService
        name = @name/'mssql'
        image = 'mcr.microsoft.com/azure-sql-edge'
        environment = map 'acceptEula' 'Y' 'saPassword' '@password'
        ports = map 1433 @on
        volume = /var/opt/mssql

redis
    return dockerService
        name = @name/'redis'
        image = 'redis'
        ports = map 6379 @on
```

global

```python
compile
    yaml 'docker-compose
    use 'docker

go
    version '3.4

    volumes
        ibissql
    
    services
        mssql
            restart always
            image mcr.microsoft.com/azure-sql-edge
            environment
                ACCEPT_EULA = Y
                SA_PASSWORD = yourStrong(!)Password
            ports
                1433:1433
            volumes
                ibissql:/var/opt/mssql
        
        redis
            restart always
            image redis
            ports
                6379:6379
```


