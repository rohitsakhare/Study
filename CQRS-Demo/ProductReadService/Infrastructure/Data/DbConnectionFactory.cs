using System.Data;
using Microsoft.Data.SqlClient;

namespace ProductReadService.Infrastructure.Data;

public class DbConnectionFactory(IConfiguration config)
{
    private readonly IConfiguration _config = config;

    public IDbConnection Create()
        => new SqlConnection(_config.GetConnectionString("DefaultConnection"));
}
