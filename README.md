# SaaS Template - .NET 10 API

A production-ready, scalable .NET 10 Web API template with comprehensive resilience patterns, testing, and best practices.

## 🚀 Features

### Core Architecture
- **Clean Architecture** - Separation of concerns with Controllers, Services, Repositories, and Data layers
- **Generic Repository Pattern** - Reusable data access layer
- **Unit of Work Pattern** - Transaction management and data consistency
- **Dependency Injection** - Fully configured with proper service lifetimes

### Resilience & Reliability
- **Polly Resilience Policies** - Retry, circuit breaker, and timeout policies for external calls
- **Health Checks** - Database, memory, and custom health checks with `/health`, `/health/ready`, `/health/live` endpoints
- **Rate Limiting** - Configurable rate limiting to protect API from abuse
- **Global Exception Handling** - Centralized error handling with proper HTTP status codes
- **Request/Response Logging** - Correlation IDs and structured request/response logging

### Performance & Scalability
- **Redis Caching** - Distributed caching with cache invalidation strategies
- **Entity Framework Core** - SQLite by default (easily switchable to SQL Server/PostgreSQL)
- **API Versioning** - Support for multiple API versions
- **Connection Pooling** - Optimized database connections

### Data Validation & Security
- **FluentValidation** - Strongly-typed validation for DTOs
- **CORS Configuration** - Configurable cross-origin resource sharing
- **HTTPS Redirection** - Secure by default

### Observability
- **Serilog** - Structured logging with console and file sinks
- **Correlation IDs** - Request tracking across services
- **Health Monitoring** - Multiple health check endpoints

### Testing
- **xUnit** - Comprehensive unit and integration tests
- **Moq** - Mocking framework for unit tests
- **FluentAssertions** - Readable test assertions
- **In-Memory Database** - Fast integration tests
- **Test Coverage** - Services, repositories, controllers, and integration scenarios

## 📁 Project Structure

```
saas-template/
├── Controllers/          # API Controllers
│   ├── BaseController.cs
│   └── UsersController.cs
├── Services/             # Business Logic Layer
│   ├── IUserService.cs
│   ├── UserService.cs
│   ├── ICacheService.cs
│   └── RedisCacheService.cs
├── Repositories/         # Data Access Layer
│   ├── IRepository.cs
│   ├── Repository.cs
│   ├── IUnitOfWork.cs
│   └── UnitOfWork.cs
├── Models/               # Data Models
│   ├── Entities/         # Domain entities
│   └── DTOs/            # Data Transfer Objects
├── Data/                 # Database Context
│   └── ApplicationDbContext.cs
├── Middleware/           # Custom Middleware
│   ├── ExceptionHandlingMiddleware.cs
│   └── RequestLoggingMiddleware.cs
├── Extensions/           # Service Extensions
│   ├── ServiceCollectionExtensions.cs
│   ├── HealthCheckExtensions.cs
│   ├── ResilienceExtensions.cs
│   ├── CachingExtensions.cs
│   ├── ValidationExtensions.cs
│   ├── RateLimitingExtensions.cs
│   ├── LoggingExtensions.cs
│   └── ConfigurationExtensions.cs
├── Common/               # Shared Utilities
│   ├── Exceptions/       # Custom exceptions
│   ├── Helpers/          # Helper classes
│   └── Resilience/       # Resilience policy names
├── Configuration/        # Configuration Models
│   └── AppSettings.cs
├── HealthChecks/         # Health Check Implementations
│   └── MemoryHealthCheck.cs
├── Validators/           # FluentValidation Validators
│   ├── CreateUserDtoValidator.cs
│   └── UpdateUserDtoValidator.cs
├── Tests/                # Test Project
│   ├── Services/         # Service unit tests
│   ├── Repositories/     # Repository unit tests
│   ├── Controllers/      # Controller unit tests
│   ├── Integration/      # Integration tests
│   ├── Helpers/          # Test helpers
│   └── Fixtures/         # Test data fixtures
├── Program.cs            # Application entry point
└── appsettings.json      # Configuration file
```

## 🛠️ Prerequisites

- .NET 10 SDK
- Redis (for caching - optional, can use in-memory cache)
- SQLite (included, or configure SQL Server/PostgreSQL)

## 📦 Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd saas-template
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Configure Redis** (optional)
   - Update `appsettings.json` with your Redis connection string
   - Default: `localhost:6379`

4. **Run database migrations**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

## ⚙️ Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=saas_template.db",
    "Redis": "localhost:6379"
  },
  "AppSettings": {
    "ApplicationName": "SaaS Template",
    "MaxRetryAttempts": 3,
    "RequestTimeoutSeconds": 30,
    "CacheExpirationMinutes": 5,
    "EnableSwagger": true
  },
  "RateLimiting": {
    "PermitLimit": 100,
    "WindowMinutes": 1
  }
}
```

### Environment Variables

- `ASPNETCORE_ENVIRONMENT` - Set to `Development`, `Staging`, or `Production`
- `ConnectionStrings__DefaultConnection` - Database connection string
- `ConnectionStrings__Redis` - Redis connection string

## 🧪 Testing

### Run All Tests
```bash
dotnet test
```

### Run Tests with Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Run Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName~UserServiceTests"
```

