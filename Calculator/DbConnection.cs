using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public class DbConnection
    {
        private readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\testDB\DB_Calculations.mdf;Integrated Security=True";

        public bool DatabaseAdd(string equasion, string result)
        {
            SqlConnection dbConnection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("INSERT INTO CalculatorDB VALUES('" + equasion + "','" + result + "')", dbConnection);

            try
            {
                dbConnection.Open();
                cmd.ExecuteNonQuery();
                dbConnection.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<string> PrivewSavedData()
        {
            List<string> columnData = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM CalculatorDB", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!char.IsDigit(reader.GetString(1).LastOrDefault())) // && reader.GetString(1).LastOrDefault() != '%')
                            {
                                columnData.Add(reader.GetString(1).Remove(reader.GetString(1).Length - 1) + "=" + reader.GetString(2));
                            }
                            else
                            {
                                columnData.Add(reader.GetString(1) + "=" + reader.GetString(2));
                            }
                        }
                    }
                }
            }

            columnData.Reverse();

            return columnData;
        }
    }
}
