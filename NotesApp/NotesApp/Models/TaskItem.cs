using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotesApp.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string ContentRtf { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int ProjectId { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public virtual Project Project { get; set; } = null!;

        // Для подзадач
        public int? ParentTaskId { get; set; }
        [ForeignKey(nameof(ParentTaskId))]
        public virtual TaskItem? ParentTask { get; set; }

        public virtual ICollection<TaskItem> SubTasks { get; set; } = new List<TaskItem>();
        [NotMapped]
        public bool IsExpanded { get; set; } = false;
    }
}
