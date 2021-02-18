using System;
using SQLiteNetExtensions.Attributes;

namespace myTask.Domain.Models
{
    public class UserUpdate
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        
        //leave for the development of web api
        public Guid UserId { get; set; }
        
        [ForeignKey(typeof(Assignment))]
        public Guid AssignmentId { get; set; }
        
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public Assignment Assignment { get; set; }
    }

    public enum UpdateType
    {
        //TODO: 
    }
    
}