using Data_layer__DataAccess;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Business_layer_main
{
    public class clsBusinessLayer
    {
        static public bool FindRecordBYID(int Id)
        {
            return clsDataAccessLayer.SearchReacordBYId(Id);
        }

        static public bool DeletTaskBYID(int id)
        {
            return clsDataAccessLayer.DeletTaskFromDatabase(id);
        }                                                                                        

        public static bool EditTask(int id, string title, string description, string hashtag, DateTime StartDate, DateTime EndDate)
        {
            if (FindRecordBYID(id))
            {
                clsDataAccessLayer.EditTask(id, title, description, hashtag, StartDate, EndDate);
                return true;
            }
            return false;
        }

        static public DataTable GetAllRecord()
        {
            return clsDataAccessLayer.GetAllRecord();
        }

        public static byte ReturnNumberOfRecordBYMode(byte mode = 0)
        {
            return clsDataAccessLayer.CountRecordByMode(mode);
        }

        public static void SendDataToDatabaseLaer(string Title, string Description, string CourseId, byte Mode, DateTime StartDate, DateTime EndDate)
        {
            clsDataAccessLayer.InsertTaskToData(Title, Description, CourseId, Mode, StartDate, EndDate);
        }

        public static bool DeleteTaskById(int taskId)
        {
            return clsDataAccessLayer.DeleteTaskById(taskId);
        }

        public static bool UpdateTaskMode(int taskId)
        {
            return clsDataAccessLayer.UpdateTaskMode(taskId);
        }

        public static DataTable GetTaskById(int taskId)
        {
            return clsDataAccessLayer.GetTaskById(taskId);
        }

        public static DataTable GetAllCourses()
        {
            return clsDataAccessLayer.GetAllCourses();
        }

        public static DataTable GetCompletedTasks()
        {
            try
            {
                return clsDataAccessLayer.GetTasksByStatus(1);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error getting completed tasks: {ex.Message}", ex);
            }
        }

        public static DataTable GetTasksByStatus(byte status)
        {
            try
            {
                return clsDataAccessLayer.GetTasksByStatus(status);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error getting tasks by status: {ex.Message}", ex);
            }
        }
    }
}