using System;
using System.Windows.Input;
using OOP_Lab4.Commands;
using OOP_Lab4.Models;
using OOP_Lab4.ViewModels;

namespace OOP_Lab4.Tasks.Task2
{
    public class PerformanceEditViewModel : ViewModelBase
    {
        public PerformanceModel Model { get; }
        public bool IsSaved { get; private set; } = false;
        public bool HasChanges { get; set; } = true;

        private string _errorMessage = string.Empty;
        public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public Action CloseAction { get; set; }

        public PerformanceEditViewModel(PerformanceModel model)
        {
            Model = model;
            SaveCommand = new RelayCommand(_ => {
                string err = Model.GetValidationError();
                if (!string.IsNullOrEmpty(err)) { ErrorMessage = err; return; }
                IsSaved = true; HasChanges = false; CloseAction?.Invoke();
            });
            CancelCommand = new RelayCommand(_ => { HasChanges = false; CloseAction?.Invoke(); });
        }
    }
}