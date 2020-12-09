using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace myTask.Models
{
    public class Tag
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Title { get; set; }
        [ManyToMany(typeof(AssignmentTag))]
        public virtual List<Assignment> Assignments { get; set; }
    }
}