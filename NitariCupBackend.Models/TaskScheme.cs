using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitariCupBackend.Models
{
    public class TaskScheme
    {
        public Guid Id { get; set; }
        public string userId { get; set; }
        public string title { get; set; }
        [AllowNull]
        public string? description { get; set; }
        public DateTime startDate { get; set; }
        public DateTime limitDate { get; set; }
        public DateTime createdAt { get; set; }
        public bool isDone { get; set; } = false;
        [AllowNull]
        public DateTime? DoneDate { get; set; }
        [AllowNull]
        public float? score { get; set; }
    }
}
