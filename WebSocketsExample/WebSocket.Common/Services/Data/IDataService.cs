using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace WebSocketCommon.Services.Data
{
    public interface IDataService
    {
        void SetConnectionString(string connectionString);
        DbCommand CreateCommand(string commandText, CommandType type = CommandType.Text);
        Task ExecuteNonQueryAsync(DbCommand dbCommand);
        Task<List<object[]>> ExecuteReaderAsync(DbCommand dbCommand);
    }
}
