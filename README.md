# AutoMapper with EF Core Projection Demo

This project demonstrates the power of **AutoMapper Projection** (`ProjectTo`) when working with **Entity Framework Core**. It is designed as a learning resource to show the difference between various data fetching strategies and their impact on performance and code cleanliness.

## Key Concepts

### 1. Why Projection Matters?
By default, EF Core fetches all columns of a table when you query an entity. If your table has 50 columns but your UI only needs 3, you are **overfetching**. Overfetching wastes database memory, network bandwidth, and application memory.

### 2. Include + Map (The Naive Approach)
- **Strategy**: Use `.Include()` to load navigation properties, fetch all entities into memory, then use `_mapper.Map<T>(entities)`.
- **Problem**: It fetches every single column from the main table and all included tables.
- **SQL Example**: `SELECT o.Id, o.Date, c.Id, c.Name, c.Email, ... FROM Orders ... JOIN Customers ...`

### 3. Manual Projection (.Select)
- **Strategy**: Use `.Select(o => new Dto { ... })` to manually pick columns.
- **Benefit**: Optimized SQL. Only the required columns are fetched.
- **Problem**: Hard to maintain. If you add a field to the DTO, you must update every `.Select()` in your codebase.

### 4. AutoMapper ProjectTo (The Best Practice)
- **Strategy**: Use `.ProjectTo<Dto>(_mapper.ConfigurationProvider)` directly on the `IQueryable`.
- **Benefit**: Combines the optimized SQL of manual projection with the maintainability of AutoMapper. EF Core translates the AutoMapper configuration directly into a SQL `SELECT` statement.

## Project Structure

- **Data/Entities**: Domain models (Customer, Order, Product, Category, OrderItem).
- **DTOs**: Data Transfer Objects for API responses.
- **Profiles**: AutoMapper configurations defining how Entities map to DTOs.
- **Controllers**: Endpoints demonstrating the 3 approaches:
    - `GET /api/orders/naive`
    - `GET /api/orders/manual-projection`
    - `GET /api/orders/projectto`

## How to Run

1.  **Prerequisites**: 
    - .NET 8 SDK
    - SQL Server (LocalDB `(localdb)\mssqllocaldb` is used by default in `appsettings.json`)
2.  **Steps**:
    - `dotnet restore`
    - `dotnet run`
3.  **Swagger**: Open `https://localhost:<port>/swagger` to interact with the API.

## Performance Comparison (Check the Logs!)

The application logs the generated SQL for each endpoint. 
Observe the difference:
- **Naive**: Huge `SELECT` with all columns.
- **ProjectTo**: Surgical `SELECT` with only DTO-required columns.

## Common Pitfalls
- **Using .Map() instead of .ProjectTo()**: .Map() happens in memory. Always use .ProjectTo() for read-only queries.
- **Forgetting .AsNoTracking()**: For read-only projection, tracking is unnecessary overhead.
- **Complex logic in Profiles**: EF Core can only translate certain expressions to SQL. Keep your mappings "SQL-friendly" or use `ValueConverters`.

---
*Created for educational purposes to help developers master EF Core Performance.*
# EfCoreProjectionDemo
