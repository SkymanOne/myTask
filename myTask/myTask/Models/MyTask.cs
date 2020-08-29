using System;
using System.Collections.Generic;

namespace myTask.Models
{
    //chose the name to avoid confusion with System.Task
    public class MyTask
    {
        public int Id { get; set; }
        public byte[] Icon { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
        public List<string> SubTasks { get; set; }
        public double PriorityLevel { get; set; }
        public int Kinbens { get; set; }
        public DateTime Deadline { get; set; }
        public int Minutes { get; set; }
    }
}