using System.Data.Common;
using Npgsql;

namespace CustomerService;

public class DbConnectionProvider(string connectionString)
{
    private readonly string _connectionString = connectionString;

    public DbConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
