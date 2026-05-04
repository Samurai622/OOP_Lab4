using Avalonia.Controls;
using OOP_Lab4.Services;
using OOP_Lab4.ViewModels;

namespace OOP_Lab4;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Ініціалізуємо сервіси та ViewModel
        var browserService = new BrowserService();
        DataContext = new MainViewModel(browserService);
    }
}