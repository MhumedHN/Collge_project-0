using System;
using System.Data;
using System.Data.SqlClient;

namespace Data_layer__DataAccess
{
    public class clsDataAccessLayer
    {
        public static void InsertTaskToData(string Title, string Description, string CourseId, byte Mode, DateTime StartDate, DateTime EndDate)
        {
            string connectionString = StringConnection.ConnectionString; 
            string query = @"
                INSERT INTO [dbo].[Task]
                   ([Title]
                   ,[Description]
                   ,[mode]
                   ,[CourseID]
                   ,[start date]
                   ,[end date])
                VALUES
                   (@Title, @Description, @Mode, @CourseID, @StartDate, @EndDate)";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Title", Title);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@Mode", Mode);
                        cmd.Parameters.AddWithValue("@CourseID", string.IsNullOrEmpty(CourseId) ? (object)DBNull.Value : Convert.ToInt32(CourseId));
                        cmd.Parameters.AddWithValue("@StartDate", StartDate);
                        cmd.Parameters.AddWithValue("@EndDate", EndDate);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error inserting task: {ex.Message}", ex);
            }
        }

        public static bool DeleteTaskById(int taskId)
        {
            string connectionString = StringConnection.ConnectionString;
            string query = "DELETE FROM [dbo].[Task] WHERE ID = @TaskId";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TaskId", taskId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting task: {ex.Message}", ex);
            }
        }

        public static bool UpdateTaskMode(int taskId)
        {
            string connectionString = StringConnection.ConnectionString;
            string query = "UPDATE [dbo].[Task] SET [mode] = 1 WHERE ID = @TaskId";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TaskId", taskId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating task mode: {ex.Message}", ex);
            }
        }

        public static void EditTask(int id, string title, string description, string hashtag, DateTime StartDate, DateTime EndDate)
        {
            string connectionString = StringConnection.ConnectionString;
            string query = @"
                UPDATE [dbo].[Task]
                SET [Title] = @Title
                   ,[Description] = @Description
                   ,[start date] = @StartDate
                   ,[end date] = @EndDate
                WHERE ID = @Id";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Title", title);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@StartDate", StartDate);
                        cmd.Parameters.AddWithValue("@EndDate", EndDate);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error editing task: {ex.Message}", ex);
            }
        }

        public static byte CountRecordByMode(byte mode)
        {
            string connectionString = StringConnection.ConnectionString;
            string query = "SELECT COUNT(*) FROM [dbo].[Task] WHERE [mode] = @Mode";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Mode", mode);
                        return Convert.ToByte(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error counting tasks: {ex.Message}", ex);
            }
        }

        public static DataTable GetTaskById(int taskId)
        {
            string connectionString = StringConnection.ConnectionString;
            string query = "SELECT * FROM [dbo].[Task] WHERE ID = @TaskId";
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TaskId", taskId);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error getting task: {ex.Message}", ex);
            }
        }

        public static DataTable GetAllCourses()
        {
            string connectionString = StringConnection.ConnectionString;
            string query = "SELECT * FROM [dbo].[Course]";
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error getting courses: {ex.Message}", ex);
            }
        }

        public static DataTable GetTasksByStatus(byte status = 1)
        {
            string connectionString = StringConnection.ConnectionString;
            string query = @"
                SELECT * 
                FROM [dbo].[Task]
                WHERE [mode] = @Status
                ORDER BY [start date]";

            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Status", status);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error getting tasks by status: {ex.Message}", ex);
            }
        }

        public static bool SearchReacordBYId(int ID)
        {
            string connectionString = StringConnection.ConnectionString;
            string query = "SELECT COUNT(*) FROM [dbo].[Task] WHERE ID = @id";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", ID);
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error searching for task: {ex.Message}", ex);
            }
        }

        public static bool DeletTaskFromDatabase(int Id)
        {
            string connectionString = StringConnection.ConnectionString;
            string query = "DELETE FROM [dbo].[Task] WHERE ID = @id";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", Id);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting task: {ex.Message}", ex);
            }
        }

        public static DataTable GetAllRecord()
        {
            string connectionString = StringConnection.ConnectionString;
            string query = "SELECT * FROM [dbo].[Task] ORDER BY [start date]";
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error getting all tasks: {ex.Message}", ex);
            }
        }
    }
}
