using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotesApp.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
        [NotMapped]
        public bool IsExpanded { get; set; } = false; // По умолчанию пусть будут открыты
    }
}
