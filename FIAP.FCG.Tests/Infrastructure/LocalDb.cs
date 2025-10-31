using Microsoft.Data.SqlClient;

namespace FIAP.FCG.Tests.Infrastructure;
public static class LocalDb
{
    private const string Master = "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;TrustServerCertificate=True;";

    public static string CreateDatabase(out string dbName)
    {
        dbName = $"fcg_tests_{Guid.NewGuid():N}";
        using var cn = new SqlConnection(Master);
        cn.Open();
        using var cmd = cn.CreateCommand();
        cmd.CommandText = $"CREATE DATABASE [{dbName}]";
        cmd.ExecuteNonQuery();
        return BuildConnectionString(dbName);
    }

    public static string BuildConnectionString(string dbName) =>
        $"{Master}Database={dbName};";

    public static void DropDatabase(string dbName)
    {
        using var cn = new SqlConnection(Master);
        cn.Open();
        using var cmd = cn.CreateCommand();
        cmd.CommandText = $@"
IF DB_ID('{dbName}') IS NOT NULL
BEGIN
  ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
  DROP DATABASE [{dbName}];
END";
        cmd.ExecuteNonQuery();
    }
}
