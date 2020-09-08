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
        [TextBlob("SubTasksBlobbed")]
        public List<string> SubTasks { get; set; }
        public string SubTasksBlobbed { get; set; }
        //set by user
        public double Importance { get; set; } = 0.5;
        //Calculated automatically
        public double PriorityCoefficient { get; set; } = 0.5;
        //set automatically as a hint for the user
        public PriorityLevel PriorityLevel { get; set; } = PriorityLevel.Low;
        public int Kinbens { get; set; } = 20;
        public DateTime Deadline { get; set; }
        public int DurationMinutes { get; set; }
        [ForeignKey(typeof(DailyTimetable))]
        public Guid WeekId { get; set; }
    }

    public enum PriorityLevel
    {
        Low,
        Medium,
        High
    }
}