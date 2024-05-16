
namespace CustomerService;

public readonly record struct Customer(long Id, string Name);

public class CustomerService
{
    private readonly DbConnectionProvider _dbConnectionProvider;

    /// <summary>
    /// CustomerService constructor for initializing the database schema.
    /// </summary>
    /// <param name="dbConnectionProvider"></param>
    public CustomerService(DbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
        CreateCustomersTable();
    }

    /// <summary>
    /// Get all customers from the database.
    /// This method will be tested in the unit / integration test.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Customer> GetCustomers()
    {
        IList<Customer> customers = [];

        using var connection = _dbConnectionProvider.GetConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, name FROM customers";
        command.Connection?.Open();

        using var dataReader = command.ExecuteReader();
        while (dataReader.Read())
        {
            var id = dataReader.GetInt64(0);
            var name = dataReader.GetString(1);
            customers.Add(new Customer(id, name));
        }

        return customers;
    }

    /// <summary>
    /// Create a new customer in the database.
    /// This method will be tested in the unit / integration test.
    /// </summary>
    /// <param name="customer"></param>
    public void Create(Customer customer)
    {
        using var connection = _dbConnectionProvider.GetConnection();
        using var command = connection.CreateCommand();

        var id = command.CreateParameter();
        id.ParameterName = "@id";
        id.Value = customer.Id;

        var name = command.CreateParameter();
        name.ParameterName = "@name";
        name.Value = customer.Name;

        command.CommandText = @"INSERT INTO customers(id, name) 
        VALUES(@id, @name)";
        command.Parameters.Add(id);
        command.Parameters.Add(name);
        command.Connection?.Open();
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Initialize the database schema in the database by creating the customers table.
    /// </summary>
    private void CreateCustomersTable()
    {
        using var connection = _dbConnectionProvider.GetConnection();
        using var command = connection.CreateCommand();

        // We could also load this from an .sql file, especially when the schema definitions are large.
        command.CommandText = @"CREATE TABLE IF NOT EXISTS 
        customers (id BIGINT NOT NULL, name VARCHAR NOT NULL, PRIMARY KEY(id))";
        command.Connection?.Open();
        command.ExecuteNonQuery();
    }
}
