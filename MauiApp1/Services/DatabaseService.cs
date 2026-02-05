using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using MauiApp1.Models;

namespace MauiApp1.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        async Task Init()
        {
            if (_database != null)
                return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "MyToDo.db3");
            _database = new SQLiteAsyncConnection(dbPath);

            //Создаение таблицы
            await _database.CreateTableAsync<Category>();
            await _database.CreateTableAsync<TodoTask>();

            // Добавим категорию по умолчанию, если пусто
            if (await _database.Table<Category>().CountAsync() == 0)
            {
                await _database.InsertAsync(new Category { Name = "General", Color = "#cccccc" });
            }
        }

        public async Task<List<TodoTask>> GetTasksAsync()
        {
            await Init();
            return await _database.Table<TodoTask>().OrderByDescending(t => t.Id).ToListAsync();
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            await Init();
            return await _database.Table<Category>().ToListAsync();
        }

        public async Task SaveTaskAsync(TodoTask task)
        {
            await Init();
            if (task.Id != 0)
                await _database.UpdateAsync(task);
            else
                await _database.InsertAsync(task);
        }

        public async Task SaveCategoryAsync(Category category)
        {
            await Init();
            if (category.Id != 0)
                await _database.UpdateAsync(category);
            else
                await _database.InsertAsync(category);
        }

        public async Task DeleteTaskAsync(TodoTask task)
        {
            await Init();
            await _database.DeleteAsync(task);
        }
        public async Task DeleteCategoryAsync(Category cat)
        {
            await Init();
            await _database.DeleteAsync(cat);
        }
    }
}
