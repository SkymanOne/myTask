using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace myTask.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}