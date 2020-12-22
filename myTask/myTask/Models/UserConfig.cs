using System;
using System.Collections.Generic;

namespace myTask.Models
{
    public class UserConfig
    {
        public Guid Id { get; set; } = default;
        public string Nickname { get; set; } = "Guest";
        public string Email { get; set; } = "";
        public bool IsInit { get; set; } = false;

        public List<double> WeeklyAvailableTimeInHours { get; set; } = new List<double>()
        {
            0, //Sunday
            4, //Monday
            4, //Tuesday
            4, //Wednesday
            4, //Thursday
            4, //Friday
            4 //Saturday
        };
    }
}