using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

namespace NotesApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // ТОЛЬКО ОДИН РАЗ!
        [ObservableProperty] private string _searchQuery = string.Empty;

        [ObservableProperty] private TaskItem? _selectedTask;
        [ObservableProperty] private object? _selectedItem;


        public ObservableCollection<Project> Projects { get; set; } = new();

        public MainViewModel()
        {
            LoadDataFromDb();
        }

        // Команда для поиска по Enter
        [RelayCommand]
        private void Search()
        {
            LoadDataFromDb(SearchQuery);
        }

        public void LoadDataFromDb(string? filter = null)
        {
            using var db = new AppDbContext();

            bool isSearching = !string.IsNullOrWhiteSpace(filter);
            string? f = isSearching ? filter!.ToLower() : null;

            // Загружаем проекты и задачи
            var allProjects = db.Projects.Include(p => p.Tasks).ToList();

            Projects.Clear();

            foreach (var proj in allProjects)
            {
                var filteredTasks = proj.Tasks.Where(t =>
                    !isSearching ||
                    t.Title.ToLower().Contains(f!) ||
                    (t.ContentRtf != null && t.ContentRtf.ToLower().Contains(f!))
                ).ToList();

                bool projectMatches = isSearching && proj.Name.ToLower().Contains(f!);

                if (!isSearching || projectMatches || filteredTasks.Any())
                {
                    proj.Tasks = filteredTasks;
                    proj.IsExpanded = isSearching;
                    Projects.Add(proj);
                }
            }
        }

        [RelayCommand]
        private void Save(object? parameter)
        {
            if (SelectedTask == null) return;

            // parameter — это будет наша RTF строка из редактора
            if (parameter is string rtfText)
            {
                SelectedTask.ContentRtf = rtfText;
            }

            using var db = new AppDbContext();
            db.Tasks.Update(SelectedTask);
            db.SaveChanges();
            System.Windows.MessageBox.Show("Успешно сохранено в PostgreSQL!");
        }

        [RelayCommand]
        private void AddProject()
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox("Введите название проекта:", "Новый проект", "Новый проект");
            if (string.IsNullOrWhiteSpace(name)) return;

            using var db = new AppDbContext();

            // Находим нашего админа в базе по имени, а не по ID
            var currentUser = db.Users.FirstOrDefault(u => u.Username == "admin");

            if (currentUser == null)
            {
                System.Windows.MessageBox.Show("Критическая ошибка: пользователь admin не найден в базе!");
                return;
            }

            var newProject = new Project
            {
                Name = name,
                UserId = currentUser.Id // Используем реальный ID из базы
            };

            db.Projects.Add(newProject);
            db.SaveChanges();

            LoadDataFromDb();
        }

        [RelayCommand]
        private void AddTask()
        {
            using var db = new AppDbContext();
            int targetProjectId = 0;

            // Логика: если выбран проект — берем его ID. 
            // Если выбрана задача — берем ID её проекта.
            if (SelectedItem is Project p) targetProjectId = p.Id;
            else if (SelectedItem is TaskItem t) targetProjectId = t.ProjectId;

            if (targetProjectId == 0)
            {
                System.Windows.MessageBox.Show("Сначала выберите проект в дереве!");
                return;
            }

            string title = Microsoft.VisualBasic.Interaction.InputBox("Введите заголовок задачи:", "Новая задача", "Новая задача");
            if (string.IsNullOrWhiteSpace(title)) return;

            db.Tasks.Add(new TaskItem { Title = title, ProjectId = targetProjectId, ContentRtf = "" });
            db.SaveChanges();
            LoadDataFromDb();
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedItem == null) return;

            using var db = new AppDbContext();

            if (SelectedItem is TaskItem task)
            {
                if (System.Windows.MessageBox.Show($"Удалить задачу '{task.Title}'?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    db.Tasks.Remove(db.Tasks.Find(task.Id));
                }
            }
            else if (SelectedItem is Project project)
            {
                if (System.Windows.MessageBox.Show($"Удалить проект '{project.Name}' и ВСЕ его задачи?", "Удаление проекта", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    // Находим проект в базе со всеми его задачами и удаляем
                    var pToDelete = db.Projects.Include(x => x.Tasks).FirstOrDefault(x => x.Id == project.Id);
                    if (pToDelete != null) db.Projects.Remove(pToDelete);
                }
            }

            db.SaveChanges();
            SelectedItem = null;
            LoadDataFromDb();
        }

        [RelayCommand]
        private void Logout()
        {
            var login = new Views.LoginWindow();
            login.Show();
            System.Windows.Application.Current.MainWindow.Close();
        }
    }
}