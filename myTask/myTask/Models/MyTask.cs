using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace myTask.Models
{
    //chose the name to avoid confusion with System.Task
    public class MyTask
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public byte[] Icon { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [ManyToMany(typeof(MyTaskTag))]
        public virtual ICollection<Tag> Tags { get; set; }
        public List<string> SubTasks { get; set; }
        public double PriorityLevel { get; set; }
        
        //Calculated manually
        public double PriorityCoefficient { get; set; }
        public int Kinbens { get; set; }
        public DateTime Deadline { get; set; }
        public int DurationMinutes { get; set; }
    }
}