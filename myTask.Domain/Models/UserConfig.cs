using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace myTask.Domain.Models
{
    public class UserConfig
    {
        [PrimaryKey]
        public Guid Id { get; set; } = default;
        public string Nickname { get; set; } = "Guest";
        public string Email { get; set; } = "";
        public bool IsInit { get; set; } = false;
        public int Kinbens { get; set; }

        [TextBlob(nameof(AvailableHoursBlobbed))]
        public double[] WeeklyAvailableTimeInHours { get; set; } = new double[]
        {
            4, //Sunday
            4, //Monday
            4, //Tuesday
            4, //Wednesday
            4, //Thursday
            4, //Friday
            4  //Saturday
        };
        public string AvailableHoursBlobbed { get; set; }
    }
}