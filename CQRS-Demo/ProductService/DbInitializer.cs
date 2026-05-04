using Microsoft.Data.SqlClient;

namespace ProductService;

public class DbInitializer(IConfiguration config)
{
    private readonly IConfiguration _config = config;

    public void Initialize()
    {
        var connectionString = _config.GetConnectionString("WriteDb");

        using var conn = new SqlConnection(connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();

        cmd.CommandText = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Products')
        CREATE TABLE Products (
            Id UNIQUEIDENTIFIER PRIMARY KEY,
            Name NVARCHAR(200),
            Price DECIMAL(18,2)
        );

        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='OutboxMessages')
        CREATE TABLE OutboxMessages (
            Id UNIQUEIDENTIFIER PRIMARY KEY,
            Type NVARCHAR(200),
            Content NVARCHAR(MAX),
            Processed BIT DEFAULT 0
        );
        ";

        cmd.ExecuteNonQuery();
    }
}
