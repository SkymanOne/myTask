using System.Collections.Generic;

namespace myTask.Models
{
    public class WeeklyTimetable
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Year { get; set; }
        public ICollection<DailyTimetable> Timetables { get; set; }
    }

    //avoid using DateTime as there is a lot of hassle with storing it and dealing with timezones and formats
    public class DailyTimetable
    {
        public int Id { get; set; }
        public Weekday Day { get; set; }
        public ICollection<MyTask> Tasks { get; set; }
    }
}