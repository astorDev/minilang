```python
compile
    yaml 'docker-compose

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
