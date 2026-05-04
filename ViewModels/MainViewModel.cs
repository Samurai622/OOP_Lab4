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

        public ICommand ShowTask1Command { get; }
        public ICommand ShowTask2Command { get; }
        public ICommand OpenWebCommand { get; }

        public MainViewModel(IBrowserService browserService)
        {
            _browserService = browserService;
            
            _currentViewModel = new Task1ViewModel();

            ShowTask1Command = new RelayCommand(_ => CurrentViewModel = new Task1ViewModel());
            ShowTask2Command = new RelayCommand(_ => CurrentViewModel = new Task2ViewModel());
            
            OpenWebCommand = new RelayCommand(_ => _browserService.OpenUrl("https://github.com/")); 
        }
    }
}