using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Npgsql;
using System.Data.SqlClient;
using System.Text;

namespace SixtyNames.Data
{
    public class SqlQueryHandler
    {
        private string _connectionString;
        public SqlQueryHandler(string connectionString)
        {
            _connectionString = connectionString;
        }
        public string GetDataString(string query)
        {

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Подключение открыто");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Не удалось установить соединение с БД");
                    throw;
                }

                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                using (var result = command.ExecuteReader())
                {
                    StringBuilder resultString = new StringBuilder();

                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            for (int i = 0; i < result.FieldCount; i++)
                            {
                                resultString.Append(result[i].ToString());
                                if (i < result.FieldCount - 1)
                                {
                                    resultString.Append(", ");
                                }
                            }
                            resultString.AppendLine();
                        }
                    }
                    return resultString.ToString();
                }
                return null;
            }
        }
        public void GetDataReader(string query)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                Console.WriteLine("Подключение открыто");

                using (var command = new NpgsqlCommand(query, connection))
                using (var dataReader = command.ExecuteReader())
                {
                    ReportGenerator reportGenerator = new ReportGenerator();
                    reportGenerator.GenerateReport(dataReader);
                }
            }
        }
    }
}
