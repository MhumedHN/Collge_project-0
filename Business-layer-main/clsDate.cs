using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Business_layer_main
{
     public class   clsDate
    {
        
        
        public static int GetDayAsNumber(DateTime date)
        {
            return (int)date.DayOfWeek + 1;
        }

   
        public static string GetDayAsString(DateTime date)
        {

            return date.DayOfWeek.ToString();
        }

     
        public static int CountDaysLeft(DateTime startDate, DateTime endDate)
        {
            TimeSpan difference = endDate.Date - startDate.Date;
            return difference.Days;
        }
    }
}