### Watch Mode (TDD)
```bash
dotnet watch test
```

### Test Structure
- **Unit Tests** - Services, Repositories, Controllers
- **Integration Tests** - Full HTTP API testing with in-memory database
- **Test Fixtures** - Reusable test data builders
- **Test Helpers** - Utility methods for test setup

## 📡 API Endpoints

### Users API (v1.0)

- `GET /api/v1/users` - Get all users
- `GET /api/v1/users/{id}` - Get user by ID
- `POST /api/v1/users` - Create new user
- `PUT /api/v1/users/{id}` - Update user
- `DELETE /api/v1/users/{id}` - Delete user

### Health Checks

- `GET /health` - Overall health status
- `GET /health/ready` - Readiness probe (database, memory)
- `GET /health/live` - Liveness probe

### Swagger UI

- `GET /swagger` - API documentation (Development only)

## 🔒 Security Features

- **HTTPS Redirection** - Enforced in production
- **Rate Limiting** - Protection against abuse
- **CORS** - Configurable cross-origin policies
- **Input Validation** - FluentValidation on all DTOs
- **Exception Handling** - No sensitive data leakage

## 📊 Monitoring & Logging

### Logging
- **Serilog** - Structured logging
- **Console Sink** - Development logging
- **File Sink** - Persistent logs in `logs/` directory
- **Correlation IDs** - Request tracking via `X-Correlation-ID` header

### Health Checks
- Database connectivity
- Memory usage
- Application status

## 🏗️ Architecture Patterns

### Repository Pattern
- Generic `IRepository<T>` interface
- Reusable data access methods
- Easy to mock for testing

### Unit of Work Pattern
- Transaction management
- Multiple repository coordination
- Consistent data operations

### Service Layer
- Business logic encapsulation
- Caching integration
- Validation orchestration

### Middleware Pipeline
1. Request Logging (correlation IDs)
2. Rate Limiting
3. HTTPS Redirection
4. CORS
5. Exception Handling

## 🔧 Extending the Template

### Adding a New Entity

1. **Create Entity**
   ```csharp
   // Models/Entities/Product.cs
   public class Product : BaseEntity
   {
       public string Name { get; set; }
       public decimal Price { get; set; }
   }
   ```

2. **Create DTOs**
   ```csharp
   // Models/DTOs/ProductDto.cs
   public class ProductDto { ... }
   public class CreateProductDto { ... }
   ```

3. **Create Service**
   ```csharp
   // Services/IProductService.cs
   public interface IProductService { ... }
   
   // Services/ProductService.cs
   public class ProductService : IProductService { ... }
   ```

4. **Create Controller**
   ```csharp
   // Controllers/ProductsController.cs
   [ApiController]
   [ApiVersion("1.0")]
   [Route("api/v{version:apiVersion}/[controller]")]
   public class ProductsController : ControllerBase { ... }
   ```

5. **Register Services**
   ```csharp
   // Extensions/ServiceCollectionExtensions.cs
   services.AddScoped<IProductService, ProductService>();
   ```

6. **Add to DbContext**
   ```csharp
   // Data/ApplicationDbContext.cs
   public DbSet<Product> Products { get; set; }
   ```

## 📚 Key Technologies

- **.NET 10** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM
- **SQLite** - Database (easily switchable)
- **Redis** - Distributed caching
- **Polly** - Resilience policies
- **Serilog** - Structured logging
- **FluentValidation** - Input validation
- **xUnit** - Testing framework
- **Moq** - Mocking framework
- **FluentAssertions** - Test assertions

## 🚦 Getting Started

1. **Run the application**
   ```bash
   dotnet run
   ```

2. **Access Swagger UI**
   - Navigate to `https://localhost:5001/swagger` (or your configured port)

3. **Test Health Endpoint**
   ```bash
   curl https://localhost:5001/health
   ```

4. **Create a User**
   ```bash
   curl -X POST https://localhost:5001/api/v1/users \
     -H "Content-Type: application/json" \
     -d '{"name":"John Doe","email":"john@example.com"}'
   ```

## 📝 Best Practices Implemented

- ✅ Dependency Injection
- ✅ Repository Pattern
- ✅ Unit of Work Pattern
- ✅ DTOs for data transfer
- ✅ Exception handling middleware
- ✅ Request/response logging
- ✅ Health checks
- ✅ Rate limiting
- ✅ Caching strategies
- ✅ Input validation
- ✅ API versioning
- ✅ Comprehensive testing
- ✅ Structured logging
- ✅ Configuration management

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new features
5. Ensure all tests pass
6. Submit a pull request

## 📄 License

This project is licensed under the MIT License.

## 🙏 Acknowledgments

Built with best practices from the .NET community and following Microsoft's recommended patterns for scalable API development.

