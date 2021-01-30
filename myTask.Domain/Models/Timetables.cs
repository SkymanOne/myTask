using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace myTask.Domain.Models
{
    public class WeeklyTimetable
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public int Number { get; set; }
        public int Year { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<DailyTimetable> Timetables { get; set; }
    }

    //avoid using DateTime as there is a lot of hassle with storing it and dealing with timezones and formats
    public class DailyTimetable
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public int DayNumber { get; set; }
        public DayOfWeek Day { get; set; }
        public double AvailableTimeInHours { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Assignment> Assignments { get; set; }
        
        [ForeignKey(typeof(WeeklyTimetable))]
        public Guid WeeklyTimetableId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public WeeklyTimetable Week { get; set; }
    }
}