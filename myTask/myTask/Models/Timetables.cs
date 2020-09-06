using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace myTask.Models
{
    public class WeeklyTimetable
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public int Number { get; set; }
        public string Year { get; set; }
        [OneToMany]
        public ICollection<DailyTimetable> Timetables { get; set; }
    }

    //avoid using DateTime as there is a lot of hassle with storing it and dealing with timezones and formats
    public class DailyTimetable
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Weekday Day { get; set; }
        public ICollection<MyTask> Tasks { get; set; }
        
        [ForeignKey(typeof(WeeklyTimetable))]
        public int WeeklyTimetableId { get; set; }

        [ManyToOne]
        public WeeklyTimetable Week { get; set; }
    }
}