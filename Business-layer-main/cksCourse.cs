using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_layer_main
{
    internal class clsCourse
    {
        public byte id { get; set; }
        public string courseName { get; set; } 
        public byte difficulty { get; set; }        
        public string hashtag { get; set;  }
        public byte DayOfWeek { get; set; }
    }                                        
}
