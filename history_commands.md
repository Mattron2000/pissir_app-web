# History

## Generate .gitignore for dotnet projects

```sh
dotnet new gitignore
```

## Generate ASP.NET Core (Backend) + Blazor WebAssembly (Frontend) projects into Server folder

```sh
dotnet new blazor -int Auto -o Server
```

Done numerous changes from "Server" & "Server.Client" project to "Backend" & "Frontend" projects changing names and modified example pages for tinkering

Created LoadingSpinner Component with Name parameter

## Add OpenApi page to Backend

```sh
dotnet add Server/Backend package Microsoft.AspNetCore.OpenApi -v 8.0.15

dotnet add Server/Backend package Swashbuckle.AspNetCore -v 8.1.1
```

## Add DB communication capacity to Backend

```sh
dotnet tool install --global dotnet-ef --version 9.0.4

dotnet add Server/Backend package Microsoft.EntityFrameworkCore.Design --version 9.0.4

dotnet add Server/Backend package Microsoft.EntityFrameworkCore.Tools --version 9.0.4

dotnet add Server/Backend package Microsoft.EntityFrameworkCore.Sqlite --version 9.0.4
```

## Entity Framework Scaffolding

```sh
mkdir './Server/Backend/DB'

# BEWARE! "SmartParking.schema" CONTAINS 'DROP TABLE IF EXISTS' queries
sqlite3 './Server/Backend/DB/SmartParking.db' < './Server/Backend/SmartParking.schema'

dotnet ef --project Server/Backend dbcontext scaffold Name=SmartParking Microsoft.EntityFrameworkCore.Sqlite -o Models
```

## Make DB Migrations

```sh
dotnet ef --project Server/Backend migrations add MIGRATION_NAME

dotnet ef --project Server/Backend migrations list

dotnet ef --project Server/Backend migrations remove

dotnet ef --project Server/Backend migrations script

dotnet ef --project Server/Backend database update
```

## Create new Project to contain shared contents between Backend and Frontend

```sh
dotnet new classlib -o Server/Shared

dotnet sln Server add Server/Shared

dotnet add Server/Backend reference Server/Shared

dotnet add Server/Frontend reference Server/Shared
```

## Fluent Validation

```sh
dotnet add Server/Shared package FluentValidation --version 12.0.0
```
