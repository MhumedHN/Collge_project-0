using Business_layer_main;
using System;
using System.Data;

namespace Data_layer__DataAccess
{

    public enum enMode { NormalMode = 0, ActiveMode = 1, DeletMode = 2 }
    internal class clsTasks
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Hashtag { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public enMode Mode { get; set; }
        public clsTasks()
        {
            Id = -1;
            Title = "";
            Description = "";
            Hashtag = "";
            StartDate = DateTime.Now;
            EndDate = new DateTime(2035, 1, 1);
            Mode = 0;

        }
        public clsTasks(int id, string title, string description, string hashtag, DateTime StartDate, DateTime EndDate, enMode Mode)
        {
            this.Id = id;
            this.Title = title;
            this.Description = description;
            this.Hashtag = hashtag;
            this.Mode = Mode;
            this.StartDate = StartDate;
            this.EndDate = EndDate;

        }
        // Add task   
        //public static void AddTask()
        //{
        //    clsTasks NewCard = new clsTasks();
        //    // I set the defulat here is ( Mode = 0  ) witch mean the mod is normall when we insert the card to datebase 
        //    NewCard.Mode = enMode.NormalMode;

        //    clsDataAccessLayer.InsertTaskToData(NewCard.Id, NewCard.Title, NewCard.Description, NewCard.Hashtag, (byte)NewCard.Mode, NewCard.StartDate, NewCard.EndDate);



        //}
        
        // Find Task  
        public static bool FindTask(int ID)
        {
           return  clsBusinessLayer.FindRecordBYID(ID); 
          
        }
        // Delet task 
        public static bool DeletTaskByID (int ID)
        {
                return clsBusinessLayer.DeletTaskBYID(ID); 
        }
        // Edit task 
        public static bool  EditTask (int id, string title, string description, string hashtag, DateTime StartDate, DateTime EndDate)
        {
            if (FindTask(id))
            {
                clsBusinessLayer.EditTask(id,title, description, hashtag, StartDate, EndDate);
                    return true;
            }
            return false; 
              
       
        }
        // get all Data 
        public static DataTable GetAllRecord ()
        {
           
                return clsBusinessLayer.GetAllRecord();
        }
       

    }




}
