using System;
using SQLiteNetExtensions.Attributes;

namespace myTask.Models
{
    //class for many-to-many relationship between Tag and Assignment models
    public class AssignmentTag
    {
        [ForeignKey(typeof(Tag))]
        public Guid TagId { get; set; }
        
        [ForeignKey(typeof(Assignment))]
        public Guid AssignmentId { get; set; }
    }
}