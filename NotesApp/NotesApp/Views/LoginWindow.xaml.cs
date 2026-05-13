using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NotesApp.Data;

namespace NotesApp.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string user = TxtUsername.Text;
            string pass = TxtPassword.Password;
            string hashedPass = DatabaseHelper.HashPassword(pass);

            using (var db = new AppDbContext())
            {
                // Ищем пользователя в PostgreSQL
                var dbUser = db.Users.FirstOrDefault(u => u.Username == user && u.PasswordHash == hashedPass);

                if (dbUser != null)
                {
                    // Если нашли — открываем главное окно
                    MainWindow main = new MainWindow();
                    main.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!");
                }
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
