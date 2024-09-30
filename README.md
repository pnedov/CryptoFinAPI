## CrpyptoFin
This repository contains project that gets, agrregates and stores bitcoin prices for specific time point or time range using external public APIs.

## Tehnologies
 - .NET Core 8.0 / C#
 - SQLite
 - Entity Framework Core 8.0
 
## Swagger UI
For API UI has embedded Swagger UI 
![image](![image](https://github.com/user-attachments/assets/41226084-3834-489a-80d7-130e76b88955)

## Structure and design
- microservice, SOA
- layered architecture, repository pattern, dependency injection
  - code, data, UI are split
  - code consists of: controllers, models, core service logic and optional 3rd party libs
  - Entity Framework supports using SQLite, easy to create new db instance and customize the CRUD 
  - data stored by default in SQLite attached database [using Microsoft.Data.Sqlite.Core](https://www.nuget.org/packages/Microsoft.Data.Sqlite.Core/)
- RESTful API
- UI based on [Swagger UI](https://www.nuget.org/packages/swashbuckle.aspnetcore.swagger/) with minimal custom and support
- use of well-known 3rd party libraries: [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)
- code architecture allows easy extend options:
  - add new external API providers
  - new classes for calculate extracted prices
  - new data providers

This technology and architecture were chosen so that in the future it would be possible to easily еьтенд the functionality and integrate this project in a more complex system of microservices.
