# Case
Ecommerce project case built with
- .NET 8
- Azure functions for integration
- Entity Framework core

API Documentation with Swagger
API endpoints:
- GET /products/{id} - Get a single product by id
- POST /products - Get all products with possible category filter and pagination

How to run locally:
- .NET 8 SDK
- IDE (VS or VS code)
- SQL server
- Add the connectionstring to the database in the appsettings.Development.json and local.setting.json or secrets for the different projects
- Run migration
