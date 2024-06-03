using Microsoft.Data.SqlClient;

/*#################### SELECT  FROM  ADO.NET ####################*/
const string connectionString = "Server=localhost,1433;Database=balta;User ID=sa;Password=Luanna21**;TrustServerCertificate=true";
using (var connection = new SqlConnection(connectionString))
{
    connection.Open();
    using (var command = new SqlCommand()) 
    {
        command.Connection = connection;
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = "SELECT ID, TITLE FROM CATEGORY";

        var reader = command.ExecuteReader();
        while (reader.Read()) {
            Console.WriteLine($"{reader.GetGuid(0)} - {reader.GetString(1)}");
        }
    }
}