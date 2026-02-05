using MauiApp1.Models;
using MauiApp1.Services;
using MauiApp1.Views;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private DatabaseService _dbService;
        private List<Models.TodoTask> _allTasks;

        
        public MainPage(DatabaseService dbService)
        {
            InitializeComponent();
            _dbService = dbService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadData();
        }

        private async Task LoadData()
        {
            _allTasks = await _dbService.GetTasksAsync();
            tasksCollection.ItemsSource = _allTasks;
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                tasksCollection.ItemsSource = _allTasks;
            }
            else
            {
                tasksCollection.ItemsSource = _allTasks.Where(t =>
                    t.Title.ToLower().Contains(e.NewTextValue.ToLower()) ||
                    (t.Project != null && t.Project.ToLower().Contains(e.NewTextValue.ToLower())));
            }
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            // Переход на страницу без параметра 
            await Shell.Current.GoToAsync(nameof(TaskDetailPage));
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is TodoTask selectedTask)
            {
                // Передача объекта на страницу редактирования
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Task", selectedTask }
                };
                await Shell.Current.GoToAsync(nameof(TaskDetailPage), navigationParameter);

                // Снимаем выделение
                tasksCollection.SelectedItem = null;
            }
        }
    }
}