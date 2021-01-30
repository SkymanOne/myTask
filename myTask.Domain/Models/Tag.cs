using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace myTask.Domain.Models
{
    public class Tag
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Title { get; set; }
        [ManyToMany(typeof(AssignmentTag), CascadeOperations = CascadeOperation.All)]
        public virtual List<Assignment> Assignments { get; set; }
    }
}