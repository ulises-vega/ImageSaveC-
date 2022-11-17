using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSaverCSharp
{
    public class SQLDbParameter
    {
        public string ParameterName { get; set; }
        public SqlDbType SqlDbType { get; set; }
        public object Value { get; set; }
    }


    public static class SQLUtils
    {
        public static SqlConnection GetNewConnection()
        {
            return new SqlConnection(@"Data Source=ULISES;Database=DocumentSystem;Integrated Security=true");
        }

        public static int ExecuteNonQuery(string query, ICollection<SQLDbParameter> parameters)
        {
            var rowsAffected = 0;
            using (var conn = GetNewConnection())
            {
                var command = new SqlCommand(query, conn);
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter.ParameterName, parameter.SqlDbType).Value = parameter.Value;
                }
                conn.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            return rowsAffected;
        }

        public static DataTable ExecuteSelectQuery(string query)
        {
            DataTable dataTable;
            using (var cn = GetNewConnection())
            {
                var adapter = new SqlDataAdapter(query, cn);
                dataTable = new DataTable();
                adapter.Fill(dataTable);
            }
            return dataTable;
        }
    }
}
