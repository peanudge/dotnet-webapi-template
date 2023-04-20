# DOT NET Playground

> Reference
> https://learn.microsoft.com/ko-kr/aspnet/core/tutorials/first-web-api?view=aspnetcore-7.0&tabs=visual-studio-code


## Initalize Database 

Create Northwind.db file

```bash
$ sqlite3 Northwind.db < ./data/NorthwindSeed.sql
```


# EF Core data provider

EF Core data providers are sets of classes that are optimized for a specific data store.

> dotnet add package Microsoft.EntityFrameworkCore.Sqlite
