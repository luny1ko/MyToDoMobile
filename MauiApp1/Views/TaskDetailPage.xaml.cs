using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.Views;

[QueryProperty(nameof(Task), "Task")]
public partial class TaskDetailPage : ContentPage
{
    private TodoTask _task;
    private readonly DatabaseService _dbService; 

    public TodoTask Task
    {
        get => _task;
        set
        {
            _task = value;
            OnPropertyChanged();
            if (_task != null)
            {
                TaskDate = _task.Date ?? DateTime.Now;
                IsExisting = _task.Id != 0;
                OnPropertyChanged(nameof(IsExisting));
            }
        }
    }

    private DateTime _taskDate;
    public DateTime TaskDate
    {
        get => _taskDate;
        set { _taskDate = value; OnPropertyChanged(); }
    }

    private bool _isExisting;
    public bool IsExisting
    {
        get => _isExisting;
        set { _isExisting = value; OnPropertyChanged(); }
    }

    public List<Category> Categories { get; set; }

    private Category _selectedCategory;
    public Category SelectedCategory
    {
        get => _selectedCategory;
        set { _selectedCategory = value; OnPropertyChanged(); }
    }

   
    public TaskDetailPage(DatabaseService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Загрузка категории
        Categories = await _dbService.GetCategoriesAsync();
        OnPropertyChanged(nameof(Categories));

        if (Task == null)
        {
            Task = new TodoTask();
        }
        else if (Task.CategoryId != 0 && Categories != null)
        {
            SelectedCategory = Categories.FirstOrDefault(c => c.Id == Task.CategoryId);
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Task.Title))
        {
            await DisplayAlert("Ошибка", "Введите название", "ОК");
            return;
        }

        Task.Date = TaskDate;
        if (SelectedCategory != null)
            Task.CategoryId = SelectedCategory.Id;

        await _dbService.SaveTaskAsync(Task);
        await Shell.Current.GoToAsync("..");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Удаление", "Удалить задачу?", "Да", "Нет");
        if (answer)
        {
            await _dbService.DeleteTaskAsync(Task);
            await Shell.Current.GoToAsync("..");
        }
    }
}