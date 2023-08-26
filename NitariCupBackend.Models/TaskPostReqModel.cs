using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitariCupBackend.Models
{
    public class TaskPostReqModel
    {
        public string AccessToken { get; set; }
        public string title { get; set; }
        public string? description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime LimitDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
