using Testcontainers.PostgreSql;

namespace CustomerService.Tests;
public sealed class CustomerServiceTest : IAsyncLifetime
{
    /// <summary>
    /// Postgres container for the integration test.
    /// The container will be automatically created and started before the test.
    /// </summary>
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .Build();

    public Task InitializeAsync()
    {
        return _postgres.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }

    /// <summary>
    /// Test case for creating new customers in the database, while also check whether
    /// the customers are successfully created.
    /// </summary>
    [Fact]
    public void ShouldReturnTwoCustomers()
    {
        // Given
        var customerService = new CustomerService(new DbConnectionProvider(_postgres.GetConnectionString()));

        // When
        customerService.Create(new Customer(1, "George"));
        customerService.Create(new Customer(2, "John"));
        var customers = customerService.GetCustomers();

        // Then
        Assert.Equal(2, customers.Count());
    }
}
