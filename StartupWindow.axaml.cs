using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;

namespace OOP_Lab4
{
    public partial class StartupWindow : Window
    {
        public StartupWindow()
        {
            InitializeComponent();
        }

        private void LocalBtn_Click(object sender, RoutedEventArgs e)
        {
            AppConfig.ApiBaseUrl = "http://localhost:3000/api";
            OpenMainWindow();
        }

        private void ExternalBtn_Click(object sender, RoutedEventArgs e)
        {
            // Тут ви можете вписати реальну адресу вашого зовнішнього сервера, 
            // якщо завантажите server.js на хостинг (наприклад, render.com)
            // Поки що поставимо заглушку або вашу майбутню адресу:
            AppConfig.ApiBaseUrl = "https://oop-lab4.onrender.com/api"; 
            
            OpenMainWindow();
        }

        private void OpenMainWindow()
        {
            // Створюємо головне вікно
            var mainWindow = new MainWindow();
            mainWindow.Show();

            // Змінюємо головне вікно додатку в системі Avalonia
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = mainWindow;
            }

            // Закриваємо стартове вікно
            this.Close();
        }
    }
}