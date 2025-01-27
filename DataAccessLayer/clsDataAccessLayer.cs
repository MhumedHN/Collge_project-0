using System;
using System.Data;
using System.Data.SqlClient;

public class clsDataAccessLayer
{
    public static DataTable GetTasksByStatus()
    {
        try
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand("SELECT * FROM Tasks WHERE Mode = 1 OR IsChecked = 1", connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw new Exception($"Database error: {ex.Message}");
        }
    }
} 