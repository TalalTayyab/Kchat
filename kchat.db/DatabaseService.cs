using Dapper;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace kchat.db
{
    public class DatabaseService
    {
        private readonly string connectionstring;

        public DatabaseService(string connectionstring)
        {
            this.connectionstring = connectionstring;
        }

        public async Task<int> ExecuteSqlAsync(string sql, object param)
        {
            using var connection = new SqlConnection(connectionstring);
            return await connection.ExecuteAsync(sql, param);
        }
    }
}
