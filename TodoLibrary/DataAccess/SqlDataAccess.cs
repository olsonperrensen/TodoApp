using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace TodoLibrary.DataAccess;

public class SqlDataAccess : ISqlDataAccess
{
    private readonly IConfiguration _config;

    public SqlDataAccess(IConfiguration config)
    {
        _config = config;
    }

    public async Task<List<T>> LoadData<T, U>(string storedProcedure,
       U parameteres,
       string connectionStringName)
    {
        string connectionString = _config.GetConnectionString(connectionStringName)!;
        using IDbConnection c = new SqlConnection(connectionString);
        var rows = await c.QueryAsync<T>(storedProcedure, parameteres,
            commandType: CommandType.StoredProcedure);
        return rows.ToList();
    }

    public async Task SaveData<T>(string storedProcedure,
               T parameters,
                      string connectionStringName)
    {
        string connectionString = _config.GetConnectionString(connectionStringName)!;
        using IDbConnection c = new SqlConnection(connectionString);
        await c.ExecuteAsync(storedProcedure, parameters,
                       commandType: CommandType.StoredProcedure);
    }
}
