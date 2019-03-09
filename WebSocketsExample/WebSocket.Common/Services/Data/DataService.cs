using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebSocketCommon.Services.Data
{
    public class DataService : IDataService
    {
        ILogger _Logger { get; set; }
        string _ConnectionString { get; set; }

        public DataService(IServiceProvider serviceProvider)
        {
            _Logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<DataService>();
        }

        protected DataService(IServiceProvider serviceProvider, string connectionString)
        {
            _Logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<DataService>();
            SetConnectionString(connectionString);
        }

        #region Interface Members
        public void SetConnectionString(string connectionString)
        {
            _ConnectionString = connectionString;
        }

        public DbCommand CreateCommand(string commandText, CommandType type = CommandType.Text)
        {
            return new SqlCommand()
            {
                CommandText = commandText,
                CommandType = type,
                CommandTimeout = 60
            };
        }

        public async Task ExecuteNonQueryAsync(DbCommand dbCommand)
        {
            var dbConnection = new SqlConnection(_ConnectionString);
            try
            {
                dbCommand.Connection = dbConnection;
                await dbCommand.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                _Logger.LogError($"Sql Execution Failed: {e.Message}");
            }
            finally
            {
                dbCommand.Dispose();
                dbConnection.Dispose();
            }
        }

        public async Task<List<object[]>> ExecuteReaderAsync(DbCommand dbCommand)
        {
            var dbConnection = new SqlConnection(_ConnectionString);
            List<object[]> tmp = new List<object[]>();
            try
            {
                dbCommand.Connection = dbConnection;
                dbConnection.Open();
                DbDataReader dbDataReader = dbDataReader = await dbCommand.ExecuteReaderAsync();
                tmp = UnwrapValues(dbDataReader);
            }
            catch (Exception e)
            {
                _Logger.LogError($"Sql Execution Failed: {e.Message}");
            }
            finally
            {
                dbCommand.Dispose();
                dbConnection.Dispose();
            }
            return tmp;
        }
        #endregion Interface Members

        private List<object[]> UnwrapValues(DbDataReader reader)
        {
            List<object[]> rows = new List<object[]>();
            while(reader.Read())
            {
                object[] buffer = new object[reader.FieldCount];
                reader.GetValues(buffer);
                rows.Add(buffer);
            }
            return rows;
        }
    }
}
