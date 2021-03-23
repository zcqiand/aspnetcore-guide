using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoWebApp02.Models
{
    [Table("Todo")]
    public class Todo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
