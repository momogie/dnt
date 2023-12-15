using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
namespace DNT
{
    class Db
    {
        public static string ConnectionString
        {
            get
            {
                return "Data source=.; Integrated Security=true";
            }
        }
        public static void Execute(string Query)
        {
            try
            {
                SqlConnection connection = new SqlConnection(ConnectionString);
                connection.Open();
                new SqlCommand(Query, connection).ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.HelpLink);
            }
        }
        public static bool Import(DataTable Data, string TableName, string DBName)
        {
            try
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy("Data source=.;Initial Catalog=" + DBName + "; Integrated Security=true", SqlBulkCopyOptions.KeepIdentity))
                {
                    sqlBulkCopy.BulkCopyTimeout = 600;
                    sqlBulkCopy.DestinationTableName = TableName;
                    sqlBulkCopy.WriteToServer(Data);

                    sqlBulkCopy.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.HelpLink);
                return false;
            }
        }
        public static DataTable Rs(string Query)
        {
            DataTable dataTable = new DataTable();
            SqlConnection selectConnection = new SqlConnection(ConnectionString);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(Query, selectConnection);
            DataSet dataSet = new DataSet();
            sqlDataAdapter.Fill(dataTable);
            selectConnection.Close();
            return dataTable;
        }
    }
}
