using System.Text.Json;
using System.Windows.Input;
using OOP_Lab4.Commands;
using OOP_Lab4.Services;
using OOP_Lab4.Tasks.Task1; 
using OOP_Lab4.Tasks.Task2; 

namespace OOP_Lab4.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IBrowserService _browserService;
        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        // ==========================================
        // Змінні для підсвічування кнопок 
        // ==========================================
        private bool _isTask1Active = true;
        public bool IsTask1Active 
        { 
            get => _isTask1Active; 
            set => SetProperty(ref _isTask1Active, value); 
        }

        private bool _isTask2Active = false;
        public bool IsTask2Active 
        { 
            get => _isTask2Active; 
            set => SetProperty(ref _isTask2Active, value); 
        }

        public ICommand ShowTask1Command { get; }
        public ICommand ShowTask2Command { get; }
        public ICommand OpenWebCommand { get; }

        public MainViewModel(IBrowserService browserService)
        {
            _browserService = browserService;
            
            _currentViewModel = new Task1ViewModel(); 

            ShowTask1Command = new RelayCommand(_ => { 
                CurrentViewModel = new Task1ViewModel();
                IsTask1Active = true; 
                IsTask2Active = false; 
            });
            
            ShowTask2Command = new RelayCommand(_ => { 
                CurrentViewModel = new Task2ViewModel();
                IsTask1Active = false; 
                IsTask2Active = true; 
            });
            
            OpenWebCommand = new RelayCommand(_ => _browserService.OpenUrl("http://localhost:3000/")); 
        }

        // Метод для генерації JSON (Для наступного етапу)
        public string GetCurrentJson()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            if (CurrentViewModel is Task1ViewModel t1) return JsonSerializer.Serialize(t1.Channels, options);
            if (CurrentViewModel is Task2ViewModel t2) return JsonSerializer.Serialize(t2.Competitions, options);
            return "{}";
        }
    }
}