using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.Views;

public partial class CategoriesPage : ContentPage
{
    private DatabaseService _db;

    public CategoriesPage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadData();
    }

    async Task LoadData()
    {
        var cats = await _db.GetCategoriesAsync();
        catsCollection.ItemsSource = cats;
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        
        string result = await DisplayPromptAsync("Новая категория", "Введите название:");

        if (!string.IsNullOrWhiteSpace(result))
        {
            // Генерирация случайного цвета
            var random = new Random();
            var color = String.Format("#{0:X6}", random.Next(0x1000000));

            var newCat = new Category
            {
                Name = result,
                Color = color // Сохранение случайного цвета
            };

            await _db.SaveCategoryAsync(newCat);
            await LoadData(); 
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var btn = (Button)sender;
        var cat = (Category)btn.CommandParameter;

        if (cat.Name == "General")
        {
            await DisplayAlert("Ошибка", "Категорию General нельзя удалить!", "ОК");
            return;
        }

        bool answer = await DisplayAlert("Удаление", $"Удалить категорию {cat.Name}?", "Да", "Нет");
        if (answer)
        {
            
            await _db.DeleteCategoryAsync(cat);

            await LoadData();
        }
    }
}