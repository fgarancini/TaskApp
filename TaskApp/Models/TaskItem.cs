using System.ComponentModel.DataAnnotations.Schema;

namespace TaskApp.Models
{
    [Table("tasks")]
    public class TaskItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool Completed { get; set; }
        public DateTime Deadline { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
        public TaskItem()
        {
            CreatedAt = DateTime.UtcNow; 
            UpdatedAt = DateTime.UtcNow;
        }

    }

    
}