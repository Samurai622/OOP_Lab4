using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using OOP_Lab4.Services;
using OOP_Lab4.ViewModels;

namespace OOP_Lab4
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var browserService = new BrowserService();
            DataContext = new MainViewModel(browserService);
        }

        // =====================================
        // ПОДІЯ: ПОКАЗАТИ JSON
        // =====================================
        private async void ShowJson_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                string json = vm.GetCurrentJson();
                var jsonWindow = new JsonWindow(json);
                await jsonWindow.ShowDialog(this);
            }
        }

        // =====================================
        // ПОДІЯ: ЗБЕРЕГТИ JSON В ФАЙЛ
        // =====================================
        private async void SaveJson_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                string json = vm.GetCurrentJson();

                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel == null) return;

                var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Зберегти JSON",
                    SuggestedFileName = "data.json",
                    DefaultExtension = "json",
                    FileTypeChoices = new[] { new FilePickerFileType("JSON Files") { Patterns = new[] { "*.json" } } }
                });

                if (file != null)
                {
                    await using var stream = await file.OpenWriteAsync();
                    using var writer = new StreamWriter(stream);
                    await writer.WriteAsync(json);
                }
            }
        }

        private void ChangeDb_Click(object sender, RoutedEventArgs e)
        {
            var startupWindow = new StartupWindow();
            startupWindow.Show();

            if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = startupWindow;
            }
            this.Close();
        }
    }
}