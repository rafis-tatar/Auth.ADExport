using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auth.ADExport.Models;
using System.Data.SqlClient;
using System.Reflection;

namespace Auth.ADExport
{
    public class DatabaseService
    {
        public static string CreateConnectionString(string address, string catalog, string username, string password)
            => $"Server={address};Initial Catalog={catalog};Persist Security Info=False;User ID={username};Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";


        public void Export(List<ADUser> users)
        {
            using var connection = new SqlConnection(CreateConnectionString("<IP>", "<Catalog>", "<User name>", "<password>"));
            connection.Open();
            try
            {
                var query = new SqlCommand("TRUNCATE TABLE dbo.ADUser", connection);
                query.ExecuteNonQuery();
                FillTable<ADUser>(connection, users);
            }
            catch (Exception e)
            {
                connection.Close();
                throw;
            }
        }

        private void FillTable<T>(SqlConnection connection, List<T> list)
        {
            var table = new DataTable();

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
                table.Columns.Add(prop.Name, prop.PropertyType);

            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                foreach (var l in list)
                {
                    var values = new object[props.Length];
                    for (var i = 0; i < props.Length; i++)
                        values[i] = props[i].GetValue(l);
                    table.Rows.Add(values);
                }
                bulkCopy.DestinationTableName = "dbo.ADUser";
                bulkCopy.WriteToServer(table);
            }
        }
    }
}
