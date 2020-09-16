using System;
using SQLiteNetExtensions.Attributes;

namespace myTask.Models
{
    //class for many-to-many relationship between Tag and MyTask models
    public class MyTaskTag
    {
        [ForeignKey(typeof(Tag))]
        public Guid TagId { get; set; }
        
        [ForeignKey(typeof(MyTask))]
        public Guid MyTaskId { get; set; }
    }
}