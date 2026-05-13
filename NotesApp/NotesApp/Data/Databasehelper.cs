using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotesApp.Models;
using System.Security.Cryptography;
namespace NotesApp.Data
{
    public static class DatabaseHelper
    {
        public static void InitializeDatabase()
        {
            using var context = new AppDbContext();

            // Эта магия сама создаст базу данных и все таблицы, если их нет (IF NOT EXISTS)
            context.Database.EnsureCreated();

            // Проверяем, есть ли уже пользователь admin
            if (!context.Users.Any(u => u.Username == "admin"))
            {
                // Создаем тестовые данные из ТЗ
                var adminUser = new User
                {
                    Username = "admin",
                    PasswordHash = HashPassword("admin") // Хэшируем по требованиям ТЗ
                };

                var testProject = new Project
                {
                    Name = "Учебный проект",
                    User = adminUser
                };

                var task1 = new TaskItem { Title = "Задача 1", ContentRtf = "", Project = testProject };
                var task2 = new TaskItem { Title = "Задача 2", ContentRtf = "", Project = testProject };

                context.Users.Add(adminUser);
                context.Projects.Add(testProject);
                context.Tasks.AddRange(task1, task2);

                context.SaveChanges();
            }
        }

        // Метод для шифрования пароля (SHA256)
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }
    }
}
