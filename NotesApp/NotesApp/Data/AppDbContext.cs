using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NotesApp.Models;

namespace NotesApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<TaskItem> Tasks { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Здесь указываем данные для входа в твой локальный PostgreSQL
            // Поменяй Password на свой!
            string connectionString = "Host=localhost;Port=5432;Database=NotesAppDb;Username=postgres;Password=1234";
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}