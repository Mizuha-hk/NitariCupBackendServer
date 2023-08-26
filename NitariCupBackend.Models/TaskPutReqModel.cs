using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitariCupBackend.Models
{
    public class TaskPutReqModel
    { 
        public string title { get; set; }
        public string? description { get; set; }
        public DateTime startDate { get; set; }
        public DateTime limitDate { get; set; }
    }
}
