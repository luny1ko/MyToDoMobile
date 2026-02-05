using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MauiApp1.Models
{
    public class TodoTask
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Project { get; set; }
        public string Priority { get; set; } = "Low"; // Low, Medium, High
        public string Owner { get; set; }
        public int CategoryId { get; set; }
        public DateTime? Date { get; set; }
        public string Status { get; set; } = "Не начата"; // Не начата, В процессе, Выполнена

        // для отображения цвета приоритета в списке 
        [Ignore]
        public string PriorityColor => Priority switch
        {
            "High" => "#ff6b6b",    
            "Medium" => "#ffb86b",  
            _ => "#6bcf6b"          
        };
    }
}
