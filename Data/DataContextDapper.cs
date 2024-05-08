using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotnetApi.Data {
    class DataContextDapper
    {
        private readonly IConfiguration _config;
        public DataContextDapper(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<T> LoadData<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Query<T>(sql);
        }
        public T LoadDataSingle<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.QuerySingle<T>(sql);
        }
        public bool ExecuteSQL(string sql, object parameters = null)
{
            using (IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                int result = dbConnection.Execute(sql, parameters); // Make sure parameters are passed here
                return result > 0;
            }
}
        public int ExecuteSQLWithRowCount(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql);
        }
        public bool ExecuteSQLWithParameters(string sql, List<SqlParameter> parameters)
        {
            // Create a new connection using the connection string from configuration
            using (SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                // Create a command object with the SQL query and bind the connection
                using (SqlCommand commandWithParams = new SqlCommand(sql, dbConnection))
                {
                    // Add each parameter to the command object
                    foreach (SqlParameter parameter in parameters)
                    {
                        commandWithParams.Parameters.Add(parameter);
                    }

                    // Open the database connection
                    dbConnection.Open();

                    // Execute the command and store the number of rows affected
                    int rowsAffected = commandWithParams.ExecuteNonQuery();

                    // Close the database connection (though 'using' statement handles it automatically)
                    dbConnection.Close();

                    // Return true if the operation affected at least one row
                    return rowsAffected > 0;
                }
            }
        }
        
    }
}