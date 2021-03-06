using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace myTask.Domain.Models
{
    //chose the name to avoid confusion with System.Task
    public class Assignment
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public byte[] Icon { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } = "No description provided";

        [ManyToMany(typeof(AssignmentTag), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public virtual List<Tag> Tags { get; set; } = new List<Tag>();

        [TextBlob(nameof(SubTasksBlobbed))] public List<SubTask> SubTasks { get; set; } = new List<SubTask>();
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
        
        public int TimeElapsedSeconds { get; set; }

        public Status Status { get; set; } = Status.Created;

        [ForeignKey(typeof(DailyTimetable))]
        public Guid DayId { get; set; }
    }

    public enum PriorityLevel
    {
        Low,
        Medium,
        High
    }

    public enum Status
    {
        Created,
        Going,
        Paused,
        Finished
    }
}