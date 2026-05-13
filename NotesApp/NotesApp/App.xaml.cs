using System.Configuration;
using System.Data;
using System.Windows;
using NotesApp.Data; // Добавляем using

namespace NotesApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            base.OnStartup(e);

            // Создаем БД и таблицы до запуска главного окна
            DatabaseHelper.InitializeDatabase();
        }
    }
}
