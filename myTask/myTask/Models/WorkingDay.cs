using System;

namespace myTask.Models
{
    public class WorkingDay
    {
        public string DayOfWeekName { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public bool IsChecked { get; set; }
        public int NumberOfWorkingHours { get; set; } = 0;
    }
    
}