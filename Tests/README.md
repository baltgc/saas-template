# Test Project

This project contains comprehensive tests for the SaaS Template API.

## Test Structure

- **Services/** - Unit tests for service layer
- **Repositories/** - Unit tests for repository layer
- **Controllers/** - Unit tests for controller layer
- **Integration/** - Integration tests for API endpoints
- **Helpers/** - Test helper utilities
- **Fixtures/** - Test data fixtures and builders

## Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Run specific test class
dotnet test --filter "FullyQualifiedName~UserServiceTests"

# Run tests in watch mode
dotnet watch test
```

## Test Coverage

The test suite covers:
- ✅ Service layer business logic
- ✅ Repository data access patterns
- ✅ Controller request/response handling
- ✅ Integration scenarios with in-memory database
- ✅ Error handling and edge cases
- ✅ Caching behavior
- ✅ Unit of Work pattern

## Test Dependencies

- **xUnit** - Test framework
- **Moq** - Mocking framework
- **FluentAssertions** - Assertion library
- **Microsoft.AspNetCore.Mvc.Testing** - Integration testing
- **Microsoft.EntityFrameworkCore.InMemory** - In-memory database for tests
- **Bogus** - Test data generation (optional)

